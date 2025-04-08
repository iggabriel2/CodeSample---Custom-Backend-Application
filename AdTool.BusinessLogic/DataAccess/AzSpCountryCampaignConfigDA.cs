using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzSpCountryCampaignConfigDA
    {

        public async Task<List<AzSpCountryCampaignConfig>> GetAzSpCountryCampaignConfigProductId(int productId)
        {
            List<AzSpCountryCampaignConfig> list = new List<AzSpCountryCampaignConfig>();
            try
            {
                var sql = "SELECT * from AzSpCountryCampaignConfig where QAPProductId = @ProductId";
                var param = new { ProductId = productId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpCountryCampaignConfig>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCountryCampaignConfigProductId - AzSpCountryCampaignConfigDA.cs", "productId : " + productId);
            }
            return list;
        }

        public async Task<AzSpCountryCampaignConfig> GetAzSpCountryCampaignConfigByProductCountryClient(int productId, int country, Guid client, UIMessage errorMessage)
        {
            AzSpCountryCampaignConfig record = new AzSpCountryCampaignConfig();
            try
            {
                var sql = "SELECT * from AzSpCountryCampaignConfig where QAPProductId = @ProductId AND ClientId = @ClientId AND CountryId = @CountryId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ProductId", productId);
                queryParameters.Add("@CountryId", country);
                queryParameters.Add("@ClientId", client);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    record = (await connection.QueryAsync<AzSpCountryCampaignConfig>(sql, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                errorMessage.ErrorMessages.Add("An error occured while processing this request.");
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCountryCampaignConfigByProductCountryClient - AzSpCountryCampaignConfigDA.cs", "productId : " + productId + " country" + country + "client" + client);
            }
            return record;
        }

        public async Task<AzSpCountryCampaignConfig> GetAzSpCountryCampaignConfigByConfigIdClientId(int config, Guid client, UIMessage errorMessage)
        {
            AzSpCountryCampaignConfig record = new AzSpCountryCampaignConfig();
            try
            {
                var sql = "SELECT * from AzSpCountryCampaignConfig where ClientId = @ClientId AND AzSpCountryCampConfigId = @ConfigId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", client);
                queryParameters.Add("@ConfigId", config);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    record = (await connection.QueryAsync<AzSpCountryCampaignConfig>(sql, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                errorMessage.ErrorMessages.Add("An error occured while processing this request.");
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCountryCampaignConfigByConfigIdClientId - AzSpCountryCampaignConfigDA.cs", "config : " + config + " client" + client);
            }
            return record;
        }

        public async Task<int> InsertAzSpCountryCampaignConfig(Guid clientId, AzSpCountryCampaignConfig campaignConfig)
        {

            int configId = 0;
            try
            {
                var sql = "INSERT INTO AzSpCountryCampaignConfig (QAPProductId,CountryId,BiddingStrategyId,TopOfSearch,ProductPages, ResearchPortfolioId, UseTier1," +
                    "Tier1TresholdSales,Tier1TresholdPageReads,UsePerformance,PerformTresholdSales,PerformTresholdPageReads,ApplyNegative,ConversionGoal,Tier1DefaultBid, ResearchDefaultBid," +
                    "PerformanceDefBid, ExcludeAudibleKeywordsFromNegative, ClientId, Tier1DefaultBudget, ResearchDefaultBudget, PerformanceDefBudget, Tier1PortfolioId, PerformancePortfolioId, TargetACOS) " +
                    "VALUES (@QAPProductId, @CountryId, @BiddingStrategyId, @TopOfSearch,@ProductPages, @ResearchPortfolioId, @UseTier1," +
                    "@Tier1TresholdSales,@Tier1TresholdPageReads,@UsePerformance,@PerformTresholdSales,@PerformTresholdPageReads,@ApplyNegative,@ConversionGoal,@Tier1DefaultBid, @ResearchDefaultBid," +
                    "@PerformanceDefBid, @ExcludeAudibleKeywordsFromNegative, @ClientId, @Tier1DefaultBudget, @ResearchDefaultBudget, @PerformanceDefBudget, @Tier1PortfolioId, @PerformancePortfolioId, @TargetACOS);" +
                    " SELECT CAST(SCOPE_IDENTITY() as int)";

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    configId = await connection.QuerySingleAsync<int>(sql, campaignConfig);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "InsertAzSpCountryCampaignConfig - AzSpCountryCampaignConfigDA.cs",  JsonSerializer.Serialize(campaignConfig), clientId);
            }
            return configId;
        }

        public async Task<int> UpdateAzSpCountryCampaignConfig(AzSpCountryCampaignConfig campaignConfig)
        {

            int configId = 0;
            try
            {
                var sql = "UPDATE AzSpCountryCampaignConfig " +
                    "SET BiddingStrategyId = @BiddingStrategyId, " +
                    "TopOfSearch = @TopOfSearch, " +
                    "ProductPages = @ProductPages, " +
                    "ResearchPortfolioId = @ResearchPortfolioId, " +
                    "Tier1TresholdSales = @Tier1TresholdSales, " +
                    "Tier1TresholdPageReads = @Tier1TresholdPageReads, " +
                    "PerformTresholdSales = @PerformTresholdSales, " +
                    "PerformTresholdPageReads = @PerformTresholdPageReads, " +
                    "ApplyNegative = @ApplyNegative, " +
                    "ConversionGoal = @ConversionGoal, " +
                    "Tier1DefaultBid = @Tier1DefaultBid, " +
                    "ResearchDefaultBid = @ResearchDefaultBid, " +
                    "PerformanceDefBid = @PerformanceDefBid, " +
                    "ExcludeAudibleKeywordsFromNegative = @ExcludeAudibleKeywordsFromNegative, " +
                    "Tier1DefaultBudget = @Tier1DefaultBudget, " +
                    "ResearchDefaultBudget = @ResearchDefaultBudget, " +
                    "PerformanceDefBudget = @PerformanceDefBudget, " +
                    "PerformancePortfolioId = @PerformancePortfolioId, " +
                    "Tier1PortfolioId = @Tier1PortfolioId, " +
                    "TargetACOS = @TargetACOS " +
                    "WHERE AzSpCountryCampConfigId = @AzSpCountryCampConfigId AND @ClientId = @ClientId";

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                  await connection.ExecuteAsync(sql, campaignConfig);
                }
            }
            catch (Exception ex)
            {
                configId = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAzSpCountryCampaignConfig - AzSpCountryCampaignConfigDA.cs", JsonSerializer.Serialize(campaignConfig));
            }
            return configId;
        }

        public async Task<int> UpdateAzSpCountryCampaignConfigFromCampaignCreation(AzSpCountryCampaignConfig campaignConfig)
        {

            int configId = 0;
            try
            {
                var sql = "UPDATE AzSpCountryCampaignConfig " +
                    "SET Tier1TresholdSales = @Tier1TresholdSales, " +
                    "Tier1TresholdPageReads = @Tier1TresholdPageReads, " +
                    "PerformTresholdSales = @PerformTresholdSales, " +
                    "PerformTresholdPageReads = @PerformTresholdPageReads " +
                    "WHERE AzSpCountryCampConfigId = @AzSpCountryCampConfigId AND @ClientId = @ClientId";

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, campaignConfig);
                }
            }
            catch (Exception ex)
            {
                configId = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAzSpCountryCampaignConfigFromCampaignCreation - AzSpCountryCampaignConfigDA.cs", JsonSerializer.Serialize(campaignConfig));
            }
            return configId;
        }
    }
}
