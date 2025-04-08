using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
using AdTool.Entities.CampaignsAdGroups;
using AdTool.Entities.D4Api;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.CampaignsAdGroups
{
    public class GetAdGroupLogic
    {
        public async Task<GetAdGroupsResponseAPI> GetAdGroups(GetAdGroupsRequest request)
        {
            GetAdGroupsResponseAPI response = new GetAdGroupsResponseAPI();

            try
            {
                //List<KeywordPerformanceByMonth> keywordPerformanceByMonths = new List<KeywordPerformanceByMonth>();
                //keywordPerformanceByMonths = await rkd.GetKeywordPerformanceDataByAdGroup(request.Authorization.ClientId, item.adGroupId.ToString(), request.StartDate, request.EndDate);
                //List<AdGroupSnapshotResponse> snapshotResponse = await GetAdGroupsInCosmos(request);

                CombineKeywordData combineKeywordData = new CombineKeywordData();
                var keywordPerformanceByMonthsAsync = combineKeywordData.GetData(request.StartDate, request.EndDate, request.Authorization.ClientId, "adgroupsoncampaigns", request.CountryId, request.CampaignId);
                var snapshotResponseAsync = GetAdGroupsInCosmos(request);

                await System.Threading.Tasks.Task.WhenAll(keywordPerformanceByMonthsAsync, snapshotResponseAsync);

                var keywordPerformanceByMonths = await keywordPerformanceByMonthsAsync;
                var snapshotResponse = await snapshotResponseAsync;

                // Iterate query results
                foreach (AdGroupSnapshotResponse item in snapshotResponse)
                {
                    KeywordPerformanceByMonth keywordByMonth = new KeywordPerformanceByMonth();
                    keywordByMonth = keywordPerformanceByMonths.Where(x => x.AdGroupId == item.adGroupId.ToString() && x.Country == item.CountryId && x.CampaignId == item.campaignId.ToString()).FirstOrDefault();

                    item.state = item.state.ToUpper();

                    if (keywordByMonth == null)
                    {
                        KeywordPerformanceByMonthAdGroup adGroupPerformanceData = new KeywordPerformanceByMonthAdGroup();
                        adGroupPerformanceData.AdGroupId = item.adGroupId.ToString();
                        adGroupPerformanceData.Country = item.CountryId;
                        adGroupPerformanceData.Impressions = 0;
                        adGroupPerformanceData.ACOS = 0;
                        adGroupPerformanceData.CPC = 0;
                        adGroupPerformanceData.Cost = 0;
                        adGroupPerformanceData.ConversionRate = 0;
                        adGroupPerformanceData.Clicks = 0;
                        adGroupPerformanceData.KindleEditionNormalizedPagesRead14d = 0;
                        adGroupPerformanceData.purchases14d = 0;
                        adGroupPerformanceData.Sales = 0;

                        item.PerformanceData = adGroupPerformanceData;
                    }
                    else
                    {

                        KeywordPerformanceByMonthAdGroup adGroupPerformanceData = new KeywordPerformanceByMonthAdGroup();
                        adGroupPerformanceData.AdGroupId = keywordByMonth.AdGroupId;

                        if (keywordByMonth.Country != 0)
                        {
                            adGroupPerformanceData.Country = keywordByMonth.Country;
                        }
                        else
                        {
                            adGroupPerformanceData.Country = item.CountryId;
                        }
                   
                        adGroupPerformanceData.Impressions = keywordByMonth.Impressions;
                        adGroupPerformanceData.Cost = keywordByMonth.Cost;
                        adGroupPerformanceData.CPC = keywordByMonth.CPC;
                        adGroupPerformanceData.ConversionRate = keywordByMonth.ConversionRate;
                        adGroupPerformanceData.Clicks = keywordByMonth.Clicks;
                        adGroupPerformanceData.KindleEditionNormalizedPagesRead14d = keywordByMonth.KindleEditionNormalizedPagesRead14d;
                        adGroupPerformanceData.purchases14d = keywordByMonth.purchases14d;
                        adGroupPerformanceData.Sales = keywordByMonth.AttributedSalesSameSku14d;

                        item.PerformanceData = adGroupPerformanceData;

                        item.PerformanceData.CPC = await GeneralStaticUtils.Round(await GeneralStaticUtils.SafeDivision(item.PerformanceData.Cost, item.PerformanceData.Clicks));

                        if (item.PerformanceData.CPC > item.PerformanceData.Cost)
                        {
                            item.PerformanceData.CPC = item.PerformanceData.Cost;
                        }

                        item.PerformanceData.ConversionRate = Math.Round(await GeneralStaticUtils.SafeDivision(item.PerformanceData.purchases14d, item.PerformanceData.Clicks) * 100, 2);
                        item.PerformanceData.CTR = Math.Round(await GeneralStaticUtils.SafeDivision(item.PerformanceData.Clicks, item.PerformanceData.Impressions) * 100, 2);

                        if (item.PerformanceData.Sales != 0)
                        {
                            decimal result1 = (item.PerformanceData.Cost / item.PerformanceData.Sales) * 100;
                            decimal result = await GeneralStaticUtils.Round(result1);
                            item.PerformanceData.ACOS = result;
                        }
                    }

                    response.AdGroups.Add(item);
                }


                response.APIAuthorization.ClientId = request.Authorization.ClientId;

                return response;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAdGroups", JsonSerializer.Serialize(request), request.Authorization.ClientId);
                response.APIAuthorization.ErrorMessage = "Failed to get ad groups";
            }

            return response;

        }

        public async Task<List<AdGroupSnapshotResponse>> GetAdGroupsInCosmos(GetAdGroupsRequest request)
        {
            List<AdGroupSnapshotResponse> adGroupSnapshotResponse = new List<AdGroupSnapshotResponse>();

            Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);
            Microsoft.Azure.Cosmos.Container container = database.GetContainer(Cosmos.CosmosAdGroups);

            IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
            // Distribute feedRanges across multiple compute units and pass each one to a different iterator
            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c where c.campaignId = @campaignId and c.CountryId = @CountryId and c.partitionKey = @partitionKey")
                      .WithParameter("@partitionKey", request.Authorization.ClientId.ToString())
                      .WithParameter("@CountryId", request.CountryId)
                        .WithParameter("@campaignId", Convert.ToInt64(request.CampaignId));
            using (FeedIterator<AdGroupSnapshotResponse> feedIterator = container.GetItemQueryIterator<AdGroupSnapshotResponse>(
                feedRanges[0],
                queryDefinition,
                null,
                new QueryRequestOptions() { }))
            {
                // Iterate query result pages
                while (feedIterator.HasMoreResults)
                {

                    FeedResponse<AdGroupSnapshotResponse> snapshotResponse = await feedIterator.ReadNextAsync();

                    // Iterate query results
                    foreach(var item in snapshotResponse)
                    {
                        adGroupSnapshotResponse.Add(item);
                    }
                }
            }

            return adGroupSnapshotResponse;
        }
    }
}
