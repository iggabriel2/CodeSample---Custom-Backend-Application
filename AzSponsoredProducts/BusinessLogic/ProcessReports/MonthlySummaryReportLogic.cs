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
    public class MonthlySummaryReportLogic
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
                    List<MonthlyReportOutput> reportOutputRaw = JsonConvert.DeserializeObject<List<MonthlyReportOutput>>(rawReportOutput);

                    //consolidate results
                    List<MonthlyReportOutput> reportOutput = new List<MonthlyReportOutput>();

                    List<string> savingDates = new List<string>();

                    foreach(var reportOutputItem in reportOutputRaw)
                    {
                        string savingDateFormated = Convert.ToDateTime(reportOutputItem.date).ToString("MMyyyy");
                        System.DateTime dateRecord = new System.DateTime(Convert.ToDateTime(reportOutputItem.date).Year, Convert.ToDateTime(reportOutputItem.date).Month, 1);

                        MonthlyReportOutput monthlyReportOutputExists = reportOutput.Where(x => x.searchTerm == reportOutputItem.searchTerm && x.campaignId == reportOutputItem.campaignId && x.savingDate == savingDateFormated && x.keywordId == reportOutputItem.keywordId && x.keyword == reportOutputItem.keyword && x.adGroupId == reportOutputItem.adGroupId && x.keywordType == reportOutputItem.keywordType).FirstOrDefault();

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

                            MonthlyReportOutput monthlyReportOutput = new MonthlyReportOutput();

                            monthlyReportOutput.searchTerm = reportOutputItem.searchTerm;
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
                    var saveResponse = await saveReportData.SaveMonthlySummaryReport(aPIAuthorizationRequest.ClientId, reportOutput, profileCode, savingDates);


















                    //daily view for search terms

                    List<DailyKeywordDataOutput> dailyOutput = new List<DailyKeywordDataOutput>();
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

                            DailyKeywordDataOutput dailyReportOutput = new DailyKeywordDataOutput();

                            dailyReportOutput.searchTerm = reportOutputItem.searchTerm;
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
                            catch(Exception ex)
                            {
                                await ErrorLogging.LogError(ex.ToString() + " Failed to get all needed campaigns for Cosmos", "MonthlySummaryReportLogic - ProccessReport", "Country Id: " + profileCode.CountryId.ToString(), aPIAuthorizationRequest.ClientId);
                                dailyReportOutput.ProductId = 0;
                                dailyReportOutput.ProductName = "";
                                dailyReportOutput.CountryName = "";
                            }

                            KeywordNegativesForCosmos negativeItem = new KeywordNegativesForCosmos();
                            negativeItem = keywordNegatives.Where(x => x.KeywordId == dailyReportOutput.keywordId && x.SearchTerm == dailyReportOutput.searchTerm).FirstOrDefault();

                            if (negativeItem != null)
                            {
                                dailyReportOutput.Negative = true;
                            }
                            else
                            {
                                dailyReportOutput.Negative= false;
                            }


                            dailyReportOutput.impressions = reportOutputItem.impressions;
                            dailyReportOutput.clicks = reportOutputItem.clicks;
                            dailyReportOutput.unitsSoldClicks14d = reportOutputItem.unitsSoldClicks14d;
                            dailyReportOutput.kindleEditionNormalizedPagesRead14d = reportOutputItem.kindleEditionNormalizedPagesRead14d;
                            dailyReportOutput.attributedSalesSameSku14d = reportOutputItem.attributedSalesSameSku14d;
                            dailyReportOutput.cost = reportOutputItem.cost;
                            //dailyReportOutput.costPerClick = reportOutputItem.costPerClick;
                            dailyReportOutput.purchases14d = reportOutputItem.purchases14d;
                            dailyReportOutput.id = aPIAuthorizationRequest.ClientId.ToString() + "." + profileCode.CountryId.ToString() + "." + reportOutputItem.keywordId.ToString() + "." + savingDateFormated + "." + reportOutputItem.searchTerm.Replace(" ","");
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


        public async Task<bool> SendKeywordDataToCosmos(List<DailyKeywordDataOutput> ReportOutputData, Guid clientId)
        {
            try
            {
                //make cosmos client
                Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosSearchTermsDataContainer, "/partitionKey");


                var concurrentTasks = new List<System.Threading.Tasks.Task>();

                foreach (DailyKeywordDataOutput item in ReportOutputData)
                {
                    concurrentTasks.Add(container.UpsertItemAsync<DailyKeywordDataOutput>(item, new PartitionKey(item.partitionKey)));
                }

                await System.Threading.Tasks.Task.WhenAll(concurrentTasks);

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SendKeywordDataToSnapshot", JsonConvert.SerializeObject(ReportOutputData), clientId);
                return false;
            }

            return true;

        }
    }
}
