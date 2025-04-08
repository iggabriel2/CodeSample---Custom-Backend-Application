using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using AdTool.Entities.View;
using AdTool.WebUI.Models.AzSp.ProductConfig;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzSpViewDA
    {

        #region AzSpCountryCampaignConfigView
        public async Task<AzSpCountryCampaignConfigView> GetAzSpCountryCampaignConfigViewByConfigIdClientId(int configId, Guid clientId, UIMessage errorMessage)
        {
            AzSpCountryCampaignConfigView record = new AzSpCountryCampaignConfigView();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    record = (await connection.QueryAsync<AzSpCountryCampaignConfigView>("GetAzSpCountryCampaignConfigViewByConfigIdClientId", new { @ClientId = clientId, @ConfigId = configId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                errorMessage.ErrorMessages.Add("Unable to get the config information");
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCountryCampaignConfigViewByConfigIdClientId - AzSpViewDA.cs", "configId : " + configId + " clientId : " + clientId);
            }
            return record;
        }

        public async Task<AzSpCountryCampaignConfigView> GetAzSpCountryCampaignConfigViewByClientProductCountry(int productId, int countryId, Guid clientId, UIMessage errorMessage)
        {
            AzSpCountryCampaignConfigView record = new AzSpCountryCampaignConfigView();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    record = (await connection.QueryAsync<AzSpCountryCampaignConfigView>("GetAzSpCountryCampaignConfigViewByClientCountryProduct", new { @ClientId = clientId, @CountryId = countryId, @ProductId = productId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                errorMessage.ErrorMessages.Add("Unable to get the config information");
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCountryCampaignConfigViewByClientProductCountry - AzSpViewDA.cs", "productId : " + productId + " clientId : " + clientId + " countryId : " + countryId);
            }
            return record;
        }

        #endregion AzSpCountryCampaignConfigView

        #region AzSpClientProfileCodes
        public async Task<List<Countries>> GetAzSpClientProfileCodesByClientId(Guid clientId)
        {
            List<Countries> list = new List<Countries>();
            try
            {
                var sql = "SELECT codes.CountryId as Id, c.Country FROM AzSpClientProfileCodes codes JOIN Countries c ON codes.CountryId = c.Id WHERE ClientId= @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<Countries>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpClientProfileCodesByClientId - AzSpViewDA.cs", " clientId : " + clientId);
            }
            return list;
        }
        #endregion

        #region AzSpProductView

        public async Task<List<AzSpProductView>> GetAzSpProductViewByClientIdProductId(Guid clientId, int productId)
        {
            List<AzSpProductView> list = new List<AzSpProductView>();
            try
            {
                var sql = "SELECT product.QAPProductId, product.ProductName, country.CountryId, c.Country " +
                    "FROM AzSpProduct product " +
                    "JOIN AzSpProductCountry country on product.QAPProductId =  country.QAPProductId " +
                    "JOIN Countries c on country.CountryId = c.Id " +
                    "WHERE product.ClientId = @ClientId AND product.QAPProductId = @ProductId";
                var param = new DynamicParameters();
                param.Add("@ProductId", productId);
                param.Add("@ClientId", clientId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpProductView>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductViewByClientIdProductId - AzSpViewDA.cs", " clientId : " + clientId + " productId : " + productId);
            }
            return list;
        }
        #endregion

        #region AzClientSavedKeywordsView

        public async Task<List<AzClientSavedKeywordsView>> GetAzClientSavedSearchesViewByClientId(Guid clientId, string type)
        {
            List<AzClientSavedKeywordsView> list = new List<AzClientSavedKeywordsView>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzClientSavedKeywordsView>("GetAzClientSavedKeywordsByType", new { @ClientId = clientId, @Type= type }, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzClientSavedKeywordsViewByClientId - AzSpViewDA.cs", " clientId : " + clientId);
            }
            return list;
        }

        #endregion

        #region AzSpClientView
        public async Task<AzSpUserView> GetAzSpUserViewByUserId(int userId)
        {
            AzSpUserView list = new AzSpUserView();
            try
            {
                var sql = "SELECT usr.Id,  " +
                    "usr.UserName,  " +
                    "usr.EmailConfirmed,  " +
                    "usr.SubscriptionAmount,  " +
                    "usr.PaymentSchedule,  " +
                    "usr.FirstName,  " +
                    "usr.LastName,  " +
                    "usr.FreeTrial, " +
                    "usr.PromoCode, " +
                    "pln.PlanName, " +
                    "usr.Email, " +
                    "info.SubscriptionStatus, " +
                    "info.CancellationDate, " +
                    "info.CCLastFour " +
                    "FROM AppUser usr " +
                    "JOIN PaymentPlans pln on usr.PaymentPlan = pln.id " +
                    "JOIN AppUserPaymentInfo info ON usr.Id = info.AppUserId " +
                    "WHERE usr.Id = @UserId";
                var param = new DynamicParameters();
                param.Add("@UserId", userId);

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpUserView>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpUserViewByClientId - AzSpViewDA", "userId : " + userId);
            }
            return list;
        }

        #endregion

        #region ActionRequiredView
        public async Task<List<ActionRequiredView>> GetActionRequiredGridList(Guid clientId, int? countryId, int? records)
        {
            List<ActionRequiredView> list = new List<ActionRequiredView>();
            try
            {
                       
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<ActionRequiredView>("GetActionRequiredGridList", new { @ClientId = clientId, @CountryId = countryId, @Records = records }, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetActionRequiredGridList - AzSpViewDA.cs", "clientId : " + clientId + "countryId: " + countryId + " records : " + records);
            }
            return list;
        }
        #endregion ActionRequiredView

        #region AzSpProductSummaryByCountryView
        public async Task<(List<AzSpProductSummaryByCountryView>, int)> GetAzSpProductSummaryByCountryView(Guid clientId, int productId)
        {
            List<AzSpProductSummaryByCountryView> list = new List<AzSpProductSummaryByCountryView>();
            int result = 0;

            try
            {

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpProductSummaryByCountryView>("GetAzSpProductSummaryByCountry", new { @ClientId = clientId, @ProductId = productId }, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductSummaryByCountry - AzSpViewDA.cs", "clientId : " + clientId + "productId: " + productId);
            }
            return (list, result);
        }
        #endregion AzSpProductSummaryByCountryView

        #region 
        public async Task<List<QAPPieChartModel>> GetTopSearchTermsByClicks(Guid clientId)
        {
            List<QAPPieChartModel> list = new List<QAPPieChartModel>();
            try
            {

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<QAPPieChartModel>("GetTopSearchTermsByClicks", new { @ClientId = clientId}, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetTopSearchTermsByClicks - AzSpViewDA.cs", "clientId : " + clientId);
            }
            return list;
        }

        public async Task<List<QAPPieChartModel>> GetTopSearchTermsByOrders(Guid clientId)
        {
            List<QAPPieChartModel> list = new List<QAPPieChartModel>();
            try
            {

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<QAPPieChartModel>("GetTopSearchTermsByOrders", new { @ClientId = clientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetTopSearchTermsByOrders - AzSpViewDA.cs", "clientId : " + clientId);
            }
            return list;
        }

        public async Task<AzSpSnapshot> GetSnapshotData(Guid clientId, int userId)
        {
            AzSpSnapshot record = new AzSpSnapshot();
            try
            {

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    record = (await connection.QueryAsync<AzSpSnapshot>("GetSnapshotData", new { @ClientId = clientId , @userId = userId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetSnapshotData - AzSpViewDA.cs", "clientId : " + clientId + " userid: " + userId);
            }
            return record;
        }

        #endregion
    }
}
