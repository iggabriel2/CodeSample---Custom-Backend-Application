using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
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

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzSpProductDA
    {
        public async Task<AzSpProduct> GetAzSpProductByClientIdProductId(Guid clientId, int productId)
        {
            AzSpProduct product = new AzSpProduct();
            try
            {
                var sqlStatement = "Select * from AzSpProduct where ClientId = @ClientId and QAPProductId = @ProductId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", clientId);
                queryParameters.Add("@ProductId", productId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    product = (await connection.QueryAsync<AzSpProduct>(sqlStatement, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductByClientIdProductId - AzSpProductDA.cs", "productId : " + productId + " clientId : " + clientId);
            }
            return product;
        }

        public async Task<AzSpProduct> GetAzSpProductByProductId(int productId)
        {
            AzSpProduct product = new AzSpProduct();
            try
            {
                var sqlStatement = "Select * from AzSpProduct where QAPProductId = @ProductId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ProductId", productId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    product = (await connection.QueryAsync<AzSpProduct>(sqlStatement, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductByProductId - AzSpProductDA.cs", "productId : " + productId);
            }
            return product;
        }

        public async Task<int> SaveAzSpProduct(AzSpProduct product)
        {
            int productId = 0;
            try
            {
                var sql = "Insert into AzSpProduct (Asin, ProductName, Active, ClientId, AzProductName, AzImageURL,Author) values (@Asin, @ProductName, @Active, @ClientId, @AzProductName, @AzImageURL, @Author); SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    productId = await connection.QuerySingleAsync<int>(sql, product);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SaveAzSpProduct - AzSpProductDA.cs", JsonSerializer.Serialize(product));
            }
            return productId;
        }

        public async Task UpdateAzSpProductName(int productId, string productName, Guid clientId)
        {
            try
            {
                var sql = "UPDATE AzSpProduct SET ProductName = @ProductName OUTPUT INSERTED.QAPProductId WHERE QAPProductId = @ProductId AND ClientId = @ClientId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ProductId", productId);
                queryParameters.Add("@ProductName", productName);
                queryParameters.Add("@ClientId", clientId);

                int? validateQuery = null;
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    validateQuery = await connection.QuerySingleAsync<int>(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateAzSpProductName - AzSpProductDA.cs", "productId : " + productId + " productName : " + productName  + " clientId :" + clientId);
            }
        }

        public async Task<List<AzSpProduct>> GetAzSpProductListByClientIdCountryId(Guid clientId, int countryId)
        {
            List<AzSpProduct> productList = new List<AzSpProduct>();
            try
            {
                var sqlStatement = "Select * from AzSpProduct where ClientId = @ClientId and CountryId = @CountryId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", clientId);
                queryParameters.Add("@CountryId", countryId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    productList = (await connection.QueryAsync<AzSpProduct>(sqlStatement, queryParameters)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductListByClientIdCountryId - AzSpProductDA.cs", "clientId :" + clientId + " countryId : " + countryId);
            }
            return productList;
        }

        public async Task<List<AzSpProduct>> GetAzSpProductListByClientId(Guid clientId)
        {
            List<AzSpProduct> productList = new List<AzSpProduct>();
            try
            {
                var sqlStatement = "Select * from AzSpProduct where ClientId = @ClientId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", clientId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    productList = (await connection.QueryAsync<AzSpProduct>(sqlStatement, queryParameters)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpProductListByClientId - AzSpProductDA.cs", "clientId :" + clientId);
            }
            return productList;
        }
    }
}
