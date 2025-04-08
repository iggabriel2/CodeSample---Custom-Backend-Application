using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using Configuration;
using Google.Api.Gax.ResourceNames;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class CombineKeywordData
    {
        //private Database database;
        //private Microsoft.Azure.Cosmos.Container container;

        public CombineKeywordData()
        {
            //this.database = _database;
        }

        public async Task<List<KeywordPerformanceByMonth>> GetData(DateTime? startDateRaw, DateTime? endDateRaw, Guid ClientId, string TypeToGet, int CountryId = 0, string ContainerTypeId = "")
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

            //make cosmos client
            //this.container = database.GetContainer(Cosmos.CosmosKeywordDataContainer);

            //this.container = await database.GetContainerQueryIterator(Cosmos.CosmosKeywordDataContainer, "/partitionKey");

            try
            {
                List<KeywordPerformanceByMonth> returnValue = new List<KeywordPerformanceByMonth>();
                List<KeywordPerformanceByMonth> returnValueCombined = new List<KeywordPerformanceByMonth>();

                if (endDate >= DateTime.Now.AddDays(-1) && startDate.Day == 1)
                {
                    //return monthly data only
                    returnValue = await GetMonthlyData(startDate, endDate, ClientId, TypeToGet, ContainerTypeId, CountryId);
                }
                else if (startDate.Day == 1 && endDate.Day == DateTime.DaysInMonth(endDate.Year, endDate.Month))
                {
                    //return monthly data only
                    returnValue = await GetMonthlyData(startDate, endDate, ClientId, TypeToGet, ContainerTypeId, CountryId);
                }
                else if ((endDate - startDate).TotalDays < 95)
                {
                    //return daily data only
                    returnValue = await GetDailyData(startDate, endDate, ClientId, TypeToGet, ContainerTypeId, CountryId);
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
                    DateTime middleMonthStartDate = new DateTime(workingStartDate.Year, workingStartDate.Month, 1);;

                    //get beginning daily data dates
                    DateTime beginDailyEndDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                    DateTime beginDailyStartDate = startDate;

                    //query all
                    var endData = GetDailyData(endDailyStartDate, endDailyEndDate, ClientId, TypeToGet, ContainerTypeId, CountryId);
                    var middleData = GetMonthlyData(middleMonthStartDate, middleMonthlyEndDate, ClientId, TypeToGet, ContainerTypeId, CountryId);
                    var beginningData = GetDailyData(beginDailyStartDate, beginDailyEndDate, ClientId, TypeToGet, ContainerTypeId, CountryId);

                    await System.Threading.Tasks.Task.WhenAll(endData, middleData, beginningData);

                    var returedEndData = await endData;
                    var returnedMiddleData = await middleData;
                    var returnedStartData = await beginningData;

                    returnValue.AddRange(returedEndData);
                    returnValue.AddRange(returnedMiddleData);
                    returnValue.AddRange(returnedStartData);


                }

                //this is modeled on keyword performance. do not remove anything.
                if (TypeToGet == "performance")
                {
                    List<KeywordPerformanceByMonth> returnedValueRaw = new List<KeywordPerformanceByMonth>();
                    returnedValueRaw = (from t in returnValue
                                           group t by new { t.KeywordId, t.CampaignId, t.ProductId, t.ProductName, t.AdGroupId, t.KeywordType, t.CountryName, t.Country, t.CampaignName, t.CampaignState, t.UsageType, t.QAPCampaignId } into grp
                                           select new KeywordPerformanceByMonth
                                           {
                                               CampaignName = grp.Key.CampaignName,
                                               CampaignState = grp.Key.CampaignState,
                                               ProductId = grp.Key.ProductId,
                                               ProductName = grp.Key.ProductName,
                                               CountryName = grp.Key.CountryName,
                                               KeywordId = grp.Key.KeywordId,
                                               KeywordType = grp.Key.KeywordType,
                                               CampaignId = grp.Key.CampaignId,
                                               Country = grp.Key.Country,
                                               AdGroupId = grp.Key.AdGroupId,
                                               UsageType = grp.Key.UsageType,
                                               QAPCampaignId = grp.Key.QAPCampaignId,
                                               AttributedSalesSameSku14d = grp.Sum(t => t.AttributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.AttributedSalesSameSku14d) : 0,
                                               Clicks = grp.Sum(t => t.Clicks) != null ? (int)grp.Sum(t => t.Clicks) : 0,
                                               Cost = grp.Sum(t => t.Cost) != null ? (decimal)grp.Sum(t => t.Cost) : (decimal)0,
                                               Impressions = grp.Sum(t => t.Impressions) != null ? (int)grp.Sum(t => t.Impressions) : 0,
                                               KindleEditionNormalizedPagesRead14d = grp.Sum(t => t.KindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.KindleEditionNormalizedPagesRead14d) : 0,
                                               purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                               CPC = grp.Sum(t => t.Clicks) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Cost) != null ? grp.Sum(t => t.Cost) / grp.Sum(t => t.Clicks) : 0,
                                               ConversionRate = grp.Sum(t => t.purchases14d) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Clicks) != null ? (grp.Sum(t => t.purchases14d) / grp.Sum(t => t.Clicks)) * 100 : 0,
                                           }).ToList();

                    returnValueCombined = returnedValueRaw.Where(x => x.Clicks > 0 || x.KindleEditionNormalizedPagesRead14d > 0 || x.purchases14d > 0).ToList();
                }
                else
                {
                    returnValueCombined = (from t in returnValue
                                           group t by new { t.KeywordId, t.CampaignId, t.ProductId, t.ProductName, t.AdGroupId, t.KeywordType, t.CountryName, t.Country, t.CampaignName, t.CampaignState, t.UsageType, t.QAPCampaignId } into grp
                                           select new KeywordPerformanceByMonth
                                           {
                                               CampaignName = grp.Key.CampaignName,
                                               CampaignState = grp.Key.CampaignState,
                                               ProductId = grp.Key.ProductId,
                                               ProductName = grp.Key.ProductName,
                                               CountryName = grp.Key.CountryName,
                                               KeywordId = grp.Key.KeywordId,
                                               KeywordType = grp.Key.KeywordType,
                                               CampaignId = grp.Key.CampaignId,
                                               Country = grp.Key.Country,
                                               AdGroupId = grp.Key.AdGroupId,
                                               UsageType = grp.Key.UsageType,
                                               QAPCampaignId = grp.Key.QAPCampaignId,
                                               AttributedSalesSameSku14d = grp.Sum(t => t.AttributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.AttributedSalesSameSku14d) : 0,
                                               Clicks = grp.Sum(t => t.Clicks) != null ? (int)grp.Sum(t => t.Clicks) : 0,
                                               Cost = grp.Sum(t => t.Cost) != null ? (decimal)grp.Sum(t => t.Cost) : (decimal)0,
                                               Impressions = grp.Sum(t => t.Impressions) != null ? (int)grp.Sum(t => t.Impressions) : 0,
                                               KindleEditionNormalizedPagesRead14d = grp.Sum(t => t.KindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.KindleEditionNormalizedPagesRead14d) : 0,
                                               purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                               CPC = grp.Sum(t => t.Clicks) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Cost) != null ? grp.Sum(t => t.Cost) / grp.Sum(t => t.Clicks) : 0,
                                               ConversionRate = grp.Sum(t => t.purchases14d) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Clicks) != null ? (grp.Sum(t => t.purchases14d) / grp.Sum(t => t.Clicks)) * 100 : 0,
                                           }).ToList();
                }
            
              

                return returnValueCombined;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "CombineKeywordData - GetData", "ClientId: " + ClientId.ToString() + " StartDate: " + startDate.ToString() + " EndDate: " + endDate.ToString() + " TypeToGet: " + TypeToGet + " ContainerTypeId: " + ContainerTypeId);
                return null;
            }
         
        }

        public async Task<List<KeywordPerformanceByMonth>> GetDailyData(DateTime startDate, DateTime endDate, Guid ClientId, string TypeToGet, string ContainerTypeId = "", int CountryId = 0)
        {
            Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);
            Microsoft.Azure.Cosmos.Container container = database.GetContainer(Cosmos.CosmosKeywordDataContainer);
            Microsoft.Azure.Cosmos.Container campaignContainer = database.GetContainer(Cosmos.CosmosCampaignDataContainer);

            List<KeywordPerformanceByMonth> response = new List<KeywordPerformanceByMonth>();

            //set object to hold cosmos response
            ConcurrentBag<DailyKeywordDataOutput> cb = new ConcurrentBag<DailyKeywordDataOutput>();
            ConcurrentBag<DailyCampaignData> cbCampaigns = new ConcurrentBag<DailyCampaignData>();

            if (TypeToGet.ToLower() == "allcampaigns")
            {
                IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                QueryDefinition queryDefinition = null;

                if (CountryId != 0)
                {
                    queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate and c.Country = @Country and c.dateRecord <= @endDAte and (c.impressions > 0 or c.kindleEditionNormalizedPagesRead14d > 0 or c.clicks > 0 or c.unitsSoldClicks14d > 0)")
                   .WithParameter("@clientId", ClientId.ToString())
                     .WithParameter("@startDate", startDate.ToUniversalTime())
                         .WithParameter("@endDAte", endDate.ToUniversalTime())
                     .WithParameter("@Country", CountryId);
                }
                else
                {
                    queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate  and c.dateRecord <= @endDAte and (c.impressions > 0 or c.kindleEditionNormalizedPagesRead14d > 0 or c.clicks > 0 or c.unitsSoldClicks14d > 0)")
                         .WithParameter("@clientId", ClientId.ToString())
                           .WithParameter("@startDate", startDate.ToUniversalTime())
                               .WithParameter("@endDAte", endDate.ToUniversalTime());
                }


                using (FeedIterator<DailyCampaignData> feedIterator = campaignContainer.GetItemQueryIterator<DailyCampaignData>(
                    feedRanges[0],
                    queryDefinition,
                    null,
                    new QueryRequestOptions() { }))
                {
                    // Iterate query result pages
                    while (feedIterator.HasMoreResults)
                    {
                        FeedResponse<DailyCampaignData> snapshotResponse = await feedIterator.ReadNextAsync();

                        // Iterate query results
                        Parallel.ForEach(snapshotResponse, item =>
                        {
                            cbCampaigns.Add(item);
                        });
                    }
                }


                response = (from t in cbCampaigns
                            group t by new { t.campaignId, t.ProductId, t.ProductName, t.CountryName, t.Country, t.campaignName, t.campaignStatus, t.UsageType, t.QAPCampaignId } into grp
                                       select new KeywordPerformanceByMonth
                                       {
                                           CampaignName = grp.Key.campaignName,
                                           CampaignState = grp.Key.campaignStatus.ToUpper(),
                                           ProductId = grp.Key.ProductId,
                                           ProductName = grp.Key.ProductName,
                                           CountryName = grp.Key.CountryName,
                                           UsageType = grp.Key.UsageType,
                                           QAPCampaignId = grp.Key.QAPCampaignId != null ? (int)grp.Key.QAPCampaignId : 0,
                                           CampaignId = grp.Key.campaignId,
                                           Country = grp.Key.Country,
                                           Clicks = grp.Sum(t => t.clicks) != null ? (int)grp.Sum(t => t.clicks) : 0,
                                           Cost = grp.Sum(t => t.cost) != null ? (decimal)grp.Sum(t => t.cost) : (decimal)0,
                                           Impressions = grp.Sum(t => t.impressions) != null ? (int)grp.Sum(t => t.impressions) : 0,
                                           KindleEditionNormalizedPagesRead14d = grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) : 0,
                                           purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                           AttributedSalesSameSku14d = grp.Sum(t => t.attributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.attributedSalesSameSku14d) : 0,
                                       }).ToList();


              






            }

            //DONE

            else if (TypeToGet.ToLower() == "adgroupsoncampaigns")
            {
                IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate  and c.dateRecord <= @endDAte and c.campaignId = @campaignId and c.Country = @Country and (c.impressions > 0 or c.kindleEditionNormalizedPagesRead14d > 0 or c.clicks > 0 or c.unitsSoldClicks14d > 0)")
                  .WithParameter("@clientId", ClientId.ToString())
                    .WithParameter("@startDate", startDate.ToUniversalTime())
                        .WithParameter("@endDAte", endDate.ToUniversalTime())
                        .WithParameter("@campaignId", ContainerTypeId)
                                .WithParameter("@Country", CountryId);
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
                            group t by new { t.campaignId, t.adGroupId, t.Country } into grp
                            select new KeywordPerformanceByMonth
                            {
                                CampaignId = grp.Key.campaignId,
                                Country = grp.Key.Country,
                                AdGroupId = grp.Key.adGroupId,
                                Clicks = grp.Sum(t => t.clicks) != null ? (int)grp.Sum(t => t.clicks) : 0,
                                Cost = grp.Sum(t => t.cost) != null ? (decimal)grp.Sum(t => t.cost) : (decimal)0,
                                Impressions = grp.Sum(t => t.impressions) != null ? (int)grp.Sum(t => t.impressions) : 0,
                                KindleEditionNormalizedPagesRead14d = grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) : 0,
                                purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                AttributedSalesSameSku14d = grp.Sum(t => t.attributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.attributedSalesSameSku14d) : 0
                            }).ToList();
















            }

            //DONE

            else if (TypeToGet.ToLower() == "keywordsinadgroup")
            {
                IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate  and c.dateRecord <= @endDAte  and c.adGroupId = @adGroupId and c.Country = @Country")
                 .WithParameter("@clientId", ClientId.ToString())
                   .WithParameter("@startDate", startDate.ToUniversalTime())
                       .WithParameter("@endDAte", endDate.ToUniversalTime())
                        .WithParameter("@adGroupId", ContainerTypeId)
                      .WithParameter("@Country", CountryId);
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
                            group t by new { t.keywordId, t.campaignId, t.ProductId, t.ProductName, t.adGroupId, t.keywordType, t.CountryName, t.Country, t.campaignName, t.campaignStatus } into grp
                            select new KeywordPerformanceByMonth
                            {
                                CampaignName = grp.Key.campaignName,
                                CampaignState = grp.Key.campaignStatus,
                                ProductId = grp.Key.ProductId,
                                ProductName = grp.Key.ProductName,
                                CountryName = grp.Key.CountryName,
                                KeywordId = grp.Key.keywordId,
                                KeywordType = grp.Key.keywordType,
                                CampaignId = grp.Key.campaignId,
                                Country = grp.Key.Country,
                                AdGroupId = grp.Key.adGroupId,
                                Clicks = grp.Sum(t => t.clicks) != null ? (int)grp.Sum(t => t.clicks) : 0,
                                Cost = grp.Sum(t => t.cost) != null ? (decimal)grp.Sum(t => t.cost) : (decimal)0,
                                Impressions = grp.Sum(t => t.impressions) != null ? (int)grp.Sum(t => t.impressions) : 0,
                                KindleEditionNormalizedPagesRead14d = grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) : 0,
                                purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                AttributedSalesSameSku14d = grp.Sum(t => t.attributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.attributedSalesSameSku14d) : 0
                            }).ToList();



















            }


            //DONE

            else //we are getting performance keyword for consolidated view
            {
                IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator

                QueryDefinition queryDefinition = null;

                if (CountryId != 0)
                {
                    queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate  and c.dateRecord <= @endDAte and c.Country = @Country and (c.impressions > 0 or c.kindleEditionNormalizedPagesRead14d > 0 or c.clicks > 0 or c.unitsSoldClicks14d > 0)")
.WithParameter("@clientId", ClientId.ToString())
.WithParameter("@startDate", startDate.ToUniversalTime())
.WithParameter("@endDAte", endDate.ToUniversalTime())
                     .WithParameter("@Country", CountryId);
                }
                else
                {
                    queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate  and c.dateRecord <= @endDAte and (c.impressions > 0 or c.kindleEditionNormalizedPagesRead14d > 0 or c.clicks > 0 or c.unitsSoldClicks14d > 0)")
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
                            group t by new { t.keywordId, t.campaignId, t.ProductId, t.ProductName, t.adGroupId, t.keywordType, t.CountryName, t.Country, t.campaignName, t.campaignStatus } into grp
                            select new KeywordPerformanceByMonth
                            {
                                CampaignName = grp.Key.campaignName,
                                CampaignState = grp.Key.campaignStatus,
                                ProductId = grp.Key.ProductId,
                                ProductName = grp.Key.ProductName,
                                CountryName = grp.Key.CountryName,
                                KeywordId = grp.Key.keywordId,
                                KeywordType = grp.Key.keywordType,
                                CampaignId = grp.Key.campaignId,
                                Country = grp.Key.Country,
                                AdGroupId = grp.Key.adGroupId,
                                Clicks = grp.Sum(t => t.clicks) != null ? (int)grp.Sum(t => t.clicks) : 0,
                                Cost = grp.Sum(t => t.cost) != null ? (decimal)grp.Sum(t => t.cost) : (decimal)0,
                                Impressions = grp.Sum(t => t.impressions) != null ? (int)grp.Sum(t => t.impressions) : 0,
                                KindleEditionNormalizedPagesRead14d = grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) : 0,
                                purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                AttributedSalesSameSku14d = grp.Sum(t => t.attributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.attributedSalesSameSku14d) : 0
                            }).ToList();
            }

            return response.ToList();
        }

        public async Task<List<KeywordPerformanceByMonth>> GetMonthlyData(DateTime startDate, DateTime endDate, Guid ClientId, string TypeToGet, string ContainerTypeId = "", int CountryId = 0)
        {
            RetrieveKeywordManagementData rkd = new RetrieveKeywordManagementData();
            List<KeywordPerformanceByMonth> response = new List<KeywordPerformanceByMonth>();

            //get all campaigns
            if (TypeToGet.ToLower() == "allcampaigns")
            {
                if (CountryId != 0)
                {
                    response = await rkd.GetKeywordPerformanceDataForCampaignsInCountry(ClientId, CountryId, startDate, endDate);
                }
                else
                {
                    response = await rkd.GetKeywordPerformanceDataForCampaigns(ClientId, startDate, endDate);
                }
            }
            //this is shown on the campaign level
            else if (TypeToGet.ToLower() == "adgroupsoncampaigns")
            {
                response = await rkd.GetKeywordPerformanceDataByAdGroup(ClientId, ContainerTypeId, startDate, endDate, CountryId);
            }
            //this is shown inside the adgroup
            else if (TypeToGet.ToLower() == "keywordsinadgroup")
            {
                response = await rkd.GetKeywordDataByAdGroup(ClientId, startDate, endDate, ContainerTypeId, CountryId);
            }
            else //we are getting performance keyword for consolidated view
            {
                if (CountryId != 0)
                {
                    response = await rkd.GetKeywordPerformanceDataInCountry(ClientId, CountryId, startDate, endDate);
                }
                else
                {
                    response = await rkd.GetKeywordPerformanceData(ClientId, startDate, endDate);
                }
            }

            return response;
        }

    }
}
