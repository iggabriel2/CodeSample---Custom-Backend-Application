using AdTool.AzSponsoredProducts.AmazonAPI.Keyword;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignManagement;
using CommandLine;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class RetrievePerfromanceKeywords
    {
        public async Task<KeywordPerformanceResponse> GetKeywords(KeywordPerformanceRequest request)
        {
            CombineKeywordData combineKeywordData = new CombineKeywordData();

            //return object
            KeywordPerformanceResponse keywordPerformanceResponse = new KeywordPerformanceResponse();
            keywordPerformanceResponse.APIAuthorization.ClientId = request.Authorization.ClientId;

            try
            {
                RetrieveData retrieveData = new RetrieveData();
                var allCampaigns = await retrieveData.GetAllCampaignsByCountry(request.Authorization.ClientId, request.CountryId);

                //prep
                KeywordListResponse keywordSnapshots = new KeywordListResponse();
                List<ProductTargetSnapshot> productTargetSnapshots = new List<ProductTargetSnapshot>();
                List<KeywordPerformanceByMonth> keywordPerformanceByMonth = new List<KeywordPerformanceByMonth>();
                List<AdGroupSnapshot> adGroupSnapshots = new List<AdGroupSnapshot>();

                //var task1 = GetKeywordSnapshot(request);
                var task2 = GetProductSnapshot(request);
                var task3 = combineKeywordData.GetData(request.StartDate, request.EndDate, request.Authorization.ClientId, "performance", request.CountryId);
                var task4 = GetAdGroupSnapshot(request);

                await System.Threading.Tasks.Task.WhenAll(task2, task3, task4);
                //keywordSnapshots = await task1;
                productTargetSnapshots = await task2;
                keywordPerformanceByMonth = await task3;
                adGroupSnapshots = await task4;

                List<string> keywordIds = keywordPerformanceByMonth.Where(x => x.KeywordType.ToLower() == "broad" || x.KeywordType.ToLower() == "phrase" || x.KeywordType.ToLower() == "exact").Select(x => x.KeywordId).ToList();
                keywordSnapshots = await GetKeywordSnapshot(request, keywordIds);

                foreach (var keywordperf in keywordPerformanceByMonth)
                {
                    try
                    {
                        //get this campaign name
                        AllCampaigns thisCampaign = allCampaigns.Where(x => x.AZCampaignId == keywordperf.CampaignId.ToString() && x.CountryId == keywordperf.Country.ToString()).FirstOrDefault();

                        if (thisCampaign != null)
                        {

                            KeywordsWithData k = new KeywordsWithData();
                            RelatedKeywordIds relatedk = new RelatedKeywordIds();

                            relatedk.KeywordId = keywordperf.KeywordId;

                            relatedk.Cost = keywordperf.Cost;
                            relatedk.Impressions = keywordperf.Impressions;
                            relatedk.Clicks = keywordperf.Clicks;
                            relatedk.PageReads = keywordperf.KindleEditionNormalizedPagesRead14d;
                            relatedk.Purchases14d = keywordperf.purchases14d;
                            relatedk.Sales = keywordperf.AttributedSalesSameSku14d;


                            Keywordb? ksnapshotFound = keywordSnapshots.keywords.Where(x => x.keywordId.ToString() == keywordperf.KeywordId).FirstOrDefault();
                        
                            if (ksnapshotFound != null)
                            {
                                AdGroupSnapshot? asnapshotFound = adGroupSnapshots.Where(x => x.adGroupId == Convert.ToInt64(keywordperf.AdGroupId) && x.CountryId == keywordperf.Country).FirstOrDefault();


                                relatedk.Bid = Convert.ToDecimal(ksnapshotFound.bid);
                                relatedk.State = ksnapshotFound.state;
                                k.MatchType = ksnapshotFound.matchType;
                                k.KeywordText = ksnapshotFound.keywordText;
                                relatedk.CampaignId = Convert.ToString(ksnapshotFound.campaignId);
                                relatedk.CampaignState = thisCampaign.Status ?? keywordperf.CampaignState;
                                relatedk.CampaignName = thisCampaign.CampaignName ?? keywordperf.CampaignName;
                                relatedk.CPC = await GeneralStaticUtils.Round(keywordperf.CPC);
                                k.ProductId = keywordperf.ProductId;
                                k.ProductName = keywordperf.ProductName;

                                if (relatedk.CPC > relatedk.Cost)
                                {
                                    relatedk.CPC = relatedk.Cost;
                                }

                                relatedk.AdGroupId = Convert.ToString(ksnapshotFound.adGroupId);

                                string repAdGroupName = asnapshotFound.name;

                                if (asnapshotFound.name.Length > 25)
                                {
                                    repAdGroupName = asnapshotFound.name.Substring(0, 7) + "..." + asnapshotFound.name.Substring(asnapshotFound.name.Length - 15, 15);
                                }


                                relatedk.AdGroupName = repAdGroupName;
                                k.KeywordType = "keyword";
                                k.CountryId = request.CountryId;
                                k.CountryName = keywordperf.CountryName;

                            }
                            else
                            {
                                ProductTargetSnapshot? psnapshotFound = productTargetSnapshots.Where(x => x.targetId.ToString() == keywordperf.KeywordId && x.CountryId == keywordperf.Country).FirstOrDefault();
                            
                                if (psnapshotFound != null && psnapshotFound.resolvedExpression != null && psnapshotFound.resolvedExpression.Count > 0 && !string.IsNullOrEmpty(psnapshotFound.resolvedExpression[0].value))
                                {
                                    AdGroupSnapshot? a2snapshotFound = adGroupSnapshots.Where(x => x.adGroupId == Convert.ToInt64(keywordperf.AdGroupId) && x.CountryId == keywordperf.Country).FirstOrDefault();

                                    relatedk.Bid = Convert.ToDecimal(psnapshotFound.bid);
                                    relatedk.State = psnapshotFound.state;
                                    k.KeywordText = psnapshotFound.resolvedExpression[0].value;
                                    relatedk.CampaignId = Convert.ToString(psnapshotFound.campaignId);
                                    relatedk.CampaignState = thisCampaign.Status ?? keywordperf.CampaignState;
                                    relatedk.CPC = await GeneralStaticUtils.Round(keywordperf.CPC);
                                    k.ProductId = keywordperf.ProductId;
                                    k.ProductName = keywordperf.ProductName;

                                    if (relatedk.CPC > relatedk.Cost)
                                    {
                                        relatedk.CPC = relatedk.Cost;
                                    }

                                    k.MatchType = "";
                                    relatedk.CampaignName = thisCampaign.CampaignName ?? keywordperf.CampaignName;
                                    relatedk.AdGroupId = Convert.ToString(psnapshotFound.adGroupId);

                                    string repAdGroupName = a2snapshotFound?.name;

                                    if (a2snapshotFound?.name.Length > 25)
                                    {
                                        repAdGroupName = a2snapshotFound.name.Substring(0, 7) + "..." + a2snapshotFound.name.Substring(a2snapshotFound.name.Length - 15, 15);
                                    }


                                    relatedk.AdGroupName = repAdGroupName;
                                    k.KeywordType = "producttarget";
                                    k.expressionType = psnapshotFound.expressionType;
                                    k.CountryId = psnapshotFound.CountryId;
                                    k.CountryName = keywordperf.CountryName;

                                    foreach (var exp in psnapshotFound.expression)
                                    {
                                        Entities.AzSpApi.CampaignManagement.Expression expression = new Entities.AzSpApi.CampaignManagement.Expression();
                                        expression.value = exp.value;
                                        expression.type = exp.type;
                                        k.expression.Add(expression);
                                    }
                                }

                            }

                            if (!string.IsNullOrEmpty(k.KeywordText))
                            {
                                KeywordsWithData kFound = new KeywordsWithData();
                                kFound = keywordPerformanceResponse.keywords.Where(x => x.KeywordText.ToLower() == k.KeywordText.ToLower() && x.MatchType.ToLower() == k.MatchType.ToLower() && x.ProductId == k.ProductId && x.CountryId == k.CountryId).FirstOrDefault();

                                if (kFound != null)
                                {
                                    kFound.TotalCost = kFound.TotalCost + relatedk.Cost;
                                    kFound.TotalImpressions = kFound.TotalImpressions + relatedk.Impressions;
                                    kFound.TotalClicks = kFound.TotalClicks + relatedk.Clicks;
                                    kFound.TotalPageReads = kFound.TotalPageReads + relatedk.PageReads;
                                    kFound.TotalPurchases14d = kFound.TotalPurchases14d + relatedk.Purchases14d;
                                    kFound.TotalSales = kFound.TotalSales + keywordperf.AttributedSalesSameSku14d;

                                    if (kFound.OverallState.ToLower() != relatedk.State.ToLower())
                                    {
                                        kFound.OverallState = "Mixed";
                                    }

                                    RelatedKeywordIds relatedKsHere = new RelatedKeywordIds();
                                    relatedKsHere = kFound.RelatedKeywordIds.Where(x => x.KeywordId == relatedk.KeywordId).FirstOrDefault();

                                    if (relatedKsHere == null)
                                    {
                                        relatedk.ConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(relatedk.Purchases14d, relatedk.Clicks) * 100, 2);
                                        relatedk.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(relatedk.Clicks, relatedk.Impressions) * 100, 2);

                                        if (relatedk.Sales != 0)
                                        {
                                            decimal result1 = (relatedk.Cost / relatedk.Sales) * 100;
                                            decimal result = await GeneralStaticUtils.Round(result1);
                                            relatedk.ACOS = result;
                                        }
                                        kFound.RelatedKeywordIds.Add(relatedk);
                                    }
                                }
                                else
                                {
                                    k.TotalCost = relatedk.Cost;
                                    k.TotalImpressions = relatedk.Impressions;
                                    k.TotalClicks = relatedk.Clicks;
                                    k.TotalPageReads = relatedk.PageReads;
                                    k.TotalPurchases14d = relatedk.Purchases14d;
                                    k.TotalSales = keywordperf.AttributedSalesSameSku14d;

                                    if (relatedk.State.ToLower() == "enabled")
                                    {
                                        k.OverallState = "Enabled";
                                    }
                                    else
                                    {
                                        k.OverallState = "Paused";
                                    }


                                    relatedk.ConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(relatedk.Purchases14d, relatedk.Clicks) * 100, 2);
                                    relatedk.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(relatedk.Clicks, relatedk.Impressions) * 100, 2);

                                    if (relatedk.Sales != 0)
                                    {
                                        decimal result1 = (relatedk.Cost / relatedk.Sales) * 100;
                                        decimal result = await GeneralStaticUtils.Round(result1);
                                        relatedk.ACOS = result;
                                    }

                                    k.RelatedKeywordIds.Add(relatedk);
                                    keywordPerformanceResponse.keywords.Add(k);
                                }
                           
                            }

                        }
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                
                }

                //calculate cpc and conversion rate
                foreach(var k in keywordPerformanceResponse.keywords)
                {
                    k.TotalCPC = await GeneralStaticUtils.Round(await GeneralStaticUtils.SafeDivision(k.TotalCost, k.TotalClicks));

                    if (k.TotalCPC > k.TotalCost)
                    {
                        k.TotalCPC = k.TotalCost;
                    }

                    k.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(k.TotalClicks, k.TotalImpressions) * 100, 2);

                    if (k.TotalSales != 0)
                    {
                        decimal result1 = (k.TotalCost / k.TotalSales) * 100;
                        decimal result = await GeneralStaticUtils.Round(result1);
                        k.ACOS = result;
                    }

                    k.TotalConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(k.TotalPurchases14d, k.TotalClicks) * 100, 2);
                }


            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "RetrievePerfromanceKeywords - GetKeywords", System.Text.Json.JsonSerializer.Serialize(request), request.Authorization.ClientId);
                keywordPerformanceResponse.APIAuthorization.ErrorMessage = "Failed to get keyword data";
            }

            return keywordPerformanceResponse;

        }

        private async Task<KeywordListResponse> GetKeywordSnapshot(KeywordPerformanceRequest request, List<string> keywordIds)
        {
            //get keywords
            GetKeywordsForAdGroup getKeywordsForAdGroup = new GetKeywordsForAdGroup();
            KeywordListResponse keywordListResponse = new KeywordListResponse();

            string keywordListRequestEndpoint = "/sp/keywords/list";
            string keywordListRequestMediaType = "application/vnd.spKeyword.v3+json";

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request.Authorization);

            RetrieveReportData rrdCodes = new RetrieveReportData();
            request.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(request.Authorization.ClientId);


            var totalKeywordsProcessed = 0;

            while (totalKeywordsProcessed < keywordIds.Count)
            {
                List<string> keywordIdsHere = keywordIds.Skip(totalKeywordsProcessed).Take(900).ToList();

                KeywordListResponse keywordListResponseTemp = new KeywordListResponse();

                keywordListResponseTemp = await getKeywordsForAdGroup.GetKeywords(request.CountryId, request.Authorization, "", keywordListRequestEndpoint, keywordListRequestMediaType, auth, keywordIdsHere, true);

                keywordListResponse.keywords = keywordListResponse.keywords.Union(keywordListResponseTemp.keywords).ToList();

                totalKeywordsProcessed = totalKeywordsProcessed + 900;
            }

            //we no longer store keywords in cosmos
            //List<KeywordSnapshot> keywordSnapshotsReturn = new List<KeywordSnapshot>();
            //ConcurrentBag<KeywordSnapshot> keywordSnapshots = new ConcurrentBag<KeywordSnapshot>();

            ////get Cosmos
            //Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);


            ////get keywords that are active
            //Container container = database.GetContainer(Cosmos.CosmosKeywords);
            //IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
            //// Distribute feedRanges across multiple compute units and pass each one to a different iterator
            //QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.CountryId = @CountryId and c.HasData")
            //          .WithParameter("@clientId", request.Authorization.ClientId.ToString())
            //                                  .WithParameter("@CountryId", request.CountryId);
            //using (FeedIterator<KeywordSnapshot> feedIterator = container.GetItemQueryIterator<KeywordSnapshot>(
            //    feedRanges[0],
            //    queryDefinition,
            //    null,
            //    new QueryRequestOptions() { }))
            //{
            //    // Iterate query result pages
            //    while (feedIterator.HasMoreResults)
            //    {
            //        FeedResponse<KeywordSnapshot> snapshotResponse = await feedIterator.ReadNextAsync();

            //        // Iterate query results
            //        Parallel.ForEach(snapshotResponse, item =>
            //        {
            //            keywordSnapshots.Add(item);
            //        });
            //    }
            //}

            //keywordSnapshotsReturn = keywordSnapshots.ToList();
            return keywordListResponse;
        }

        private async Task<List<ProductTargetSnapshot>> GetProductSnapshot(KeywordPerformanceRequest request)
        {
            ConcurrentBag<ProductTargetSnapshot> productTargetSnapshots = new ConcurrentBag<ProductTargetSnapshot>();
            List<ProductTargetSnapshot> productTargetSnapshotsReturn = new List<ProductTargetSnapshot>();

            //get Cosmos
            Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);


            //get products that are active
            Container container2 = database.GetContainer(Cosmos.CosmosProductTargets);
            IReadOnlyList<FeedRange> feedRanges2 = await container2.GetFeedRangesAsync();
            // Distribute feedRanges across multiple compute units and pass each one to a different iterator
            QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.CountryId = @CountryId and c.HasData")
                      .WithParameter("@clientId", request.Authorization.ClientId.ToString())
                                              .WithParameter("@CountryId", request.CountryId);
            using (FeedIterator<ProductTargetSnapshot> feedIterator2 = container2.GetItemQueryIterator<ProductTargetSnapshot>(
                feedRanges2[0],
                queryDefinition2,
                null,
                new QueryRequestOptions() { }))
            {
                // Iterate query result pages
                while (feedIterator2.HasMoreResults)
                {
                    FeedResponse<ProductTargetSnapshot> snapshotResponse = await feedIterator2.ReadNextAsync();

                    // Iterate query results
                    Parallel.ForEach(snapshotResponse, item =>
                    {
                        productTargetSnapshots.Add(item);
                    });
                }
            }

            productTargetSnapshotsReturn = productTargetSnapshots.ToList();
            return productTargetSnapshotsReturn;
        }

        private async Task<List<AdGroupSnapshot>> GetAdGroupSnapshot(KeywordPerformanceRequest request)
        {
            ConcurrentBag<AdGroupSnapshot> adGroupSnapshots = new ConcurrentBag<AdGroupSnapshot>();
            List<AdGroupSnapshot> adGroupSnapshotsReturn = new List<AdGroupSnapshot>();

            //get Cosmos
            Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);


            //get ad groups that are active
            Container container2 = database.GetContainer(Cosmos.CosmosAdGroups);
            IReadOnlyList<FeedRange> feedRanges2 = await container2.GetFeedRangesAsync();
            // Distribute feedRanges across multiple compute units and pass each one to a different iterator
            QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.CountryId = @CountryId")
                      .WithParameter("@clientId", request.Authorization.ClientId.ToString())
                        .WithParameter("@CountryId", request.CountryId);
            using (FeedIterator<AdGroupSnapshot> feedIterator2 = container2.GetItemQueryIterator<AdGroupSnapshot>(
                feedRanges2[0],
                queryDefinition2,
                null,
                new QueryRequestOptions() { }))
            {
                // Iterate query result pages
                while (feedIterator2.HasMoreResults)
                {
                    FeedResponse<AdGroupSnapshot> snapshotResponse = await feedIterator2.ReadNextAsync();

                    // Iterate query results
                    Parallel.ForEach(snapshotResponse, item =>
                    {
                        adGroupSnapshots.Add(item);
                    });
                }
            }

            adGroupSnapshotsReturn = adGroupSnapshots.ToList();
            return adGroupSnapshotsReturn;
        }
    }
}
