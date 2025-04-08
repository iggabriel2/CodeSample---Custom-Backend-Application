using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Edit.Auth;
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
using System.Xml;

namespace AdTool.AzSponsoredProducts.Data.ReportData
{
    public class RetrieveReportData
    {
        public async Task<MonthlyReportSettings> GetDaysInMonthToChekLastMonthlyReport()
        {
            MonthlyReportSettings monthlyReportSettings = new MonthlyReportSettings();
            monthlyReportSettings.DaysInMonthToProcess = 7;
            monthlyReportSettings.ParallelSettings = 30;

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    monthlyReportSettings = (await connection.QueryAsync<MonthlyReportSettings>("GetDaysInMonthToChekLastMonthlyReport", commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return monthlyReportSettings;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetDaysInMonthToChekLastMonthlyReport";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "none";
                await logging.WriteToLog(logError);

                monthlyReportSettings.DaysInMonthToProcess = 7;
                monthlyReportSettings.ParallelSettings = 30;

                return monthlyReportSettings;
            }
        }

        public async Task<List<ReportUser>> GetAllReportUsers()
        {
            List<ReportUser> reportUsers = new List<ReportUser>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    reportUsers = (await connection.QueryAsync<ReportUser>("GetAllReportUsers", commandType: CommandType.StoredProcedure)).ToList();
                }
                return reportUsers;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAllReportUsers";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "none";
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<ReportUser>> GetSpecificReportUser(Guid? ClientId)
        {
            List<ReportUser> reportUsers = new List<ReportUser>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    reportUsers = (await connection.QueryAsync<ReportUser>("GetSpecificReportUser", new { @ClientId = ClientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return reportUsers;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetSpecificReportUser";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "ClientId: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<ClientProfileCodes>> GetProfileCodes(Guid ClientId)
        {
            List<ClientProfileCodes> profileCodes = new List<ClientProfileCodes>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    profileCodes = (await connection.QueryAsync<ClientProfileCodes>("GetProfileCodes", new { @clientId = ClientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return profileCodes;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetProfileCodes";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<ReportLoggingByClient>> GetReportLoggingByClient(Guid clientId)
        {
            List<ReportLoggingByClient> reportLogging = new List<ReportLoggingByClient>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    reportLogging = (await connection.QueryAsync<ReportLoggingByClient>("GetReportLoggingByClient", new { @clientid = clientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return reportLogging;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetReportLoggingByClient";
                logError.ClientId = clientId;
                logError.Parameters = "none";
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<ReconcileHistory>> GetCampaignsToReconcile(Guid clientId)
        {
            List<ReconcileHistory> reconcileHistory = new List<ReconcileHistory>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    reconcileHistory = (await connection.QueryAsync<ReconcileHistory>("GetCampaignsToReconcile", new { @clientid = clientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return reconcileHistory;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCampaignsToReconcile";
                logError.ClientId = clientId;
                logError.Parameters = "none";
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<ProductValueForReport> GetProductName(ReconcileHistory reconcileHistory)
        {
            try
            {
                ProductValueForReport productValueForReport = new ProductValueForReport();

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    productValueForReport = (await connection.QueryAsync<ProductValueForReport>("GetProductName", reconcileHistory, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return productValueForReport;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetProductName", JsonSerializer.Serialize(reconcileHistory));
                return null;
            }
        }

        public async Task<List<string>> GetKeywordIdsWithActivity(Guid clientId, int CountryId)
        {
            List<string> keywordIds = new List<string>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordIds = (await connection.QueryAsync<string>("GetKeywordIdsWithActivity", new { @clientid = clientId, @Country = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordIds;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordIdsWithActivity";
                logError.ClientId = clientId;
                logError.Parameters = "Client Id and Country Id: " + CountryId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }
        public async Task<List<string>> GetKeywordIdsWithActivityForAG(Guid clientId, int CountryId)
        {
            List<string> keywordIds = new List<string>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordIds = (await connection.QueryAsync<string>("GetKeywordIdsWithActivityForAG", new { @clientid = clientId, @Country = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordIds;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordIdsWithActivityForAG";
                logError.ClientId = clientId;
                logError.Parameters = "Client Id and Country Id: " + CountryId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<BillingTotals>> GetOverageAmounts(Guid ClientId)
        {
            List<BillingTotals> overageAmounts = new List<BillingTotals>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    overageAmounts = (await connection.QueryAsync<BillingTotals>("GetBillingTotals", new { @ClientId = ClientId }, commandTimeout: 120, commandType: CommandType.StoredProcedure)).ToList();
                }
                return overageAmounts;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetOverageAmounts";
                logError.Parameters = "NA";
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<OverageAmounts> GetLastMonthTotalSpend(int AppUserId, DateTime BillingMonth)
        {
            OverageAmounts overageAmounts = new OverageAmounts();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    overageAmounts = (await connection.QueryAsync<OverageAmounts>("GetLastMonthTotalSpend", new { @AppUserId = AppUserId, @BillingMonth = BillingMonth }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return overageAmounts;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetLastMonthTotalSpend";
                logError.Parameters = "NA";
                await logging.WriteToLog(logError);

                return null;
            }
        }
    }
}
