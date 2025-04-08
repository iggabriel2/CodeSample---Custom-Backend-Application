using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSpApi.CampaignManagement;
using Configuration;
using Google.Ads.GoogleAds.V11.Enums;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class RetrievePerformanceSearchTerms
    {
        Regex regexSearch = new Regex(@"^(b[\da-z]{9}|\d{9}(X|\d))$");

        public async Task<SearchTermPerformanceResponse> GetSearchTerms(SearchTermPerformanceRequest request)
        {
            //return object
            SearchTermPerformanceResponse response = new SearchTermPerformanceResponse();
            response.APIAuthorization.ClientId = request.Authorization.ClientId;

            try
            {
                List<AdGroupSnapshot> adGroupSnapshots = new List<AdGroupSnapshot>();

                RetrieveData retrieveData = new RetrieveData();
                var allCampaigns = await retrieveData.GetAllCampaigns(request.Authorization.ClientId);

                //just from db
                //RetrieveKeywordManagementData rkd = new RetrieveKeywordManagementData();
                //searchTermPerformanceByMonth = await rkd.GetSearchTermPerformanceData(request.Authorization.ClientId, request.StartDate, request.EndDate);

                //get keywords from monthlysummaryreport
                CombineSearchTermData combineSearchTermData = new CombineSearchTermData();
                List<SearchTermPerformanceByMonth> searchTermPerformanceByMonth = new List<SearchTermPerformanceByMonth>();
                var task1 = combineSearchTermData.GetData(request.StartDate, request.EndDate, request.Authorization.ClientId, request.CountryId);
                var task2 = GetAdGroupSnapshot(request);

                await System.Threading.Tasks.Task.WhenAll(task1, task2);

                searchTermPerformanceByMonth = await task1;
                adGroupSnapshots = await task2;

        


                foreach (var searchTerm in searchTermPerformanceByMonth)
                {
                    try
                    {
                        SearchTermsWithData s = new SearchTermsWithData();

                        AllCampaigns thisCampaign = allCampaigns.Where(x => x.AZCampaignId == searchTerm.CampaignId.ToString() && x.CountryId == searchTerm.CountryId.ToString()).FirstOrDefault();

                        var searchTermFound = response.SearchTerms.Where(x => x.SearchTerm == searchTerm.SearchTerm && x.CountryId == searchTerm.CountryId && x.ProductId == searchTerm.ProductId).FirstOrDefault();

                        if (thisCampaign != null) {
                            if (searchTermFound != null)
                            {
                                AdGroupSnapshot? asnapshotFound = adGroupSnapshots.Where(x => x.adGroupId == Convert.ToInt64(searchTerm.AdGroup) && x.CountryId == searchTerm.CountryId).FirstOrDefault();

                                searchTermFound.Clicks = searchTermFound.Clicks + searchTerm.Clicks;
                                searchTermFound.Impressions = searchTermFound.Impressions + searchTerm.Impressions;
                                searchTermFound.PageReads = searchTermFound.PageReads + searchTerm.PageReads;
                                searchTermFound.Purchases14d = searchTermFound.Purchases14d + searchTerm.Purchases14d;
                                searchTermFound.Cost = searchTermFound.Cost + searchTerm.Cost;
                                searchTermFound.Sales = searchTermFound.Sales + searchTerm.AttributedSalesSameSku14d;

                                RelatedCampaigns relatedCampaign = new RelatedCampaigns();
                                relatedCampaign.CampaignId = searchTerm.CampaignId;
                                relatedCampaign.UsageTypeId = thisCampaign.UsageTypeId;
                                relatedCampaign.CampaignName = thisCampaign.CampaignName ?? searchTerm.CampaignName;
                                relatedCampaign.CampaignState = thisCampaign.Status ?? searchTerm.CampaignState;
                                relatedCampaign.Cost = searchTerm.Cost;
                                relatedCampaign.Clicks = searchTerm.Clicks;
                                relatedCampaign.Impressions = searchTerm.Impressions;
                                relatedCampaign.PageReads = searchTerm.PageReads;
                                relatedCampaign.Purchases14d = searchTerm.Purchases14d;
                                relatedCampaign.ConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(searchTerm.Purchases14d, searchTerm.Clicks) * 100, 2);
                                relatedCampaign.CPC = await GeneralStaticUtils.Round(searchTerm.CPC);
                                relatedCampaign.KeywordType = searchTerm.KeywordType;
                                relatedCampaign.Keyword = searchTerm.Keyword;
                                relatedCampaign.AdGroup = searchTerm.AdGroup;
                                relatedCampaign.KeywordId = searchTerm.KeywordId;
                                relatedCampaign.Sales = searchTerm.AttributedSalesSameSku14d;

                                relatedCampaign.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(relatedCampaign.Clicks, relatedCampaign.Impressions) * 100, 2);

                                if (relatedCampaign.Sales != 0)
                                {
                                    decimal result1 = (relatedCampaign.Cost / relatedCampaign.Sales) * 100;
                                    decimal result = await GeneralStaticUtils.Round(result1);
                                    relatedCampaign.ACOS = result;
                                }

                                string repAdGroupName = asnapshotFound.name;

                                if (asnapshotFound.name.Length > 25)
                                {
                                    repAdGroupName = asnapshotFound.name.Substring(0, 7) + "..." + asnapshotFound.name.Substring(asnapshotFound.name.Length - 15, 15);
                                }


                                relatedCampaign.AdGroupName = repAdGroupName;

                                if (relatedCampaign.CPC > relatedCampaign.Cost)
                                {
                                    relatedCampaign.CPC = relatedCampaign.Cost;
                                }


                                if (searchTerm.Negative)
                                {
                                    relatedCampaign.Status = "Yes";
                                }
                                else
                                {
                                    relatedCampaign.Status = "No";
                                }


                                if (searchTerm.Reviewed)
                                {
                                    relatedCampaign.Reviewed = "Yes";
                                }
                                else
                                {
                                    relatedCampaign.Reviewed = "No";
                                }

                                ////////only return enabled campaigns
                                if (relatedCampaign.CampaignState.ToLower() == "enabled")
                                {
                                    searchTermFound.RelatedCampaigns.Add(relatedCampaign);
                                }
                                ////////end only return enabled campaigns


                                if (searchTermFound.Status.ToLower() != "mixed")
                                {
                                    if ((searchTerm.Negative && searchTermFound.Status.ToLower() != "yes") || (!searchTerm.Negative && searchTermFound.Status.ToLower() != "no"))
                                    {
                                        searchTermFound.Status = "Mixed";
                                    }
                                }


                                if (searchTermFound.Reviewed.ToLower() != "mixed")
                                {
                                    if ((searchTerm.Reviewed && searchTermFound.Reviewed.ToLower() != "yes") || (!searchTerm.Reviewed && searchTermFound.Reviewed.ToLower() != "no"))
                                    {
                                        searchTermFound.Reviewed = "Mixed";
                                    }
                                }

                            }
                            else
                            {
                                AdGroupSnapshot? asnapshotFound = adGroupSnapshots.Where(x => x.adGroupId == Convert.ToInt64(searchTerm.AdGroup) && x.CountryId == searchTerm.CountryId).FirstOrDefault();

                                s.SearchTerm = searchTerm.SearchTerm;
                                s.Cost = searchTerm.Cost;
                                s.Clicks = searchTerm.Clicks;
                                s.Impressions = searchTerm.Impressions;
                                s.PageReads = searchTerm.PageReads;
                                s.Purchases14d = searchTerm.Purchases14d;
                                s.CountryId = searchTerm.CountryId;
                                s.ProductId = searchTerm.ProductId;
                                s.ProductName = searchTerm.ProductName;
                                s.CountryName = searchTerm.CountryName;
                                s.Sales = searchTerm.AttributedSalesSameSku14d;

                                if (searchTerm.Negative)
                                {
                                    s.Status = "Yes";
                                }
                                else
                                {
                                    s.Status = "No";
                                }

                                if (searchTerm.Reviewed)
                                {
                                    s.Reviewed = "Yes";
                                }
                                else
                                {
                                    s.Reviewed = "No";
                                }

                                //figure out what kind of search term it is
                                bool targetingExpression = false;
                                bool complement = false;
                                bool asinLength = false;

                                if (searchTerm.KeywordType.ToUpper() == "TARGETING_EXPRESSION")
                                    targetingExpression = true;

                                if (searchTerm.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && searchTerm.Keyword.ToLower() == "complements")
                                    complement = true;

                                if (searchTerm.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && searchTerm.SearchTerm.Length == 10 && !searchTerm.SearchTerm.Contains(" "))
                                    asinLength = true;

                                if ((targetingExpression) || (complement) || (asinLength && regexSearch.Match(searchTerm.SearchTerm).Success))
                                {
                                    s.SimpleKeywordType = "producttarget";
                                }
                                else
                                {
                                    s.SimpleKeywordType = "keyword";
                                }

                                RelatedCampaigns relatedCampaign = new RelatedCampaigns();
                                relatedCampaign.CampaignId = searchTerm.CampaignId;
                                relatedCampaign.UsageTypeId = thisCampaign.UsageTypeId;
                                relatedCampaign.CampaignName = thisCampaign.CampaignName ?? searchTerm.CampaignName;
                                relatedCampaign.CampaignState = thisCampaign.Status ?? searchTerm.CampaignState;
                                relatedCampaign.Cost = searchTerm.Cost;
                                relatedCampaign.Clicks = searchTerm.Clicks;
                                relatedCampaign.Impressions = searchTerm.Impressions;
                                relatedCampaign.PageReads = searchTerm.PageReads;
                                relatedCampaign.Purchases14d = searchTerm.Purchases14d;
                                relatedCampaign.ConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(searchTerm.Purchases14d, searchTerm.Clicks) * 100, 2);
                                relatedCampaign.CPC = await GeneralStaticUtils.Round(searchTerm.CPC);
                                relatedCampaign.KeywordType = searchTerm.KeywordType;
                                relatedCampaign.Keyword = searchTerm.Keyword;
                                relatedCampaign.AdGroup = searchTerm.AdGroup;
                                relatedCampaign.KeywordId = searchTerm.KeywordId;
                                relatedCampaign.Sales = searchTerm.AttributedSalesSameSku14d;

                                relatedCampaign.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(relatedCampaign.Clicks, relatedCampaign.Impressions) * 100, 2);

                                if (relatedCampaign.Sales != 0)
                                {
                                    decimal result1 = (relatedCampaign.Cost / relatedCampaign.Sales) * 100;
                                    decimal result = await GeneralStaticUtils.Round(result1);
                                    relatedCampaign.ACOS = result;
                                }

                                string repAdGroupName = asnapshotFound.name;

                                if (asnapshotFound.name.Length > 25)
                                {
                                    repAdGroupName = asnapshotFound.name.Substring(0, 7) + "..." + asnapshotFound.name.Substring(asnapshotFound.name.Length - 15, 15);
                                }


                                relatedCampaign.AdGroupName = repAdGroupName;

                                if (relatedCampaign.CPC > relatedCampaign.Cost)
                                {
                                    relatedCampaign.CPC = relatedCampaign.Cost;
                                }


                                if (searchTerm.Negative)
                                {
                                    relatedCampaign.Status = "Yes";
                                }
                                else
                                {
                                    relatedCampaign.Status = "No";
                                }

                                if (searchTerm.Reviewed)
                                {
                                    relatedCampaign.Reviewed = "Yes";
                                }
                                else
                                {
                                    relatedCampaign.Reviewed = "No";
                                }


                                ////////only return enabled campaigns
                                if (relatedCampaign.CampaignState.ToLower() == "enabled")
                                {
                                    s.RelatedCampaigns.Add(relatedCampaign);
                                }

                                if (s.RelatedCampaigns.Count > 0)
                                {
                                    response.SearchTerms.Add(s);
                                }
                                ////////end only return enabled campaigns

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }

                //calculate cpc and conversion rate
                foreach (var k in response.SearchTerms)
                {
                    k.CPC = await GeneralStaticUtils.Round(await GeneralStaticUtils.SafeDivision(k.Cost, k.Clicks));

                    if (k.CPC > k.Cost)
                    {
                        k.CPC = k.Cost;
                    }

                    k.ConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(k.Purchases14d, k.Clicks) * 100, 2);
                    k.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(k.Clicks, k.Impressions) * 100, 2);

                    if (k.Sales != 0)
                    {
                        decimal result1 = (k.Cost / k.Sales) * 100;
                        decimal result = await GeneralStaticUtils.Round(result1);
                        k.ACOS = result;
                    }
                }

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "RetrievePerformanceSearchTerms - GetSearchTerms", System.Text.Json.JsonSerializer.Serialize(request), request.Authorization.ClientId);
                response.APIAuthorization.ErrorMessage = "Failed to get search term data";
            }

            return response;

        }

        private async Task<List<AdGroupSnapshot>> GetAdGroupSnapshot(SearchTermPerformanceRequest request)
        {
            ConcurrentBag<AdGroupSnapshot> adGroupSnapshots = new ConcurrentBag<AdGroupSnapshot>();
            List<AdGroupSnapshot> adGroupSnapshotsResponse = new List<AdGroupSnapshot>();

            //get Cosmos
            Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);


            //get products that are active
            Container container2 = database.GetContainer(Cosmos.CosmosAdGroups);
            IReadOnlyList<FeedRange> feedRanges2 = await container2.GetFeedRangesAsync();
            // Distribute feedRanges across multiple compute units and pass each one to a different iterator
            QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId")
                      .WithParameter("@clientId", request.Authorization.ClientId.ToString());
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
                    foreach (var item in snapshotResponse)
                    {
                        adGroupSnapshots.Add(item);
                    }
                }
            }

            adGroupSnapshotsResponse = adGroupSnapshots.ToList();
            return adGroupSnapshotsResponse;
        }
    }
}
