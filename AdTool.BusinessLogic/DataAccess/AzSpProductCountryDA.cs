using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzSpProductCountryDA
    {
        public async Task<List<AzSpProductCountry>> GetAzSpProductCountryByProductId(int productId)
        {
            List<AzSpProductCountry> list = new List<AzSpProductCountry>();
            try
            {
                var sql = "Select * from AzSpProductCountry where QAPProductId = @ProductId";
                var param = new { ProductId = productId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpProductCountry>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductCountryByProductId - AzSpProductCountryDA.cs", "productId : " + productId);
            }
            return list;
        }

        public async Task<AzSpProductCountry> GetAzSpProductCountryByProductIdCountryId(int productId, int countryId)
        {
            AzSpProductCountry record = new AzSpProductCountry();
            try
            {
                var sql = "Select * from AzSpProductCountry where QAPProductId = @ProductId AND CountryId = @CountryId";
                var param = new DynamicParameters();
                param.Add("@ProductId", productId);
                param.Add("@CountryId", countryId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    record = (await connection.QueryAsync<AzSpProductCountry>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductCountryByProductIdCountryId - AzSpProductCountryDA.cs", "productId : " + productId);
            }
            return record;
        }

        public async Task<List<Countries>> GetAvailbleAzSpProductCountryByProductId(int productId)
        {
            List<Countries> list = new List<Countries>();
            try
            {
                var sql = "SELECT a.CountryId as Id, b.Country from AzSpProductCountry a JOIN Countries b ON a.CountryId = b.Id where a.QAPProductId = @ProductId";
                var param = new { ProductId = productId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<Countries>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAvailbleAzSpProductCountryByProductId - AzSpProductCountryDA.cs", "productId : " + productId);
            }
            return list;
        }

        public async Task<List<Countries>> GetConfiguredAzSpProductCountryByProductId(int productId)
        {
            List<Countries> list = new List<Countries>();
            try
            {
                var sql = "SELECT config.CountryId, c.Country from AzSpCountryCampaignConfig config JOIN Countries c ON config.CountryId = c.Id where config.QAPProductId = @ProductId";
                var param = new { ProductId = productId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<Countries>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetConfiguredAzSpProductCountryByProductId - AzSpProductCountryDA.cs", "productId : " + productId);
            }
            return list;
        }

        public async Task SaveAzSpProductCountry(AzSpProductCountry product)
        {
            try
            {
                var sql = "Insert into AzSpProductCountry (QAPProductId, CountryId) values (@QAPProductId, @CountryId)";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, product);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SaveAzSpProductCountry - AzSpProductCountryDA.cs", JsonSerializer.Serialize(product));
            }
        }
    }
}
