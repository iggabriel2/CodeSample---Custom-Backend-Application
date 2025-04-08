using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Data.ReportData
{
    public class SaveReportData
    {
        public async Task<bool> ClearBulkSummaryTemp(Guid ClientId)
        {
            try
            {
                //clear temp table as a precaution
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("ClearBulkSummaryTemp", new { @clientid = ClientId }, commandType: CommandType.StoredProcedure);
                }

                return true;
            }
            catch(Exception ex) {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveBulkSummaryReport";
                logError.ClientId = ClientId;
                logError.Parameters = "Client Id: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return false;
            }
        }

        public async Task<bool> SaveHistoryItems(ReconcileHistory reconcileHistory)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("SaveHistoryStatus", reconcileHistory, commandType: CommandType.StoredProcedure);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveHistoryItems";
                logError.ClientId = reconcileHistory.ClientId;
                logError.Parameters = JsonSerializer.Serialize(reconcileHistory);
                await logging.WriteToLog(logError);

                return false;
            }
        }

        public async Task<bool> SaveBulkSummaryReport(Guid ClientId, List<ReportOutput> ReportOutput, ClientProfileCodes profileCode, Guid thisReportGuid)
        {
            try
            {
                ReportOutput.ForEach(s => s.ClientId = ClientId);
                ReportOutput.ForEach(s => s.Country = profileCode.CountryId);
                ReportOutput.ForEach(s => s.BulkId = thisReportGuid);

                //make sure to add 0s wherever needed
                foreach(var reportOutputItem in ReportOutput)
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



                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<ReportOutput>(ReportOutput);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.BulkCopyTimeout = 120;
                    bcopy.DestinationTableName = "[dbo].[AzSpSearchTermSummaryReportTemp]";
                    SqlBulkCopyColumnMapping mapping = new SqlBulkCopyColumnMapping("BulkId", "BulkId");
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("Country", "Country");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("keyword", "Keyword");
                    SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("keywordId", "KeywordId");
                    SqlBulkCopyColumnMapping mapping8 = new SqlBulkCopyColumnMapping("searchTerm", "SearchTerm");
                    SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("campaignId", "CampaignId");
                    SqlBulkCopyColumnMapping mapping10 = new SqlBulkCopyColumnMapping("campaignName", "CampaignName");
                    SqlBulkCopyColumnMapping mapping11 = new SqlBulkCopyColumnMapping("clicks", "Clicks");
                    SqlBulkCopyColumnMapping mapping12 = new SqlBulkCopyColumnMapping("keywordType", "KeywordType");
                    SqlBulkCopyColumnMapping mapping13 = new SqlBulkCopyColumnMapping("impressions", "Impressions");
                    SqlBulkCopyColumnMapping mapping14 = new SqlBulkCopyColumnMapping("adGroupId", "AdGroupId");
                    SqlBulkCopyColumnMapping mapping15 = new SqlBulkCopyColumnMapping("costPerClick", "CostPerClick");
                    SqlBulkCopyColumnMapping mapping16 = new SqlBulkCopyColumnMapping("portfolioId", "PortfolioId");
                    SqlBulkCopyColumnMapping mapping17 = new SqlBulkCopyColumnMapping("purchases14d", "purchases14d");
                    SqlBulkCopyColumnMapping mapping18 = new SqlBulkCopyColumnMapping("kindleEditionNormalizedPagesRead14d", "KindleEditionNormalizedPagesRead14d");
                    SqlBulkCopyColumnMapping mapping19 = new SqlBulkCopyColumnMapping("attributedSalesSameSku14d", "AttributedSalesSameSku14d");
                    SqlBulkCopyColumnMapping mapping20 = new SqlBulkCopyColumnMapping("clickThroughRate", "ClickThroughRate");
                    SqlBulkCopyColumnMapping mapping21 = new SqlBulkCopyColumnMapping("roasClicks14d", "RoasClicks14d");
                    SqlBulkCopyColumnMapping mapping22 = new SqlBulkCopyColumnMapping("unitsSoldClicks14d", "UnitsSoldClicks14d");
                    SqlBulkCopyColumnMapping mapping23 = new SqlBulkCopyColumnMapping("campaignStatus", "CampaignStatus");
                    SqlBulkCopyColumnMapping mapping24 = new SqlBulkCopyColumnMapping("cost", "Cost");

                    bcopy.ColumnMappings.Add(mapping);
                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping6);
                    bcopy.ColumnMappings.Add(mapping8);
                    bcopy.ColumnMappings.Add(mapping9);
                    bcopy.ColumnMappings.Add(mapping10);
                    bcopy.ColumnMappings.Add(mapping11);
                    bcopy.ColumnMappings.Add(mapping12);
                    bcopy.ColumnMappings.Add(mapping13);
                    bcopy.ColumnMappings.Add(mapping14);
                    bcopy.ColumnMappings.Add(mapping15);
                    bcopy.ColumnMappings.Add(mapping16);
                    bcopy.ColumnMappings.Add(mapping17);
                    bcopy.ColumnMappings.Add(mapping18);
                    bcopy.ColumnMappings.Add(mapping19);
                    bcopy.ColumnMappings.Add(mapping20);
                    bcopy.ColumnMappings.Add(mapping21);
                    bcopy.ColumnMappings.Add(mapping22);
                    bcopy.ColumnMappings.Add(mapping23);
                    bcopy.ColumnMappings.Add(mapping24);
                    bcopy.WriteToServer(dt);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveBulkSummaryReport";
                logError.ClientId = ClientId;
                logError.Parameters = JsonSerializer.Serialize(ReportOutput);
                await logging.WriteToLog(logError);

                return false;
            }

        }

        public async Task<bool> ReconcileSummarySearchTermReport(Guid ClientId)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("BulkReportSummarySearchTermProcessing", new { @clientid = ClientId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ReconcileSummarySearchTermReport";
                logError.ClientId = ClientId;
                logError.Parameters = "Client Id: " + ClientId.ToString();
                await logging.WriteToLog(logError);
                return false;
            }

            return true;
        }
        
        public async Task<bool> SaveDailyReport(List<DailyReportSave> dailyReport, Guid ClientId)
        {
            try
            {
                foreach (var dailyReportItem in dailyReport)
                {
                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        var affectedRows = await connection.ExecuteAsync("SaveDailyReport", new { @ReportDate = dailyReportItem.ReportDate, @ClientId = dailyReportItem.ClientId, @CountryId = dailyReportItem.CountryId }, commandType: CommandType.StoredProcedure);
                    }

                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        var affectedRows = await connection.ExecuteAsync("SaveDailyReport2", dailyReportItem, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveDailyReport";
                logError.ClientId = ClientId;
                logError.Parameters = JsonSerializer.Serialize(dailyReport);
                await logging.WriteToLog(logError);
                return false;
            }

            return true;
        }



        public async Task<bool> SaveMonthlySummaryReport(Guid ClientId, List<MonthlyReportOutput> ReportOutput, ClientProfileCodes profileCode, List<string> savingDates)
        {
            try
            {
                //clear temp table as a precaution

                var consolidatedSavingDates = savingDates.Distinct();

                foreach (var savingDate in consolidatedSavingDates)
                {
                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        var affectedRows = await connection.ExecuteAsync("SaveMonthlySummaryReportDelete", new { @clientid = ClientId, @ReportMonth = savingDate, @country = profileCode.CountryId }, commandType: CommandType.StoredProcedure);
                    }
                }

                ReportOutput.ForEach(s => s.ClientId = ClientId);
                ReportOutput.ForEach(s => s.Country = profileCode.CountryId);

                foreach (var reportOutputItem in ReportOutput)
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

                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<MonthlyReportOutput>(ReportOutput);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.BulkCopyTimeout = 120;
                    bcopy.DestinationTableName = "dbo.AzSpSearchTermMonthlySummaryReport";
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("Country", "Country");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("keyword", "Keyword");
                    SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("keywordId", "KeywordId");

                    SqlBulkCopyColumnMapping mapping8 = new SqlBulkCopyColumnMapping("searchTerm", "SearchTerm");
                    SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("campaignId", "CampaignId");
                    SqlBulkCopyColumnMapping mapping10 = new SqlBulkCopyColumnMapping("campaignName", "CampaignName");
                    SqlBulkCopyColumnMapping mapping11 = new SqlBulkCopyColumnMapping("clicks", "Clicks");
                    SqlBulkCopyColumnMapping mapping12 = new SqlBulkCopyColumnMapping("keywordType", "KeywordType");
                    SqlBulkCopyColumnMapping mapping13 = new SqlBulkCopyColumnMapping("impressions", "Impressions");
                    SqlBulkCopyColumnMapping mapping14 = new SqlBulkCopyColumnMapping("adGroupId", "AdGroupId");
                    SqlBulkCopyColumnMapping mapping16 = new SqlBulkCopyColumnMapping("portfolioId", "PortfolioId");
                    SqlBulkCopyColumnMapping mapping17 = new SqlBulkCopyColumnMapping("purchases14d", "purchases14d");
                    SqlBulkCopyColumnMapping mapping18 = new SqlBulkCopyColumnMapping("kindleEditionNormalizedPagesRead14d", "KindleEditionNormalizedPagesRead14d");
                    SqlBulkCopyColumnMapping mapping19 = new SqlBulkCopyColumnMapping("attributedSalesSameSku14d", "AttributedSalesSameSku14d");
                    SqlBulkCopyColumnMapping mapping22 = new SqlBulkCopyColumnMapping("unitsSoldClicks14d", "UnitsSoldClicks14d");
                    SqlBulkCopyColumnMapping mapping23 = new SqlBulkCopyColumnMapping("campaignStatus", "CampaignStatus");
                    SqlBulkCopyColumnMapping mapping24 = new SqlBulkCopyColumnMapping("cost", "Cost");
                    SqlBulkCopyColumnMapping mapping25 = new SqlBulkCopyColumnMapping("savingDate", "ReportMonth");
                    SqlBulkCopyColumnMapping mapping26 = new SqlBulkCopyColumnMapping("dateRecord", "ReportMonthDate");

                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping6);
                    bcopy.ColumnMappings.Add(mapping8);
                    bcopy.ColumnMappings.Add(mapping9);
                    bcopy.ColumnMappings.Add(mapping10);
                    bcopy.ColumnMappings.Add(mapping11);
                    bcopy.ColumnMappings.Add(mapping12);
                    bcopy.ColumnMappings.Add(mapping13);
                    bcopy.ColumnMappings.Add(mapping14);
                    bcopy.ColumnMappings.Add(mapping16);
                    bcopy.ColumnMappings.Add(mapping17);
                    bcopy.ColumnMappings.Add(mapping18);
                    bcopy.ColumnMappings.Add(mapping19);
                    bcopy.ColumnMappings.Add(mapping22);
                    bcopy.ColumnMappings.Add(mapping23);
                    bcopy.ColumnMappings.Add(mapping24);
                    bcopy.ColumnMappings.Add(mapping25);
                    bcopy.ColumnMappings.Add(mapping26);
                    bcopy.WriteToServer(dt);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveMonthlySummaryReport";
                logError.ClientId = ClientId;
                logError.Parameters = JsonSerializer.Serialize(ReportOutput);
                await logging.WriteToLog(logError);

                return false;
            }

        }











        public async Task<bool> SaveMonthlySummaryReportForKeywords(Guid ClientId, List<MonthlyReportOutputForKeywords> ReportOutput, ClientProfileCodes profileCode, List<string> savingDates)
        {
            try
            {
                //clear temp table as a precaution

                var consolidatedSavingDates = savingDates.Distinct();

                foreach (var savingDate in consolidatedSavingDates)
                {
                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        var affectedRows = await connection.ExecuteAsync("SaveMonthlySummaryReportDeleteForKeywords", new { @clientid = ClientId, @ReportMonth = savingDate, @country = profileCode.CountryId }, commandType: CommandType.StoredProcedure);
                    }
                }

                ReportOutput.ForEach(s => s.ClientId = ClientId);
                ReportOutput.ForEach(s => s.Country = profileCode.CountryId);

                foreach (var reportOutputItem in ReportOutput)
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

                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<MonthlyReportOutputForKeywords>(ReportOutput);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.BulkCopyTimeout = 120;
                    bcopy.DestinationTableName = "dbo.AzSpSearchTermMonthlySummaryReportForKeywords";
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("Country", "Country");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("keyword", "Keyword");
                    SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("keywordId", "KeywordId");
                    SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("campaignId", "CampaignId");
                    SqlBulkCopyColumnMapping mapping10 = new SqlBulkCopyColumnMapping("campaignName", "CampaignName");
                    SqlBulkCopyColumnMapping mapping11 = new SqlBulkCopyColumnMapping("clicks", "Clicks");
                    SqlBulkCopyColumnMapping mapping12 = new SqlBulkCopyColumnMapping("keywordType", "KeywordType");
                    SqlBulkCopyColumnMapping mapping13 = new SqlBulkCopyColumnMapping("impressions", "Impressions");
                    SqlBulkCopyColumnMapping mapping14 = new SqlBulkCopyColumnMapping("adGroupId", "AdGroupId");
                    SqlBulkCopyColumnMapping mapping16 = new SqlBulkCopyColumnMapping("portfolioId", "PortfolioId");
                    SqlBulkCopyColumnMapping mapping17 = new SqlBulkCopyColumnMapping("purchases14d", "purchases14d");
                    SqlBulkCopyColumnMapping mapping18 = new SqlBulkCopyColumnMapping("kindleEditionNormalizedPagesRead14d", "KindleEditionNormalizedPagesRead14d");
                    SqlBulkCopyColumnMapping mapping19 = new SqlBulkCopyColumnMapping("attributedSalesSameSku14d", "AttributedSalesSameSku14d");
                    SqlBulkCopyColumnMapping mapping22 = new SqlBulkCopyColumnMapping("unitsSoldClicks14d", "UnitsSoldClicks14d");
                    SqlBulkCopyColumnMapping mapping23 = new SqlBulkCopyColumnMapping("campaignStatus", "CampaignStatus");
                    SqlBulkCopyColumnMapping mapping24 = new SqlBulkCopyColumnMapping("cost", "Cost");
                    SqlBulkCopyColumnMapping mapping25 = new SqlBulkCopyColumnMapping("savingDate", "ReportMonth");
                    SqlBulkCopyColumnMapping mapping26 = new SqlBulkCopyColumnMapping("dateRecord", "ReportMonthDate");

                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping6);
                    bcopy.ColumnMappings.Add(mapping9);
                    bcopy.ColumnMappings.Add(mapping10);
                    bcopy.ColumnMappings.Add(mapping11);
                    bcopy.ColumnMappings.Add(mapping12);
                    bcopy.ColumnMappings.Add(mapping13);
                    bcopy.ColumnMappings.Add(mapping14);
                    bcopy.ColumnMappings.Add(mapping16);
                    bcopy.ColumnMappings.Add(mapping17);
                    bcopy.ColumnMappings.Add(mapping18);
                    bcopy.ColumnMappings.Add(mapping19);
                    bcopy.ColumnMappings.Add(mapping22);
                    bcopy.ColumnMappings.Add(mapping23);
                    bcopy.ColumnMappings.Add(mapping24);
                    bcopy.ColumnMappings.Add(mapping25);
                    bcopy.ColumnMappings.Add(mapping26);
                    bcopy.WriteToServer(dt);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveMonthlySummaryReportForKeywords";
                logError.ClientId = ClientId;
                logError.Parameters = JsonSerializer.Serialize(ReportOutput);
                await logging.WriteToLog(logError);

                return false;
            }

        }

        public async Task<int> SaveReportProcessingStart()
        {
            try
            {
                int RecordUpdating = 0;
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    RecordUpdating = (await connection.QueryAsync<int>("SaveReportProcessingStart", new { @ProcessingDate = DateTime.Now.Date , @StartTime = DateTime.Now }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return RecordUpdating;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveReportProcessingStart - Reports didn't run";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "No Parameters";
                await logging.WriteToLog(logError);
                return 0;
            }
        }

        public async Task<bool> SaveReportProcessingEnd(int ID)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("SaveReportProcessingEnd", new { @EndTime = DateTime.Now, @id = ID }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveReportProcessingEnd";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "No Parameters";
                await logging.WriteToLog(logError);
                return false;
            }

            return true;
        }

        public async Task<int> SaveReportDataByClientProfileCode(ReportLoggingByClient reportRunByClient)
        {
            int insertedId = 0;

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    insertedId = (await connection.QueryAsync<int>("SaveReportDataByClientProfileCode", new { @AzClientId = reportRunByClient.AzClientId, @CountryId = reportRunByClient.CountryId, @LastRunDate = reportRunByClient.LastRunDate, @StartDate = reportRunByClient.StartDate }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveReportDataByClientProfileCode";
                logError.ClientId = reportRunByClient.AzClientId;
                logError.Parameters = JsonSerializer.Serialize(reportRunByClient);
                await logging.WriteToLog(logError);
            }

            return insertedId;
        }

        public async Task<bool> UpdateLastProcessingReportDate(ReportLoggingByClient reportRunByClient)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateLastProcessingReportDate", new { @Id = reportRunByClient.Id, @Today = reportRunByClient.Today}, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateLastProcessingReportDate";
                logError.ClientId = reportRunByClient.AzClientId;
                logError.Parameters = JsonSerializer.Serialize(reportRunByClient);
                await logging.WriteToLog(logError);
                return false;
            }

            return true;
        }


        public async System.Threading.Tasks.Task<bool> EditOrUpdateBilledAmounts(OverageAmounts overageAmounts)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("EditOrUpdateBilledAmounts", overageAmounts, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "EditOrUpdateBilledAmounts";
                logError.Parameters = JsonSerializer.Serialize(overageAmounts);
                await logging.WriteToLog(logError);
                return false;
            }
        }
    }
}
