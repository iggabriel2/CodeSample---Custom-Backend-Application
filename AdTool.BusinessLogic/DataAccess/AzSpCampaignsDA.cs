using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzSpCampaignsDA
    {
        public async Task<int> UpdateAzSpCampaignsProductId(Guid clientId, string campaignId, int countryId, int productId)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AzSpCampaigns " +
                    "SET ProductId = @ProductId " +
                    "WHERE AZCampaignId = @AZCampaignId " +
                    "AND azClientId = @azClientId " +
                    "AND CountryId = @CountryId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@AZCampaignId", campaignId);
                queryParameters.Add("@azClientId", clientId);
                queryParameters.Add("@CountryId", countryId);
                queryParameters.Add("@ProductId", productId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAzSpCampaignsProductId - AzSpCampaignsDA.cs", "clientId : " + clientId + " , campaignid : " + campaignId + " countryid : " + countryId + " productId: " + productId);
            }
            return result;
        }

        public async Task<int> UpdateAzSpCampaignsDetails(AzSpCampaigns campaign)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AzSpCampaigns " +
                    "SET Budget = @Budget, " +
                    "CampaignName = @CampaignName, " +
                    "State = @State, " +
                    "DynamicBiddingStrategy = @DynamicBiddingStrategy " +
                    "WHERE Id = @Id " +
                    "AND azClientId = @azClientId " +
                    "AND CountryId = @CountryId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", campaign.Id);
                queryParameters.Add("@azClientId", campaign.azClientId);
                queryParameters.Add("@CountryId", campaign.CountryId);
                queryParameters.Add("@Budget", campaign.Budget);
                queryParameters.Add("@DynamicBiddingStrategy", campaign.DynamicBiddingStrategy);
                queryParameters.Add("@CampaignName", campaign.CampaignName);
                queryParameters.Add("@State", campaign.State);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAzSpCampaignsDetails - AzSpCampaignsDA.cs", JsonSerializer.Serialize(campaign));
            }
            return result;
        }

        public async Task<int> UpdateAzSpCampaignsKeywordManagement(Guid clientId, string campaignId, int countryId, bool include)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AzSpCampaigns " +
                    "SET IncludeInKeywordManagement = @IncludeInKeywordManagement " +
                    "WHERE AZCampaignId = @AZCampaignId " +
                    "AND azClientId = @azClientId " +
                    "AND CountryId = @CountryId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@AZCampaignId", campaignId);
                queryParameters.Add("@azClientId", clientId);
                queryParameters.Add("@CountryId", countryId);
                queryParameters.Add("@Client", clientId);
                queryParameters.Add("@IncludeInKeywordManagement", include);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAzSpCampaignsKeywordManagement - AzSpCampaignsDA.cs", "clientId : " + clientId + " , campaignid : " + campaignId + " countryid : " + countryId + " include: " + include);
            }
            return result;
        }

        public async Task<AzSpCampaigns> GetAzSpCampaignByIdClientId(Guid clientId, int id)
        {
            AzSpCampaigns campaign = new AzSpCampaigns();
            try
            {
                var sqlStatement = "SELECT * FROM AzSpCampaigns where AzClientId = @ClientId and Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", clientId);
                queryParameters.Add("@Id", id);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    campaign = (await connection.QueryAsync<AzSpCampaigns>(sqlStatement, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetActionRequiredById - ActionRequiredDA.cs", " clientId : " + clientId + " id: " + id);
            }
            return campaign;
        }

    }
}
