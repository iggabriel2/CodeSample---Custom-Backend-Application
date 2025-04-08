using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.Logging;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Data.ReportData
{
    public class RetrieveNightlyExtras
    {
        public async Task<List<DisabledCampaign>> GetDisabledPerformanceCampaigns(Guid clientId)
        {
            List<DisabledCampaign> disabledCampaigns = new List<DisabledCampaign>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    disabledCampaigns = (await connection.QueryAsync<DisabledCampaign>("GetDisabledPerformanceCampaigns", new { @clientId = clientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return disabledCampaigns;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetDisabledPerformanceCampaigns";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "client id";
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<NewCampaignsToAssign>> GetUnassignedCampaigns(Guid clientId)
        {
            List<NewCampaignsToAssign> unassignedCampaigns = new List<NewCampaignsToAssign>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    unassignedCampaigns = (await connection.QueryAsync<NewCampaignsToAssign>("GetNewCampaignsToAssign", new { @clientId = clientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return unassignedCampaigns;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetUnassignedCampaigns";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "client id";
                await logging.WriteToLog(logError);

                return null;
            }
        }


        public async Task<List<KeywordDetailsForCosmos>> GetKeywordDetailsForCosmos(Guid ClientId, int CountryId)
        {
            List<KeywordDetailsForCosmos> keywordDetailsForCosmos = new List<KeywordDetailsForCosmos>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordDetailsForCosmos = (await connection.QueryAsync<KeywordDetailsForCosmos>("GetKeywordDetailsForCosmos", new { @ClientId = ClientId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordDetailsForCosmos;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordDetailsForCosmos";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "client id: " + ClientId.ToString() + " , countryid: " + CountryId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<KeywordNegativesForCosmos>> GetKeywordNegativesForCosmos(Guid ClientId, int CountryId)
        {
            List<KeywordNegativesForCosmos> keywordNegativesForCosmos = new List<KeywordNegativesForCosmos>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordNegativesForCosmos = (await connection.QueryAsync<KeywordNegativesForCosmos>("GetKeywordNegativesForCosmos", new { @ClientId = ClientId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordNegativesForCosmos;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordNegativesForCosmos";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "client id: " + ClientId.ToString() + " , countryid: " + CountryId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<KeywordNegativesForCosmos>> GetKeywordNegativesForCosmosAll(Guid ClientId)
        {
            List<KeywordNegativesForCosmos> keywordNegativesForCosmos = new List<KeywordNegativesForCosmos>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordNegativesForCosmos = (await connection.QueryAsync<KeywordNegativesForCosmos>("GetKeywordNegativesForCosmosAll", new { @ClientId = ClientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordNegativesForCosmos;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordNegativesForCosmosAll";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "client id: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }
    }
}
