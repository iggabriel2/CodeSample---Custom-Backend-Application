using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Logging;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Data.KeywordManagement
{
    public class SaveKeywordManagementData
    {
        public async Task<bool> SaveKeywordHistory(List<SaveKeywordHistory> saveKeywordHistories, ReportUser reportUser)
        {
            try
            {
               
                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<SaveKeywordHistory>(saveKeywordHistories);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.BulkCopyTimeout = 120;
                    bcopy.DestinationTableName = "dbo.AzSpKeywordAutomationHistoryTemp";
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("DateProcessed", "DateProcessed");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("CountryId", "CountryId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("SearchTerm", "SearchTerm");
                    SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("ProductId", "ProductId");
                    SqlBulkCopyColumnMapping mapping8 = new SqlBulkCopyColumnMapping("Action", "Action");
                    SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("Reason", "Reason");
                    SqlBulkCopyColumnMapping mapping10 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");

                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping6);
                    bcopy.ColumnMappings.Add(mapping8);
                    bcopy.ColumnMappings.Add(mapping9);
                    bcopy.ColumnMappings.Add(mapping10);
                    bcopy.WriteToServer(dt);
                }

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("ReconcileKeywordHistoryTemp", new { @ClientId = reportUser.ClientId }, commandType: CommandType.StoredProcedure);
                }

                return true;
            }
            catch (Exception ex)
            {
                Guid clientIdGuid = new Guid(saveKeywordHistories[0].ClientId.ToString());

                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveKeywordHistory";
                logError.ClientId = clientIdGuid;
                logError.Parameters = JsonSerializer.Serialize(saveKeywordHistories);
                await logging.WriteToLog(logError);

                return false;
            }

        }

        public async Task<bool> SaveActionsRequired(List<SaveActionRequired> saveActionRequireds, ReportUser reportUser)
        {
            try
            {

                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<SaveActionRequired>(saveActionRequireds);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.BulkCopyTimeout = 120;
                    bcopy.DestinationTableName = "dbo.ActionRequiredTemp";
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("AzCampaignId", "AzCampaignId");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("ActionId", "ActionId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("Description", "Description");
                    SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("Resolved", "Resolved");
                    SqlBulkCopyColumnMapping mapping8 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");
                    SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("CountryId", "CountryId");

                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping6);
                    bcopy.ColumnMappings.Add(mapping8);
                    bcopy.ColumnMappings.Add(mapping9);
                    bcopy.WriteToServer(dt);
                }

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("ReconcileActionRequiredTemp", new { @ClientId = reportUser.ClientId}, commandType: CommandType.StoredProcedure);
                }

                return true;
            }
            catch (Exception ex)
            {
                Guid clientIdGuid = new Guid(saveActionRequireds[0].ClientId.ToString());

                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveActionsRequired";
                logError.ClientId = clientIdGuid;
                logError.Parameters = JsonSerializer.Serialize(saveActionRequireds);
                await logging.WriteToLog(logError);

                return false;
            }

        }


        //this was the original sp - not currently in use
        //public async Task<bool> SaveKeywordNegPos(List<SaveSummaryReportAction> saveSummaryReportActions, Guid ClientId)
        //{
        //    try
        //    {
        //        saveSummaryReportActions.ForEach(s => s.ClientId = ClientId);

        //        LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
        //        DataTable dt = linqToDataTableUtil.LinqToDataTable<SaveSummaryReportAction>(saveSummaryReportActions);

        //        using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
        //        {
        //            bcopy.BulkCopyTimeout = 120;
        //            bcopy.DestinationTableName = "dbo.SaveSummaryReportActionsTemp";
        //            SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("SummaryReportId", "SummaryReportId");
        //            SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("AzCampaignId", "AzCampaignId");
        //            SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("Negative", "Negative");
        //            SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("Promoted", "Promoted");
        //            SqlBulkCopyColumnMapping mapping8 = new SqlBulkCopyColumnMapping("CountryId", "CountryId");
        //            SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("SearchTerm", "SearchTerm");
        //            SqlBulkCopyColumnMapping mapping10 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");

        //            bcopy.ColumnMappings.Add(mapping2);
        //            bcopy.ColumnMappings.Add(mapping3);
        //            bcopy.ColumnMappings.Add(mapping4);
        //            bcopy.ColumnMappings.Add(mapping6);
        //            bcopy.ColumnMappings.Add(mapping8);
        //            bcopy.ColumnMappings.Add(mapping9);
        //            bcopy.ColumnMappings.Add(mapping10);
        //            bcopy.WriteToServer(dt);
        //        }

        //        using (var connection = new SqlConnection(DapperConnection.ConnectionString))
        //        {
        //            var affectedRows = await connection.ExecuteAsync("SaveSummaryReportActionsTempReconcile", new { @clientid = ClientId }, commandType: CommandType.StoredProcedure);
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Guid clientIdGuid = Guid.Empty;

        //        Logging logging = new Logging();
        //        LogError logError = new LogError();
        //        logError.ErrorMessage = ex.ToString();
        //        logError.FailureMethod = "SaveKeywordNegPos";
        //        logError.ClientId = clientIdGuid;
        //        logError.Parameters = JsonSerializer.Serialize(saveSummaryReportActions);
        //        logging.WriteToLog(logError);

        //        return false;
        //    }

        //}



        public async Task<bool> SaveKeywordNegPos(List<SaveSummaryReportAction> saveSummaryReportActions, Guid ClientId)
        {
            try
            {
                saveSummaryReportActions.ForEach(s => s.ClientId = ClientId);

                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<SaveSummaryReportAction>(saveSummaryReportActions);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.BulkCopyTimeout = 120;
                    bcopy.DestinationTableName = "[dbo].[AzSpSearchTermSummaryKManagementTemp]";
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("CountryId", "Country");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("keyword", "Keyword");
                    SqlBulkCopyColumnMapping mapping8 = new SqlBulkCopyColumnMapping("SearchTerm", "SearchTerm");
                    SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("AzCampaignId", "CampaignId");
                    SqlBulkCopyColumnMapping mapping12 = new SqlBulkCopyColumnMapping("keywordType", "KeywordType");
                    SqlBulkCopyColumnMapping mapping14 = new SqlBulkCopyColumnMapping("AdGroup", "AdGroupId");
                    SqlBulkCopyColumnMapping mapping15 = new SqlBulkCopyColumnMapping("Promoted", "Promoted");
                    SqlBulkCopyColumnMapping mapping16 = new SqlBulkCopyColumnMapping("Negative", "Negative");
                    SqlBulkCopyColumnMapping mapping17 = new SqlBulkCopyColumnMapping("KeywordId", "KeywordId");

                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping8);
                    bcopy.ColumnMappings.Add(mapping9);
                    bcopy.ColumnMappings.Add(mapping12);
                    bcopy.ColumnMappings.Add(mapping14);
                    bcopy.ColumnMappings.Add(mapping15);
                    bcopy.ColumnMappings.Add(mapping16);
                    bcopy.ColumnMappings.Add(mapping17);
                    bcopy.WriteToServer(dt);
                }

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("BulkAzSpSearchTermSummaryKManagementProcessing", new { @clientid = ClientId }, commandType: CommandType.StoredProcedure);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveKeywordNegPos";
                logError.ClientId = ClientId;
                logError.Parameters = JsonSerializer.Serialize(saveSummaryReportActions);
                await logging.WriteToLog(logError);

                return false;
            }

        }

    }
}
