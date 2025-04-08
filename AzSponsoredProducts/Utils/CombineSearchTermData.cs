using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class CombineSearchTermData
    {
        public async Task<List<SearchTermPerformanceByMonth>> GetData(DateTime? startDateRaw, DateTime? endDateRaw, Guid ClientId, int CountryId)
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            //set default values
            if (startDateRaw == null)
            {
                startDate = DateTime.Now.AddYears(-100).Date;
            }
            else
            {
                startDate = startDateRaw.GetValueOrDefault();
            }
              

            if (endDateRaw == null)
            {
                endDate = DateTime.Now.AddYears(100).Date;
            }
            else
            {
                endDate = endDateRaw.GetValueOrDefault();
            }
     
            try
            {
                List<SearchTermPerformanceByMonth> returnValue = new List<SearchTermPerformanceByMonth>();
                List<SearchTermPerformanceByMonth> returnValueCombined = new List<SearchTermPerformanceByMonth>();

                if (endDate >= DateTime.Now.AddDays(-1) && startDate.Day == 1)
                {
                    //return monthly data only
                    returnValue = await GetMonthlyData(CountryId, startDate, endDate, ClientId);
                }
                else if (startDate.Day == 1 && endDate.Day == DateTime.DaysInMonth(endDate.Year, endDate.Month))
                {
                    //return monthly data only
                    returnValue = await GetMonthlyData(CountryId, startDate, endDate, ClientId);
                }
                else if ((endDate - startDate).TotalDays < 95)
                {
                    //return daily data only
                    returnValue = await GetDailyData(startDate, endDate, ClientId, CountryId);
                }
                else
                {
                    //get end daily data dates
                    DateTime endDailyEndDate = endDate;
                    DateTime endDailyStartDate = new DateTime(endDate.Year, endDate.Month, 1);

                    //get middle monthly data dates - we can use the first for the day in both cases becuase this data is already consolidated by month
                    DateTime middleMonthlyEndDateTemp = new DateTime(endDate.Year, endDate.Month, 1);
                    DateTime middleMonthlyEndDate = middleMonthlyEndDateTemp.AddDays(-1);

                    DateTime workingStartDate = startDate.AddMonths(1);
                    DateTime middleMonthStartDate = new DateTime(workingStartDate.Year, workingStartDate.Month, 1);

                    //get beginning daily data dates
                    DateTime beginDailyEndDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                    DateTime beginDailyStartDate = startDate;

                    //query all
                    var endData = GetDailyData(endDailyStartDate, endDailyEndDate, ClientId, CountryId);
                    var middleData = GetMonthlyData(CountryId, middleMonthStartDate, middleMonthlyEndDate, ClientId);
                    var beginningData = GetDailyData(beginDailyStartDate, beginDailyEndDate, ClientId, CountryId);

                    await System.Threading.Tasks.Task.WhenAll(endData, middleData, beginningData);

                    var returedEndData = await endData;
                    var returnedMiddleData = await middleData;
                    var returnedStartData = await beginningData;

                    returnValue.AddRange(returedEndData);
                    returnValue.AddRange(returnedMiddleData);
                    returnValue.AddRange(returnedStartData);
                }



                returnValueCombined = (from t in returnValue
                                       group t by new { t.CampaignId, t.CountryId, t.AdGroup, t.KeywordId, t.KeywordType, t.SearchTerm, t.Keyword, t.ProductId, t.ProductName, t.CountryName, t.CampaignName, t.CampaignState, t.Negative } into grp
                            select new SearchTermPerformanceByMonth
                            {
                                Negative = grp.Key.Negative,
                                CampaignName = grp.Key.CampaignName,
                                CampaignState = grp.Key.CampaignState,
                                ProductId = grp.Key.ProductId,
                                ProductName = grp.Key.ProductName,
                                CountryName = grp.Key.CountryName,
                                Keyword = grp.Key.Keyword,
                                SearchTerm = grp.Key.SearchTerm,
                                KeywordId = grp.Key.KeywordId,
                                KeywordType = grp.Key.KeywordType,
                                CampaignId = grp.Key.CampaignId,
                                CountryId = grp.Key.CountryId,
                                AdGroup = grp.Key.AdGroup,
                                Clicks = grp.Sum(t => t.Clicks) != null ? (int)grp.Sum(t => t.Clicks) : 0,
                                Cost = grp.Sum(t => t.Cost) != null ? (decimal)grp.Sum(t => t.Cost) : (decimal)0,
                                Impressions = grp.Sum(t => t.Impressions) != null ? (int)grp.Sum(t => t.Impressions) : 0,
                                PageReads = grp.Sum(t => t.PageReads) != null ? (int)grp.Sum(t => t.PageReads) : 0,
                                Purchases14d = grp.Sum(t => t.Purchases14d) != null ? (int)grp.Sum(t => t.Purchases14d) : 0,
                                CPC = grp.Sum(t => t.Clicks) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Cost) != null ? grp.Sum(t => t.Cost) / grp.Sum(t => t.Clicks) : 0,
                                ConversionRate = grp.Sum(t => t.Purchases14d) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Clicks) != null ? (grp.Sum(t => t.Purchases14d) / grp.Sum(t => t.Clicks)) * 100 : 0,
                                AttributedSalesSameSku14d = grp.Sum(t => t.AttributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.AttributedSalesSameSku14d) : 0
                            }).ToList();

                List<KeywordNegativesForCosmos> keywordNegatives = new List<KeywordNegativesForCosmos>();

                RetrieveNightlyExtras rnd = new RetrieveNightlyExtras();
                keywordNegatives = await rnd.GetKeywordNegativesForCosmosAll(ClientId);

                foreach(var negativeKeyword in keywordNegatives)
                {
                    returnValueCombined.Where(x => x.KeywordId == negativeKeyword.KeywordId && x.SearchTerm == negativeKeyword.SearchTerm && negativeKeyword.Negative == true && x.CountryId == negativeKeyword.CountryId).ToList().ForEach(s => s.Negative = true);
                    returnValueCombined.Where(x => x.KeywordId == negativeKeyword.KeywordId && x.SearchTerm == negativeKeyword.SearchTerm && negativeKeyword.Reviewed == true && x.CountryId == negativeKeyword.CountryId).ToList().ForEach(s => s.Reviewed = true);
                }

                return returnValueCombined;
            }
            catch(Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "CombineSearchTermData - GetData", "ClientId: " + ClientId.ToString() + " StartDate: " + startDate.ToString() + " EndDate: " + endDate.ToString());
                return null;
            }
        }

        public async Task<List<SearchTermPerformanceByMonth>> GetDailyData(DateTime startDate, DateTime endDate, Guid ClientId, int CountryId)
        {
            //make cosmos client
            Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);
            Microsoft.Azure.Cosmos.Container container = database.GetContainer(Cosmos.CosmosSearchTermsDataContainer);

            List<SearchTermPerformanceByMonth> response = new List<SearchTermPerformanceByMonth>();

            //set object to hold cosmos response
            ConcurrentBag<DailyKeywordDataOutput> cb = new ConcurrentBag<DailyKeywordDataOutput>();

            IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
            // Distribute feedRanges across multiple compute units and pass each one to a different iterator

            QueryDefinition queryDefinition = null;

            if (CountryId != 0)
            {
                queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate  and c.dateRecord <= @endDAte and c.Country = @Country")
              .WithParameter("@clientId", ClientId.ToString())
                .WithParameter("@startDate", startDate.ToUniversalTime())
                    .WithParameter("@endDAte", endDate.ToUniversalTime())
                 .WithParameter("@Country", CountryId);
            }
            else
            {
                queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate  and c.dateRecord <= @endDAte")
              .WithParameter("@clientId", ClientId.ToString())
                .WithParameter("@startDate", startDate.ToUniversalTime())
                    .WithParameter("@endDAte", endDate.ToUniversalTime());
            }

            using (FeedIterator<DailyKeywordDataOutput> feedIterator = container.GetItemQueryIterator<DailyKeywordDataOutput>(
                feedRanges[0],
                queryDefinition,
                null,
                new QueryRequestOptions() { }))
            {
                // Iterate query result pages
                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<DailyKeywordDataOutput> snapshotResponse = await feedIterator.ReadNextAsync();

                    // Iterate query results
                    Parallel.ForEach(snapshotResponse, item =>
                    {
                        cb.Add(item);
                    });
                }
            }



            response = (from t in cb
                        group t by new { t.campaignId, t.Country, t.adGroupId, t.keywordId, t.keywordType, t.searchTerm, t.keyword, t.ProductId, t.ProductName, t.CountryName, t.campaignName, t.campaignStatus, t.Negative } into grp
                        select new SearchTermPerformanceByMonth
                        {
                            Negative = grp.Key.Negative,
                            CampaignName = grp.Key.campaignName,
                            CampaignState = grp.Key.campaignStatus,
                            ProductId = grp.Key.ProductId,
                            ProductName = grp.Key.ProductName,
                            CountryName = grp.Key.CountryName,
                            Keyword = grp.Key.keyword,
                            SearchTerm = grp.Key.searchTerm,
                            KeywordId = grp.Key.keywordId,
                            KeywordType = grp.Key.keywordType,
                            CampaignId = grp.Key.campaignId,
                            CountryId = grp.Key.Country,
                            AdGroup = grp.Key.adGroupId,
                            Clicks = grp.Sum(t => t.clicks) != null ? (int)grp.Sum(t => t.clicks) : 0,
                            Cost = grp.Sum(t => t.cost) != null ? (decimal)grp.Sum(t => t.cost) : (decimal)0,
                            Impressions = grp.Sum(t => t.impressions) != null ? (int)grp.Sum(t => t.impressions) : 0,
                            PageReads = grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) : 0,
                            Purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                            AttributedSalesSameSku14d = grp.Sum(t => t.attributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.attributedSalesSameSku14d) : 0,
                        }).ToList();




            return response.ToList();
        }

        public async Task<List<SearchTermPerformanceByMonth>> GetMonthlyData(int CountryId, DateTime startDate, DateTime endDate, Guid ClientId)
        {
            RetrieveKeywordManagementData rkd = new RetrieveKeywordManagementData();
            List<SearchTermPerformanceByMonth> searchTermPerformanceByMonth = new List<SearchTermPerformanceByMonth>();
            if (CountryId != 0)
            {
                searchTermPerformanceByMonth = await rkd.GetSearchTermPerformanceDataByCountry(ClientId, CountryId, startDate, endDate);
            }
            else
            {
                searchTermPerformanceByMonth = await rkd.GetSearchTermPerformanceData(ClientId, startDate, endDate);
            }
        
            return searchTermPerformanceByMonth;
        }
    }
}
