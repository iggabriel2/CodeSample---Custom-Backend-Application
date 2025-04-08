using AdTool.AzSponsoredProducts.AmazonAPI.Keyword;
using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignManagement;
using Azure.Core;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static AdTool.Entities.AzSpApi.CampaignManagement.KeywordResponseByAdGroup;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class BidUpdaterLogic
    {
        public async Task<SimpleResponse> UpdateBids(BidChangeRequest request)
        {
            SimpleResponse simpleResponse = new SimpleResponse();

            try
            {
                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request.Authorization);


                //get profile codes
                RetrieveReportData rrdCodes = new RetrieveReportData();
                request.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(request.Authorization.ClientId);

                //handle if token fails
                if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
                {
                    simpleResponse.APIAuthorization.AccessToken = "";
                    simpleResponse.APIAuthorization.ErrorMessage = "Token Failed";
                    return simpleResponse;
                }
                else
                {
                    simpleResponse.APIAuthorization = auth;
                }

                simpleResponse.APIAuthorization.ErrorMessage = "Failed Ad Groups (empty if none): ";

                request.Authorization.AccessToken = auth.AccessToken;
                request.Authorization.TokenExpirationTime = auth.TokenExpirationTime;

                //get all campaigns by product, country, user, and campaign type
                RetrieveData retrieveData = new RetrieveData();
                List<AllCampaigns> allCampaigns = new List<AllCampaigns>();
                List<AllCampaigns> finalCampaigns = new List<AllCampaigns>();

                //get all campaigns from db if there is not one in the request
                if (request.CampaignId == null || request.CampaignId.Count < 1)
                {
                    allCampaigns = await retrieveData.GetAllCampaigns(request.Authorization.ClientId);

                    var finalCampaignsRaw = allCampaigns.Where(x => x.Status == "ENABLED" && x.QAPProductId == request.ProductId && x.CountryId == request.CountryId.ToString() && x.UsageTypeId == request.CampaignUsageType).ToList();
                    finalCampaigns = finalCampaignsRaw.ToList();

                }
                else
                {
                    foreach(var camp in request.CampaignId)
                    {
                        AllCampaigns a = new AllCampaigns();
                        a.AZCampaignId = camp;
                        finalCampaigns.Add(a);
                    }
               
                }

                //loop through campaigns
                foreach (var campaign in finalCampaigns)
                {
                    //as i loop, get all related ad groups and all keywords in each ad group
                    List<AdGroupSnapshot> adgroupSnapshots = new List<AdGroupSnapshot>();

                    if (string.IsNullOrEmpty(request.AdGroupId))
                    {
                        var adgroupSnapshotsRaw = await GetAdGroupSnapshot(campaign.AZCampaignId, request.Authorization.ClientId.ToString(), request.CountryId);
                        adgroupSnapshots = adgroupSnapshotsRaw.ToList();
                    }
                    else
                    {
                        AdGroupSnapshot ag = new AdGroupSnapshot();
                        ag.adGroupId = Convert.ToInt64(request.AdGroupId);
                        ag.campaignId = Convert.ToInt64(request.CampaignId);
                        adgroupSnapshots.Add(ag);
                    }

                    foreach(var adGroup in adgroupSnapshots)
                    {
                        simpleResponse = await ProcessAdGroup(adGroup, request, simpleResponse, auth);
                    }
                }

                CountrySuccess countrySucces = new CountrySuccess();
                countrySucces.CountryId = request.CountryId;

                if (simpleResponse.APIAuthorization.ErrorMessage == "Failed Ad Groups (empty if none): ")
                {
                    countrySucces.Success = true;
                }
                else
                {
                    countrySucces.Success = false;
                }

                simpleResponse.CountrySuccess.Add(countrySucces);
            }
            catch (Exception ex)
            {
                simpleResponse.APIAuthorization.ErrorMessage = "UpdateBids failed";
            }

            return simpleResponse;
        }

        private async Task<KeywordResponseByAdGroup> GetKeywordsSnapshotHere(string adgroupid, string clientId, int countryId, BidChangeRequest request, APIAuthorization auth)
        {
            KeywordResponseByAdGroup keywordresponseByAdGroup = new KeywordResponseByAdGroup();
            List<ProductTargetSnapshot> productTargetSnapshots = new List<ProductTargetSnapshot>();

            string keywordListRequestEndpoint = "/sp/keywords/list";
            string keywordListRequestMediaType = "application/vnd.spKeyword.v3+json";

            //get keywords
            GetKeywordsForAdGroup getKeywordsForAdGroup = new GetKeywordsForAdGroup();
            KeywordListResponse keywordListResponse = new KeywordListResponse();

            try
            {
                keywordListResponse = await getKeywordsForAdGroup.GetKeywords(countryId, request.Authorization, adgroupid, keywordListRequestEndpoint, keywordListRequestMediaType, auth);
            }
            catch(Exception ex)
            {

            }

            if (keywordListResponse != null && keywordListResponse.keywords != null && keywordListResponse.keywords.Count > 0)
            {
                keywordresponseByAdGroup.KeywordType = "keyword";

                //assign keywordlistresponse to snapshot
                foreach (var keyword in keywordListResponse.keywords)
                {
                    KeywordsWithDataByAdGroup keywordSnapshot = new KeywordsWithDataByAdGroup();

                    keywordSnapshot.KeywordId = keyword.keywordId;
                    keywordSnapshot.AdGroupId = keyword.adGroupId;
                    keywordSnapshot.CampaignId = keyword.campaignId;
                    keywordSnapshot.KeywordText = keyword.keywordText;
                    keywordSnapshot.MatchType = keyword.matchType;
                    keywordSnapshot.State = keyword.state;
                    keywordSnapshot.Bid = Convert.ToDecimal(keyword.bid);
                    keywordSnapshot.CountryId = request.CountryId;
                    keywordSnapshot.KeywordType = "keyword";

                    keywordresponseByAdGroup.keywords.Add(keywordSnapshot);
                }
            }
            else
            {

                //get Cosmos
                Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);

                keywordresponseByAdGroup.KeywordType = "producttarget";

                //get products by ad group
                Container container2 = database.GetContainer(Cosmos.CosmosProductTargets);
                IReadOnlyList<FeedRange> feedRanges2 = await container2.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.adGroupId = @adGroupId")
                          .WithParameter("@clientId", request.Authorization.ClientId.ToString())
                          .WithParameter("@adGroupId", Convert.ToInt64(adgroupid));
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
                        foreach (ProductTargetSnapshot item in snapshotResponse)
                        {
                            productTargetSnapshots.Add(item);
                        }
                    }
                }

                foreach (var pt in productTargetSnapshots)
                {
                    KeywordsWithDataByAdGroup keywordSnapshot = new KeywordsWithDataByAdGroup();

                    keywordSnapshot.KeywordId = pt.targetId.ToString();
                    keywordSnapshot.AdGroupId = pt.adGroupId.ToString();
                    keywordSnapshot.CampaignId = pt.campaignId.ToString();
                    keywordSnapshot.KeywordText = pt.resolvedExpression[0].value;
                    keywordSnapshot.State = pt.state;
                    keywordSnapshot.Bid = Convert.ToDecimal(pt.bid);
                    keywordSnapshot.CountryId = request.CountryId;
                    keywordSnapshot.KeywordType = "producttarget";
                    keywordSnapshot.expressionType = pt.expressionType;

                    foreach (var exp in pt.expression)
                    {
                        ExpressionByAdGroup expression = new ExpressionByAdGroup();
                        expression.value = exp.value;
                        expression.type = exp.type;
                        keywordSnapshot.expression.Add(expression);
                    }

                    keywordresponseByAdGroup.keywords.Add(keywordSnapshot);
                }
            }


            return keywordresponseByAdGroup;
        }

        private async Task<List<AdGroupSnapshot>> GetAdGroupSnapshot(string CampaignId, string ClientId, int CountryId)
        {
            List<AdGroupSnapshot> adGroupSnapshots = new List<AdGroupSnapshot>();

            //get Cosmos
            Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);


            //get products that are active
            Container container2 = database.GetContainer(Cosmos.CosmosAdGroups);
            IReadOnlyList<FeedRange> feedRanges2 = await container2.GetFeedRangesAsync();
            // Distribute feedRanges across multiple compute units and pass each one to a different iterator
            QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.campaignId = @campaignId and c.CountryId = @CountryId")
                      .WithParameter("@clientId", ClientId)
                      .WithParameter("@campaignId", Convert.ToInt64(CampaignId))
                      .WithParameter("@CountryId", CountryId);
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
                    foreach (AdGroupSnapshot item in snapshotResponse)
                    {
                        adGroupSnapshots.Add(item);
                    }
                }
            }

            return adGroupSnapshots;
        }

        private async System.Threading.Tasks.Task<SimpleResponse> ProcessAdGroup(AdGroupSnapshot adGroup, BidChangeRequest request, SimpleResponse simpleResponse, APIAuthorization auth)
        {
            var keywordSnapshots = await GetKeywordsSnapshotHere(adGroup.adGroupId.ToString(), request.Authorization.ClientId.ToString(), request.CountryId, request, auth);

            List<KeywordChangeRequest> keywordChangeRequest = new List<KeywordChangeRequest>();

            if (keywordSnapshots != null)
            {
                foreach (var keyword in keywordSnapshots.keywords)
                {
                    KeywordChangeRequest keywordChange = new KeywordChangeRequest();
                    keywordChange.keywordId = keyword.KeywordId.ToString();

                    if (request.AdjustCurrentBid == "up")
                    {
                        keywordChange.bid = keyword.Bid + request.bid;
                    }
                    else if (request.AdjustCurrentBid == "down")
                    {
                        keywordChange.bid = keyword.Bid - request.bid;
                    }
                    else
                    {
                        keywordChange.bid = request.bid;
                    }

                    keywordChangeRequest.Add(keywordChange);
                }

                if (keywordSnapshots.keywords.Count > 0)
                {
                    if (keywordSnapshots.KeywordType == "keyword")
                    {
                        simpleResponse = await UpdateKeywordsInAdGroup(simpleResponse, adGroup, request, keywordChangeRequest, keywordSnapshots, auth);
                    }
                    else
                    {
                        simpleResponse = await UpdateProductTargetsInAdGroup(simpleResponse, adGroup, request, keywordChangeRequest, keywordSnapshots, auth, adGroup.campaignId.ToString());
                    }
                }
            }
          

            return simpleResponse;
        }

        private async Task<SimpleResponse> UpdateKeywordsInAdGroup(SimpleResponse simpleResponse, AdGroupSnapshot adGroup, BidChangeRequest request, List<KeywordChangeRequest> keywordChangeRequest, KeywordResponseByAdGroup keywordSnapshots, APIAuthorization auth)
        {
            //Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
            //Microsoft.Azure.Cosmos.Container keywordContainer = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosKeywords, "/partitionKey");

            string adGroupRequestEndpoint = "/sp/keywords";
            string adGroupRequestMediaType = "application/vnd.spKeyword.v3+json";

            UpdateMultipleKeywords updateKeywordApi = new UpdateMultipleKeywords();
            var keywordUpdated = await updateKeywordApi.Update(request.CountryId, keywordChangeRequest, request.Authorization, adGroupRequestEndpoint, adGroupRequestMediaType, auth);

            if (keywordUpdated == "1")
            {
                //due to volume of data, we are no longer maintaining all keywords in cosmos
                //foreach (var keyword in keywordSnapshots.keywords)
                //{
                //    try
                //    {
                //        KeywordSnapshot item = new KeywordSnapshot();
                //        item.id = request.Authorization.ClientId.ToString() + "." + request.CountryId.ToString() + "." + keyword.KeywordId.ToString(); ;
                //        item.partitionKey = request.Authorization.ClientId.ToString();

                //        // Read the item to see if it exists
                //        ItemResponse<KeywordSnapshot> itemResource = await keywordContainer.ReadItemAsync<KeywordSnapshot>(item.id, new PartitionKey(item.partitionKey));

                //        if (itemResource != null)
                //        {
                //            var itemBody = itemResource.Resource;

                //            itemBody.bid = request.bid;

                //            // replace the item with the updated content
                //            await keywordContainer.ReplaceItemAsync<KeywordSnapshot>(itemBody, itemBody.id, new PartitionKey(itemBody.partitionKey));
                //        }
                //    }
                //    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                //    {
                //        //nothing to do. Record doesn't exist on Cosmos, so it doesn't need to be updated.
                //    }
                //}
                
            }
            else
            {
                simpleResponse.APIAuthorization.ErrorMessage += " " + adGroup.adGroupId.ToString();
            }

            return simpleResponse;
        }

        private async Task<SimpleResponse> UpdateProductTargetsInAdGroup(SimpleResponse simpleResponse, AdGroupSnapshot adGroup, BidChangeRequest request, List<KeywordChangeRequest> keywordChangeRequest, KeywordResponseByAdGroup keywordSnapshots, APIAuthorization auth, string campaignId)
        {
            Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
            Microsoft.Azure.Cosmos.Container keywordContainer = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosProductTargets, "/partitionKey");

            string adGroupRequestEndpoint = "/sp/targets";
            string adGroupRequestMediaType = "application/vnd.spTargetingClause.v3+json";

            UpdateMultipleProductTargets updateProductTargetApi = new UpdateMultipleProductTargets();
            var productTargetUpdated = await updateProductTargetApi.Update(request.CountryId, keywordChangeRequest, request.Authorization, adGroupRequestEndpoint, adGroupRequestMediaType, auth);

            if (productTargetUpdated == "1")
            {
                foreach (var keyword in keywordSnapshots.keywords)
                {
                    try
                    {
                        ProductTargetSnapshot item = new ProductTargetSnapshot();
                        item.id = request.Authorization.ClientId.ToString() + "." + request.CountryId.ToString() + "." + campaignId + "." + keyword.KeywordId.ToString();
                        item.partitionKey = request.Authorization.ClientId.ToString();

                        // Read the item to see if it exists
                        ItemResponse<ProductTargetSnapshot> itemResource = await keywordContainer.ReadItemAsync<ProductTargetSnapshot>(item.id, new PartitionKey(item.partitionKey));

                        if (itemResource != null)
                        {
                            var itemBody = itemResource.Resource;


                            if (request.AdjustCurrentBid == "up")
                            {
                                itemBody.bid = itemBody.bid + request.bid;
                            }
                            else if (request.AdjustCurrentBid == "down")
                            {
                                itemBody.bid = itemBody.bid - request.bid;
                            }
                            else
                            {
                                itemBody.bid = request.bid;
                            }

                            // replace the item with the updated content
                            await keywordContainer.ReplaceItemAsync<ProductTargetSnapshot>(itemBody, itemBody.id, new PartitionKey(itemBody.partitionKey));
                        }
                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        //nothing to do. Record doesn't exist on Cosmos, so it doesn't need to be updated.
                    }
                }
               
            }
            else
            {
                simpleResponse.APIAuthorization.ErrorMessage += " " + adGroup.adGroupId.ToString();
            }

            return simpleResponse;
        }
    }
}
