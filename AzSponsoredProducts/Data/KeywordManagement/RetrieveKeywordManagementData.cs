using Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Logging;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Save;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.View;

namespace AdTool.AzSponsoredProducts.Data.KeywordManagement
{
    public class RetrieveKeywordManagementData
    {
        public async Task<List<AllSearchTerms>> GetAllSearchTerms(Guid ClientId, DateTime CreationDate)
        {
            List<AllSearchTerms> AllSearchTerms = new List<AllSearchTerms>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    AllSearchTerms = (await connection.QueryAsync<AllSearchTerms>("GetKeywordsForUpdating", new { @ClientId = ClientId, @CreationDate =  CreationDate}, commandType: CommandType.StoredProcedure)).ToList();
                }
                return AllSearchTerms;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAllSearchTerms";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "none";
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<CampaignProductRelationships>> GetCampaignProductRelationships(Guid ClientId)
        {
            List<CampaignProductRelationships> CampaignProductRelationships = new List<CampaignProductRelationships>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    CampaignProductRelationships = (await connection.QueryAsync<CampaignProductRelationships>("GetCampaignProductRelationships", new { @clientId = ClientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return CampaignProductRelationships;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCampaignProductRelationships";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "none";
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<PromoNegativeRules>> GetPromoNegativeRules(Guid ClientId)
        {
            List<PromoNegativeRules> PromoNegativeRules = new List<PromoNegativeRules>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    PromoNegativeRules = (await connection.QueryAsync<PromoNegativeRules>("GetPromoNegativeRules", new { @clientId = ClientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return PromoNegativeRules;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetPromonegativeRules";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "none";
                await logging.WriteToLog(logError);

                return null;
            }
        }
        public async Task<bool> GetCampaignActiveStatus(Guid ClientId, string CampaignId, int CountryId)
        {
            bool active = false;
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    active = (await connection.QueryAsync<bool>("GetCampaignActiveStatus", new { @clientId = ClientId, @campaignId = CampaignId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return active;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCampaignActiveStatus";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString() + ", " + CampaignId;
                await logging.WriteToLog(logError);

                return false;
            }
        }

        public async Task<List<KeywordPerformanceByMonth>> GetKeywordPerformanceData(Guid ClientId, DateTime? StartDate, DateTime? EndDate)
        {
            List<KeywordPerformanceByMonth> keywordPerformanceByMonths = new List<KeywordPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<KeywordPerformanceByMonth>("GetKeywordPerformanceData", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordPerformanceData";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                throw ex;
            }
        }

        public async Task<List<KeywordPerformanceByMonth>> GetKeywordPerformanceDataInCountry(Guid ClientId, int CountryId, DateTime? StartDate, DateTime? EndDate)
        {
            List<KeywordPerformanceByMonth> keywordPerformanceByMonths = new List<KeywordPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<KeywordPerformanceByMonth>("GetKeywordPerformanceDataInCountry", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordPerformanceDataInCountry";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                throw ex;
            }
        }

        public async Task<List<KeywordPerformanceByMonth>> GetKeywordPerformanceDataForCampaigns(Guid ClientId, DateTime? StartDate, DateTime? EndDate)
        {
            List<KeywordPerformanceByMonth> keywordPerformanceByMonths = new List<KeywordPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<KeywordPerformanceByMonth>("GetKeywordPerformanceDataForCampaigns", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordPerformanceDataForCampaigns";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                throw ex;
            }
        }

        public async Task<List<KeywordPerformanceByMonth>> GetKeywordPerformanceDataForCampaignsInCountry(Guid ClientId, int CountryId, DateTime? StartDate, DateTime? EndDate)
        {
            List<KeywordPerformanceByMonth> keywordPerformanceByMonths = new List<KeywordPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<KeywordPerformanceByMonth>("GetKeywordPerformanceDataForCampaignsInCountry", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate, @CountryID = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordPerformanceDataForCampaignsInCountry";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                throw ex;
            }
        }

        public async Task<List<KeywordPerformanceByMonth>> GetKeywordDataByAdGroup(Guid ClientId, DateTime? StartDate, DateTime? EndDate, string AdGroup, int CountryId)
        {
            List<KeywordPerformanceByMonth> keywordPerformanceByMonths = new List<KeywordPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<KeywordPerformanceByMonth>("GetKeywordDataByAdGroup", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate, @AdGroup = AdGroup, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordDataByAdGroup";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                throw ex;
            }
        }

        public async Task<List<KeywordPerformanceByMonth>> GetKeywordPerformanceDataByAdGroup(Guid ClientId, string CampaignId, DateTime? StartDate, DateTime? EndDate, int CountryId)
        {
            List<KeywordPerformanceByMonth> keywordPerformanceByMonths = new List<KeywordPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<KeywordPerformanceByMonth>("GetKeywordPerformanceDataByAdGroup", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate, @CampaignId = CampaignId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordPerformanceDataByAdGroup";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString() + " " + CampaignId;
                await logging.WriteToLog(logError);

                throw ex;
            }

        }

        public async Task<List<KeywordPerformanceByCampaign>> GetKeywordPerformanceByCampaign(int? countryId, string? campaignName, int? campaignStatus, int? productId, DateTime? dateFrom, DateTime? dateTo, int? campaignUsage, Guid? clientId)
        {
            List<KeywordPerformanceByCampaign> list = new List<KeywordPerformanceByCampaign>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<KeywordPerformanceByCampaign>("GetAzSpCampaignSummaryGridList ", new { @ClientId = clientId , @CountryId  = countryId, @ProductId  = productId , @CampaignStatus = campaignStatus , @CampaignUsage = campaignUsage, @DateFrom = dateFrom, @DateTo = dateTo, @CampaignName = campaignName }, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCampaignSummaryGridList - AzSpViewDA.cs", "countryId : " + countryId + " campaignName: " + campaignName + "campaignStatus : " + campaignStatus + "productId :" + productId +
                    " dateFrom :" + dateFrom + " dateTo : " + dateTo + " campaignUsage :" + campaignUsage + " clientId : " + clientId);               
            }

            return list;
        }

        public async Task<List<SearchTermPerformanceByMonth>> GetSearchTermPerformanceData(Guid ClientId, DateTime? StartDate, DateTime? EndDate)
        {
            List<SearchTermPerformanceByMonth> keywordPerformanceByMonths = new List<SearchTermPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<SearchTermPerformanceByMonth>("GetSearchTermPerformanceData", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetSearchTermPerformanceData";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                throw ex;
            }

        }

        public async Task<List<SearchTermPerformanceByMonth>> GetSearchTermPerformanceDataByCountry(Guid ClientId, int CountryId, DateTime? StartDate, DateTime? EndDate)
        {
            List<SearchTermPerformanceByMonth> keywordPerformanceByMonths = new List<SearchTermPerformanceByMonth>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordPerformanceByMonths = (await connection.QueryAsync<SearchTermPerformanceByMonth>("GetSearchTermPerformanceDataByCountry", new { @clientId = ClientId, @StartDate = StartDate, @EndDate = EndDate, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return keywordPerformanceByMonths;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetSearchTermPerformanceDataByCountry";
                logError.ClientId = ClientId;
                logError.Parameters = ClientId.ToString();
                await logging.WriteToLog(logError);

                throw ex;
            }

        }
    }

}
