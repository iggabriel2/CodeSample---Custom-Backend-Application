using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using Configuration;
using Google.Type;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class MonthlySummaryReportLogicForKeywords
    {
        private static readonly object ReportLock = new object();

        public async Task<bool> ProccessReport(APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, string reportUrl, bool ProcessLastMonth = false)
        {
            try
            {
                string rawReportOutput = "";
                if (keepProccessing)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(reportUrl);

                        if (response.IsSuccessStatusCode)
                        {

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (GZipStream csStream = new GZipStream(stream, CompressionMode.Decompress))
                            {
                                StreamReader reader = new StreamReader(csStream);
                                rawReportOutput = reader.ReadToEnd();
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(rawReportOutput))
                    {
                        keepProccessing = false;
                    }
                }

                if (keepProccessing)
                {
                    List<MonthlyReportOutputForKeywords> reportOutputRaw = JsonConvert.DeserializeObject<List<MonthlyReportOutputForKeywords>>(rawReportOutput);

                    //consolidate results
                    List<MonthlyReportOutputForKeywords> reportOutput = new List<MonthlyReportOutputForKeywords>();

                    List<string> savingDates = new List<string>();

                    foreach (var reportOutputItem in reportOutputRaw)
                    {
                        string savingDateFormated = Convert.ToDateTime(reportOutputItem.date).ToString("MMyyyy");
                        System.DateTime dateRecord = new System.DateTime(Convert.ToDateTime(reportOutputItem.date).Year, Convert.ToDateTime(reportOutputItem.date).Month, 1);

                        MonthlyReportOutputForKeywords monthlyReportOutputExists = reportOutput.Where(x => x.campaignId == reportOutputItem.campaignId && x.savingDate == savingDateFormated && x.keywordId == reportOutputItem.keywordId && x.keyword == reportOutputItem.keyword && x.adGroupId == reportOutputItem.adGroupId && x.keywordType == reportOutputItem.keywordType).FirstOrDefault();

                        if (monthlyReportOutputExists != null)
                        {
                            monthlyReportOutputExists.impressions = monthlyReportOutputExists.impressions + reportOutputItem.impressions;
                            monthlyReportOutputExists.clicks = monthlyReportOutputExists.clicks + reportOutputItem.clicks;
                            monthlyReportOutputExists.unitsSoldClicks14d = monthlyReportOutputExists.unitsSoldClicks14d + reportOutputItem.unitsSoldClicks14d;
                            monthlyReportOutputExists.kindleEditionNormalizedPagesRead14d = monthlyReportOutputExists.kindleEditionNormalizedPagesRead14d + reportOutputItem.kindleEditionNormalizedPagesRead14d;
                            monthlyReportOutputExists.attributedSalesSameSku14d = monthlyReportOutputExists.attributedSalesSameSku14d + reportOutputItem.attributedSalesSameSku14d;
                            monthlyReportOutputExists.cost = monthlyReportOutputExists.cost + reportOutputItem.cost;
                            monthlyReportOutputExists.purchases14d = monthlyReportOutputExists.purchases14d + reportOutputItem.purchases14d;
                        }
                        else
                        {
                            savingDates.Add(savingDateFormated);

                            MonthlyReportOutputForKeywords monthlyReportOutput = new MonthlyReportOutputForKeywords();

                            monthlyReportOutput.campaignId = reportOutputItem.campaignId;
                            monthlyReportOutput.savingDate = savingDateFormated;
                            monthlyReportOutput.keywordId = reportOutputItem.keywordId;
                            monthlyReportOutput.keyword = reportOutputItem.keyword;
                            monthlyReportOutput.adGroupId = reportOutputItem.adGroupId;
                            monthlyReportOutput.keywordType = reportOutputItem.keywordType;
                            monthlyReportOutput.campaignName = reportOutputItem.campaignName;
                            monthlyReportOutput.portfolioId = reportOutputItem.portfolioId;
                            monthlyReportOutput.adGroupName = reportOutputItem.adGroupName;
                            monthlyReportOutput.campaignStatus = reportOutputItem.campaignStatus;
                            monthlyReportOutput.dateRecord = dateRecord;

                            monthlyReportOutput.impressions = reportOutputItem.impressions;
                            monthlyReportOutput.clicks = reportOutputItem.clicks;
                            monthlyReportOutput.unitsSoldClicks14d = reportOutputItem.unitsSoldClicks14d;
                            monthlyReportOutput.kindleEditionNormalizedPagesRead14d = reportOutputItem.kindleEditionNormalizedPagesRead14d;
                            monthlyReportOutput.attributedSalesSameSku14d = reportOutputItem.attributedSalesSameSku14d;
                            monthlyReportOutput.cost = reportOutputItem.cost;
                            monthlyReportOutput.purchases14d = reportOutputItem.purchases14d;

                            reportOutput.Add(monthlyReportOutput);
                        }
                    }

                    //save first report to db
                    SaveReportData saveReportData = new SaveReportData();
                    var saveResponse = await saveReportData.SaveMonthlySummaryReportForKeywords(aPIAuthorizationRequest.ClientId, reportOutput, profileCode, savingDates);

                    //daily view for keywords

                    List<DailyKeywordDataOutputForKeywords> dailyOutput = new List<DailyKeywordDataOutputForKeywords>();
                    List<KeywordDetailsForCosmos> keywordDetails = new List<KeywordDetailsForCosmos>();
                    List<KeywordNegativesForCosmos> keywordNegatives = new List<KeywordNegativesForCosmos>();

                    RetrieveNightlyExtras rnd = new RetrieveNightlyExtras();
                    keywordDetails = await rnd.GetKeywordDetailsForCosmos(aPIAuthorizationRequest.ClientId, profileCode.CountryId);
                    keywordNegatives = await rnd.GetKeywordNegativesForCosmos(aPIAuthorizationRequest.ClientId, profileCode.CountryId);

                    foreach (var reportOutputItem in reportOutputRaw)
                    {
                        System.DateTime dateDecision = Convert.ToDateTime(reportOutputItem.date);

                        if (dateDecision > System.DateTime.Now.AddDays(-7))
                        {
                            string savingDateFormated = Convert.ToDateTime(reportOutputItem.date).ToString("yyyyMMdd");
                            System.DateTime dateRecord = Convert.ToDateTime(reportOutputItem.date).ToUniversalTime();

                            DailyKeywordDataOutputForKeywords dailyReportOutput = new DailyKeywordDataOutputForKeywords();

                            dailyReportOutput.campaignId = reportOutputItem.campaignId;
                            dailyReportOutput.savingDate = savingDateFormated;
                            dailyReportOutput.keywordId = reportOutputItem.keywordId;
                            dailyReportOutput.keyword = reportOutputItem.keyword;
                            dailyReportOutput.adGroupId = reportOutputItem.adGroupId;
                            dailyReportOutput.keywordType = reportOutputItem.keywordType;
                            dailyReportOutput.campaignName = reportOutputItem.campaignName;
                            dailyReportOutput.portfolioId = reportOutputItem.portfolioId;
                            dailyReportOutput.adGroupName = reportOutputItem.adGroupName;
                            dailyReportOutput.campaignStatus = reportOutputItem.campaignStatus;
                            dailyReportOutput.dateRecord = dateRecord;

                            try
                            {
                                KeywordDetailsForCosmos keywordDetailHere = keywordDetails.Where(x => x.CampaignId == dailyReportOutput.campaignId).FirstOrDefault();

                                dailyReportOutput.ProductId = keywordDetailHere.ProductId;
                                dailyReportOutput.ProductName = keywordDetailHere.ProductName;
                                dailyReportOutput.CountryName = keywordDetailHere.CountryName;
                                dailyReportOutput.QAPCampaignId = keywordDetailHere.QAPCampaignId;
                                dailyReportOutput.UsageType = keywordDetailHere.UsageType;
                            }
                            catch (Exception ex)
                            {
                                await ErrorLogging.LogError(ex.ToString() + " Failed to get all needed campaigns for Cosmos", "MonthlySummaryReportLogicForKeywords - ProccessReport", "Country Id: " + profileCode.CountryId.ToString(), aPIAuthorizationRequest.ClientId);
                                dailyReportOutput.ProductId = 0;
                                dailyReportOutput.ProductName = "";
                                dailyReportOutput.CountryName = "";
                            }

                            KeywordNegativesForCosmos negativeItem = new KeywordNegativesForCosmos();
                            negativeItem = keywordNegatives.Where(x => x.KeywordId == dailyReportOutput.keywordId).FirstOrDefault();

                            if (negativeItem != null)
                            {
                                dailyReportOutput.Negative = true;
                            }
                            else
                            {
                                dailyReportOutput.Negative = false;
                            }


                            dailyReportOutput.impressions = reportOutputItem.impressions;
                            dailyReportOutput.clicks = reportOutputItem.clicks;
                            dailyReportOutput.unitsSoldClicks14d = reportOutputItem.unitsSoldClicks14d;
                            dailyReportOutput.kindleEditionNormalizedPagesRead14d = reportOutputItem.kindleEditionNormalizedPagesRead14d;
                            dailyReportOutput.attributedSalesSameSku14d = reportOutputItem.attributedSalesSameSku14d;
                            dailyReportOutput.cost = reportOutputItem.cost;
                            //dailyReportOutput.costPerClick = reportOutputItem.costPerClick;
                            dailyReportOutput.purchases14d = reportOutputItem.purchases14d;
                            dailyReportOutput.id = aPIAuthorizationRequest.ClientId.ToString() + "." + profileCode.CountryId.ToString() + "." + reportOutputItem.keywordId.ToString() + "." + savingDateFormated;
                            dailyReportOutput.ClientId = aPIAuthorizationRequest.ClientId.ToString();
                            dailyReportOutput.Country = profileCode.CountryId;
                            dailyReportOutput.partitionKey = aPIAuthorizationRequest.ClientId.ToString();

                            dailyOutput.Add(dailyReportOutput);

                        }
                    }


                    foreach (var reportOutputItem in dailyOutput)
                    {
                        if (reportOutputItem.cost == null)
                            reportOutputItem.cost = 0;

                        if (reportOutputItem.clicks == null)
                            reportOutputItem.clicks = 0;

                        if (reportOutputItem.impressions == null)
                            reportOutputItem.impressions = 0;

                        if (reportOutputItem.costPerClick == null)
                            reportOutputItem.costPerClick = 0;

                        if (reportOutputItem.kindleEditionNormalizedPagesRead14d == null)
                            reportOutputItem.kindleEditionNormalizedPagesRead14d = 0;

                        if (reportOutputItem.purchases14d == null)
                            reportOutputItem.purchases14d = 0;

                        if (reportOutputItem.attributedSalesSameSku14d == null)
                            reportOutputItem.attributedSalesSameSku14d = 0;

                        if (reportOutputItem.clickThroughRate == null)
                            reportOutputItem.clickThroughRate = 0;

                        if (reportOutputItem.roasClicks14d == null)
                            reportOutputItem.roasClicks14d = 0;

                        if (reportOutputItem.unitsSoldClicks14d == null)
                            reportOutputItem.unitsSoldClicks14d = 0;
                    }

                    var savedToCosmos = await SendKeywordDataToCosmos(dailyOutput, aPIAuthorizationRequest.ClientId);

                    List<DailyCampaignData> dailyCampaignData = new List<DailyCampaignData>();

                    //campaign data
                    dailyCampaignData = (from t in dailyOutput
                                         group t by new { t.portfolioId, t.ClientId, t.dateRecord, t.savingDate, t.campaignId, t.ProductId, t.ProductName, t.CountryName, t.Country, t.campaignName, t.campaignStatus, t.UsageType, t.QAPCampaignId } into grp
                                select new DailyCampaignData
                                {
                                    ClientId = aPIAuthorizationRequest.ClientId.ToString(),
                                    partitionKey = aPIAuthorizationRequest.ClientId.ToString(),
                                    id = aPIAuthorizationRequest.ClientId.ToString() + "." + profileCode.CountryId.ToString() + "." + grp.Key.campaignId.ToString() + "." + grp.Key.savingDate,
                                    portfolioId = grp.Key.portfolioId,
                                    dateRecord = grp.Key.dateRecord,
                                    savingDate = grp.Key.savingDate,
                                    campaignName = grp.Key.campaignName,
                                    campaignStatus = grp.Key.campaignStatus.ToUpper(),
                                    ProductId = grp.Key.ProductId,
                                    ProductName = grp.Key.ProductName,
                                    CountryName = grp.Key.CountryName,
                                    UsageType = grp.Key.UsageType,
                                    QAPCampaignId = grp.Key.QAPCampaignId != null ? (int)grp.Key.QAPCampaignId : 0,
                                    campaignId = grp.Key.campaignId,
                                    Country = grp.Key.Country,
                                    clicks = grp.Sum(t => t.clicks) != null ? (int)grp.Sum(t => t.clicks) : 0,
                                    cost = grp.Sum(t => t.cost) != null ? (decimal)grp.Sum(t => t.cost) : (decimal)0,
                                    impressions = grp.Sum(t => t.impressions) != null ? (int)grp.Sum(t => t.impressions) : 0,
                                    kindleEditionNormalizedPagesRead14d = grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.kindleEditionNormalizedPagesRead14d) : 0,
                                    purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                    unitsSoldClicks14d = grp.Sum(t => t.unitsSoldClicks14d) != null ? (int)grp.Sum(t => t.unitsSoldClicks14d) : 0,
                                    attributedSalesSameSku14d = grp.Sum(t => t.attributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.attributedSalesSameSku14d) : 0,
                                }).ToList();








                    var savedToCosmos2 = await SendCampaignDataToCosmos(dailyCampaignData, aPIAuthorizationRequest.ClientId);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public async Task<bool> SendKeywordDataToCosmos(List<DailyKeywordDataOutputForKeywords> ReportOutputData, Guid clientId)
        {
            try
            {
                //make cosmos client
                CosmosClient cosmosInstance = new CosmosClient(Cosmos.CosmosUri, Cosmos.CosmosKey, new CosmosClientOptions() { AllowBulkExecution = true });
                Database database = await cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosKeywordDataContainer, "/partitionKey");


                var concurrentTasks = new List<System.Threading.Tasks.Task>();

                foreach (DailyKeywordDataOutputForKeywords item in ReportOutputData)
                {
                    concurrentTasks.Add(container.UpsertItemAsync<DailyKeywordDataOutputForKeywords>(item, new PartitionKey(item.partitionKey)));
                }

                await System.Threading.Tasks.Task.WhenAll(concurrentTasks);

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SendKeywordDataToCosmos", JsonConvert.SerializeObject(ReportOutputData), clientId);
                return false;
            }

            return true;

        }

        public async Task<bool> SendCampaignDataToCosmos(List<DailyCampaignData> ReportOutputData, Guid clientId)
        {
            try
            {
                //make cosmos client
                CosmosClient cosmosInstance = new CosmosClient(Cosmos.CosmosUri, Cosmos.CosmosKey, new CosmosClientOptions() { AllowBulkExecution = true });
                Database database = await cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosCampaignDataContainer, "/partitionKey");


                var concurrentTasks = new List<System.Threading.Tasks.Task>();

                foreach (DailyCampaignData item in ReportOutputData)
                {
                    concurrentTasks.Add(container.UpsertItemAsync<DailyCampaignData>(item, new PartitionKey(item.partitionKey)));
                }

                await System.Threading.Tasks.Task.WhenAll(concurrentTasks);

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SendCampaignDataToCosmos", JsonConvert.SerializeObject(ReportOutputData), clientId);
                return false;
            }

            return true;

        }
    }
}
