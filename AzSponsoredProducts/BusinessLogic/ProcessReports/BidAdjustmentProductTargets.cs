using AdTool.AzSponsoredProducts.AmazonAPI.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignManagement;
using Configuration;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class BidAdjustmentProductTargets
    {
        public async System.Threading.Tasks.Task<List<SaveKeywordHistory>> AdjustBid(ReportUser reportUser, List<PromoNegativeRules> promoNegativeRules, APIAuthorization auth, List<ClientProfileCodes> profileCodes)
        {
            try
            {



                //method preperation
                UpdateProductTarget updateProductApi = new UpdateProductTarget();
                List<DailyKeywordDataOutputForKeywords> keywordOutput = new List<DailyKeywordDataOutputForKeywords>();
                List<DailyKeywordDataOutputForKeywords> masterKeywordOutputGrouped = new List<DailyKeywordDataOutputForKeywords>();
                List<KeywordBidTracker> bidTrackers = new List<KeywordBidTracker>();
                List<KeywordBidTracker> allKeywordsToUpdate = new List<KeywordBidTracker>();
                List<KeywordBidTracker> finalKeywordsToUpdate = new List<KeywordBidTracker>();
                List<DailyKeywordDataOutputForKeywords> salesDataPerKeyword = new List<DailyKeywordDataOutputForKeywords>();
                List<SaveKeywordHistory> saveKeywordHistories = new List<SaveKeywordHistory>();
                List<AllCampaigns> allCampaigns = new List<AllCampaigns>();

                RetrieveData retrieveData = new RetrieveData();
                allCampaigns = await retrieveData.GetAllCampaigns(reportUser.ClientId);

                List<int> campaignIds = allCampaigns.Select(x => x.QAPCampaignId).ToList();

                //make cosmos client
                Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container KeywordDatacontainer = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosKeywordDataContainer, "/partitionKey");
                Microsoft.Azure.Cosmos.Container BidTrackingContainer = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosBidTrackingContainer, "/partitionKey");
                Microsoft.Azure.Cosmos.Container producttargetContainer = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosProductTargets, "/partitionKey");

                //bring in keywords that had a sale today
                DateTime CreationDate = DateTime.Now.AddDays(-1);

                IReadOnlyList<FeedRange> feedRanges = await KeywordDatacontainer.GetFeedRangesAsync();
                QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate and c.unitsSoldClicks14d > 0 and c.ProductId != 0 and c.ProductId != null and (c.keywordType != 'PHRASE'  and c.keywordType != 'EXACT' and  c.keywordType != 'BROAD')")
                       .WithParameter("@clientId", reportUser.ClientId.ToString())
                         .WithParameter("@startDate", CreationDate.ToUniversalTime());

                using (FeedIterator<DailyKeywordDataOutputForKeywords> feedIterator = KeywordDatacontainer.GetItemQueryIterator<DailyKeywordDataOutputForKeywords>(
                      feedRanges[0],
                      queryDefinition,
                      null,
                      new QueryRequestOptions() { }))
                {
                    // Iterate query result pages
                    while (feedIterator.HasMoreResults)
                    {
                        FeedResponse<DailyKeywordDataOutputForKeywords> snapshotResponse = await feedIterator.ReadNextAsync();

                        // Iterate query results
                        foreach (var item in snapshotResponse)
                        {
                            if (campaignIds.Contains(item.QAPCampaignId ?? 0))
                            {
                                keywordOutput.Add(item);
                            }
                        }
                    }
                }


                //simplify to make them easier to work with
                masterKeywordOutputGrouped = (from t in keywordOutput
                                              group t by new { t.keywordId, t.Country } into grp
                                              select new DailyKeywordDataOutputForKeywords
                                              {
                                                  keywordId = grp.Key.keywordId,
                                                  Country = grp.Key.Country
                                              }).ToList();


                foreach (var keyword in masterKeywordOutputGrouped)
                {
                    IReadOnlyList<FeedRange> feedRanges2 = await BidTrackingContainer.GetFeedRangesAsync();
                    QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.keywordId = @keywordId and c.CountryId = @CountryId  and c.KeywordType = 'ProductTarget'")
                           .WithParameter("@clientId", reportUser.ClientId.ToString())
                             .WithParameter("@CountryId", keyword.Country)
                      .WithParameter("@keywordId", keyword.keywordId);

                    using (FeedIterator<KeywordBidTracker> feedIterator2 = BidTrackingContainer.GetItemQueryIterator<KeywordBidTracker>(
                          feedRanges2[0],
                          queryDefinition2,
                          null,
                          new QueryRequestOptions() { }))
                    {
                        // Iterate query result pages
                        while (feedIterator2.HasMoreResults)
                        {
                            FeedResponse<KeywordBidTracker> snapshotResponse = await feedIterator2.ReadNextAsync();

                            // Iterate query results
                            foreach (var item in snapshotResponse)
                            {
                                bidTrackers.Add(item);
                            }
                        }
                    }
                }

                //identify records to update
                foreach (var ky in masterKeywordOutputGrouped)
                {
                    KeywordBidTracker bidTrackerFound = bidTrackers.Where(x => x.keywordId == ky.keywordId && x.CountryId == ky.Country).FirstOrDefault();

                    KeywordBidTracker newBidTrackerItem = new KeywordBidTracker();

                    if (bidTrackerFound == null)
                    {
                        newBidTrackerItem.ClientId = reportUser.aPIAuthorizationRequest.ClientId.ToString();
                        newBidTrackerItem.keywordId = ky.keywordId;
                        newBidTrackerItem.CountryId = ky.Country;
                        newBidTrackerItem.KeywordType = "ProductTarget";
                        newBidTrackerItem.LastUpdated = DateTime.Now.AddYears(-2);
                        allKeywordsToUpdate.Add(newBidTrackerItem);
                    }
                    else if (bidTrackerFound.LastUpdated < DateTime.Now.AddDays(-3))
                    {
                        allKeywordsToUpdate.Add(bidTrackerFound);
                    }
                }


                //loop through records to update
                foreach (var ky in allKeywordsToUpdate)
                {
                    //bring in all sales data from azure related to this keyword/country/client since the date
                    IReadOnlyList<FeedRange> feedRanges3 = await KeywordDatacontainer.GetFeedRangesAsync();
                    QueryDefinition queryDefinition3 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord > @startDate and c.Country = @Country and c.ProductId != 0 and c.ProductId != null and c.keywordId = @keywordId and c.clicks > 0")
                           .WithParameter("@clientId", reportUser.ClientId.ToString())
                             .WithParameter("@startDate", ky.LastUpdated.ToUniversalTime())
                                  .WithParameter("@Country", ky.CountryId)
                         .WithParameter("@keywordId", ky.keywordId);

                    using (FeedIterator<DailyKeywordDataOutputForKeywords> feedIterator3 = KeywordDatacontainer.GetItemQueryIterator<DailyKeywordDataOutputForKeywords>(
                          feedRanges3[0],
                          queryDefinition3,
                          null,
                          new QueryRequestOptions() { }))
                    {
                        // Iterate query result pages
                        while (feedIterator3.HasMoreResults)
                        {
                            FeedResponse<DailyKeywordDataOutputForKeywords> snapshotResponse = await feedIterator3.ReadNextAsync();

                            // Iterate query results
                            foreach (var item in snapshotResponse)
                            {
                                salesDataPerKeyword.Add(item);
                            }
                        }
                    }

                    //group results into a single record
                    DailyKeywordDataOutputForKeywords keywordOutputGroupedForThisKeyword = (from t in salesDataPerKeyword
                                                                                            group t by new { t.keywordId, t.Country, t.ProductId } into grp
                                                                                            select new DailyKeywordDataOutputForKeywords
                                                                                            {
                                                                                                keywordId = grp.Key.keywordId,
                                                                                                Country = grp.Key.Country,
                                                                                                clicks = grp.Sum(t => t.clicks) != null ? (int)grp.Sum(t => t.clicks) : 0,
                                                                                                cost = grp.Sum(t => t.cost) != null ? (decimal)grp.Sum(t => t.cost) : (decimal)0,
                                                                                                purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                                                                                attributedSalesSameSku14d = grp.Sum(t => t.attributedSalesSameSku14d) != null ? grp.Sum(t => t.attributedSalesSameSku14d) : 0,
                                                                                                ProductId = grp.Key.ProductId
                                                                                            }).FirstOrDefault();

                    //for each sale, note the date in keywordbidtracking and count the number of sales since that date.
                    //If there has been at least three sales and three days since last result, move on to adjust bid.
                    if (keywordOutputGroupedForThisKeyword != null && keywordOutputGroupedForThisKeyword.purchases14d >= 3)
                    {
                        //calculate acos on sales/clicks
                        decimal result1 = await GeneralStaticUtils.SafeDivision((decimal)keywordOutputGroupedForThisKeyword.cost, (decimal)keywordOutputGroupedForThisKeyword.attributedSalesSameSku14d) * 100;
                        decimal result = await GeneralStaticUtils.Round(result1);
                        decimal ACOS = result;

                        //Go down until 1% under desired ACOS or up to at least 10% under desired ACOS
                        PromoNegativeRules rule = promoNegativeRules.Where(x => x.QAPProductID == keywordOutputGroupedForThisKeyword.ProductId).FirstOrDefault();

                        if (rule != null && rule.TargetACOS != null && rule.TargetACOS != 0)
                        {
                            //get current bid

                            List<ProductTargetSnapshot> keywords = new List<ProductTargetSnapshot>();

                            IReadOnlyList<FeedRange> feedRanges4 = await producttargetContainer.GetFeedRangesAsync();
                            QueryDefinition queryDefinition4 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.CountryId = @Country and c.targetId = @keywordId")
                                         .WithParameter("@clientId", reportUser.ClientId.ToString())
                                  .WithParameter("@Country", ky.CountryId)
                                .WithParameter("@keywordId", Convert.ToInt64(ky.keywordId));

                            using (FeedIterator<ProductTargetSnapshot> feedIterator4 = producttargetContainer.GetItemQueryIterator<ProductTargetSnapshot>(
                                  feedRanges4[0],
                                  queryDefinition4,
                                  null,
                                  new QueryRequestOptions() { }))
                            {
                                // Iterate query result pages
                                while (feedIterator4.HasMoreResults)
                                {
                                    FeedResponse<ProductTargetSnapshot> snapshotResponse = await feedIterator4.ReadNextAsync();

                                    // Iterate query results
                                    foreach (var item in snapshotResponse)
                                    {
                                        keywords.Add(item);
                                    }
                                }
                            }

                            decimal newBid = 0;

                            if (ACOS > rule.TargetACOS)
                            {
                                //find current bid and take it down one cent
                                decimal currentBid = keywords[0].bid;
                                if (currentBid != 0)
                                {
                                    newBid = decimal.Subtract(currentBid, (decimal).01);
                                }
                            }
                            else
                            {
                                //if it greater than 10% less, find current bid and take it up one cent
                                decimal TargetAcosMinusTen = decimal.Subtract((decimal)rule.TargetACOS, 10);
                                if (ACOS < TargetAcosMinusTen)
                                {
                                    decimal currentBid = keywords[0].bid;
                                    if (currentBid != 0)
                                    {
                                        newBid = decimal.Add(currentBid, (decimal).01);
                                    }
                                }
                            }

                            if (newBid != 0)
                            {
                                //Send api call to Amazon to update bid
                                string adGroupRequestEndpoint = "/sp/targets";
                                string adGroupRequestMediaType = "application/vnd.spTargetingClause.v3+json";

                                KeywordChangeRequest request = new KeywordChangeRequest();
                                request.Authorization.AccessToken = auth.AccessToken;
                                request.Authorization.TokenExpirationTime = auth.TokenExpirationTime;
                                request.Authorization.ClientProfileCodes = profileCodes;
                                request.keywordId = ky.keywordId;
                                request.state = keywords[0].state;
                                request.bid = newBid;

                              
                                var productUpdated = await updateProductApi.Update(ky.CountryId, request, adGroupRequestEndpoint, adGroupRequestMediaType, auth);


                                if (productUpdated == "1")
                                {

                                    //Update cosmos with our last modified date
                                    ProductTargetSnapshot item = new ProductTargetSnapshot();
                                    item.id = keywords[0].id;
                                    item.partitionKey = keywords[0].partitionKey;

                                    // Read the item to see if it exists
                                    ItemResponse<ProductTargetSnapshot> itemResource = await producttargetContainer.ReadItemAsync<ProductTargetSnapshot>(item.id, new PartitionKey(item.partitionKey));

                                    var itemBody = itemResource.Resource;

                                    itemBody.bid = request.bid;

                                    // replace the item with the updated content
                                    await producttargetContainer.ReplaceItemAsync<ProductTargetSnapshot>(itemBody, itemBody.id, new PartitionKey(itemBody.partitionKey));




                                    //Update cosmos bid history with our last modified date
                                    //make object to save keyword bid change
                                    KeywordBidTracker kyBid = new KeywordBidTracker();
                                    kyBid = ky;
                                    kyBid.LastUpdated = DateTime.UtcNow.Date;
                                    kyBid.id = reportUser.ClientId.ToString() + "." + ky.CountryId.ToString() + "." + ky.keywordId.ToString();
                                    kyBid.partitionKey = reportUser.ClientId.ToString();

                                    await BidTrackingContainer.UpsertItemAsync<KeywordBidTracker>(kyBid, new PartitionKey(item.partitionKey));




                                    //save record to list to update actions taken later
                                    SaveKeywordHistory saveKeywordHistory = new SaveKeywordHistory();
                                    saveKeywordHistory.CountryId = ky.CountryId;
                                    saveKeywordHistory.SearchTerm = keywords[0].resolvedExpression[0].value;
                                    saveKeywordHistory.ProductId = keywordOutputGroupedForThisKeyword.ProductId;
                                    saveKeywordHistory.Action = 3;
                                    saveKeywordHistory.Reason = "Keyword/ASIN bid has been updated from " + keywords[0].bid.ToString() + " to " + newBid.ToString() + " according to ACOS rules.";
                                    saveKeywordHistory.ClientId = reportUser.ClientId;
                                    saveKeywordHistory.DateProcessed = DateTime.Now.Date;
                                    saveKeywordHistories.Add(saveKeywordHistory);
                                }
                            }

                        }
                    }

                }

                //return all actions taken
                return saveKeywordHistories;

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "BidAdjustmentProductTargets - AdjustBid", JsonConvert.SerializeObject(reportUser) + " " + JsonConvert.SerializeObject(promoNegativeRules), reportUser.ClientId);
            }

            return null;
        }
    }
}
