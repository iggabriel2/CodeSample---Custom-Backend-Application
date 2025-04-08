using AdTool.AzSponsoredProducts.AmazonAPI.Keyword;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignManagement;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdTool.Entities.AzSpApi.CampaignManagement.KeywordResponseByAdGroup;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class RetrieveAllKeywords
    {
        public async Task<KeywordResponseByAdGroup> GetKeywords(KeywordRequestByAdGroup request)
        {
            //return object
            KeywordResponseByAdGroup keywordresponseByAdGroup = new KeywordResponseByAdGroup();
            keywordresponseByAdGroup.APIAuthorization.ClientId = request.Authorization.ClientId;

            try
            {
                //prep
                List<ProductTargetSnapshot> productTargetSnapshots = new List<ProductTargetSnapshot>();

                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request.Authorization);


                //get profile codes
                RetrieveReportData rrdCodes = new RetrieveReportData();
                request.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(request.Authorization.ClientId);

                //handle if token fails
                if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
                {
                    keywordresponseByAdGroup.APIAuthorization.AccessToken = "";
                    keywordresponseByAdGroup.APIAuthorization.ErrorMessage = "Token Failed";
                    return keywordresponseByAdGroup;
                }
                else
                {
                    keywordresponseByAdGroup.APIAuthorization = auth;
                }

                request.Authorization.AccessToken = auth.AccessToken;
                request.Authorization.TokenExpirationTime = auth.TokenExpirationTime;

                string keywordListRequestEndpoint = "/sp/keywords/list";
                string keywordListRequestMediaType = "application/vnd.spKeyword.v3+json";

                //get keywords
                GetKeywordsForAdGroup getKeywordsForAdGroup = new GetKeywordsForAdGroup();
                KeywordListResponse keywordListResponse = new KeywordListResponse();

                keywordListResponse = await getKeywordsForAdGroup.GetKeywords(request.CountryId, request.Authorization, request.AdGroupId, keywordListRequestEndpoint, keywordListRequestMediaType, auth);



                if (keywordListResponse != null && keywordListResponse.keywords != null && keywordListResponse.keywords.Count > 0) 
                {
                    //assign keywordlistresponse to snapshot
                    foreach(var keyword in keywordListResponse.keywords)
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

                    //get products by ad group
                    Container container2 = database.GetContainer(Cosmos.CosmosProductTargets);
                    IReadOnlyList<FeedRange> feedRanges2 = await container2.GetFeedRangesAsync();
                    // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                    QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.adGroupId = @adGroupId")
                              .WithParameter("@clientId", request.Authorization.ClientId.ToString())
                              .WithParameter("@adGroupId", Convert.ToInt64(request.AdGroupId));
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

                    foreach(var pt in productTargetSnapshots)
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

                //get keywords from monthlysummaryreport
                //RetrieveKeywordManagementData rkd = new RetrieveKeywordManagementData();
                //keywordPerformanceByMonth = await rkd.GetKeywordDataByAdGroup(request.Authorization.ClientId, request.StartDate, request.EndDate, request.AdGroupId);

                List<KeywordPerformanceByMonth> keywordPerformanceByMonth = new List<KeywordPerformanceByMonth>();
                CombineKeywordData combineKeywordData = new CombineKeywordData();
                keywordPerformanceByMonth = await combineKeywordData.GetData(request.StartDate, request.EndDate, request.Authorization.ClientId, "keywordsinadgroup", request.CountryId, request.AdGroupId);



                foreach (var keywordperf in keywordPerformanceByMonth)
                {
                    try
                    {
                        KeywordsWithDataByAdGroup? ksnapshotFound = keywordresponseByAdGroup.keywords.Where(x => x.KeywordId == keywordperf.KeywordId && x.CountryId == keywordperf.Country).FirstOrDefault();


                        ksnapshotFound.Cost = keywordperf.Cost;
                        ksnapshotFound.Impressions = keywordperf.Impressions;
                        ksnapshotFound.Clicks = keywordperf.Clicks;
                        ksnapshotFound.PageReads = keywordperf.KindleEditionNormalizedPagesRead14d;
                        ksnapshotFound.Purchases14d = keywordperf.purchases14d;
                        ksnapshotFound.ConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(keywordperf.purchases14d, keywordperf.Clicks) * 100, 2);
                        ksnapshotFound.CPC = await GeneralStaticUtils.Round(keywordperf.CPC);
                        ksnapshotFound.Sales = keywordperf.AttributedSalesSameSku14d;

                        ksnapshotFound.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(keywordperf.Clicks, keywordperf.Impressions) * 100, 2);

                        if (ksnapshotFound.Sales != 0)
                        {
                            decimal result1 = (keywordperf.Cost / keywordperf.AttributedSalesSameSku14d) * 100;
                            decimal result = await GeneralStaticUtils.Round(result1);
                            ksnapshotFound.ACOS = result;
                        }

                        if (ksnapshotFound.CPC > ksnapshotFound.Cost)
                        {
                            ksnapshotFound.CPC = ksnapshotFound.Cost;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }


            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "RetrieveAllKeywords - GetKeywords", System.Text.Json.JsonSerializer.Serialize(request), request.Authorization.ClientId);
                keywordresponseByAdGroup.APIAuthorization.ErrorMessage = "Failed to get keyword data";
            }

            return keywordresponseByAdGroup;

        }
    }
}
