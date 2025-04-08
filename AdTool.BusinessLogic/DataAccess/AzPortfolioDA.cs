using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzPortfolioDA
    {
        public async Task<List<AzPortfolio>> GetAzPortfolioListByAccountId(Guid clientId)
        {
            List<AzPortfolio> list = new List<AzPortfolio>();
            try
            {
                var sql = "SELECT * FROM AzPortfolio WHERE ClientId= @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzPortfolio>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzPortfolioListByAccountId - AzPortfolioDA.cs", "clientId : " + clientId);
            }
            return list;
        }

        public async Task<List<AzPortfolio>> GetAzPortfolioListByClientIdCountryId(Guid clientId, int countryId)
        {
            List<AzPortfolio> list = new List<AzPortfolio>();
            try
            {
                var sql = "SELECT * FROM AzPortfolio WHERE ClientId= @ClientId AND CountryId = @CountryId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", clientId);
                queryParameters.Add("@CountryId", countryId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzPortfolio>(sql, queryParameters)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzPortfolioListByClientIdCountryId - AzPortfolioDA.cs", "clientId : " + clientId + "countryId : " + countryId);
            }
            return list;
        }

        public async Task<AzPortfolio> GetAzPortfolioListByPortfolioId(int portfolioId)
        {
           AzPortfolio list = new AzPortfolio();
            try
            {
                var sql = "SELECT * FROM AzPortfolio WHERE Id= @PortfolioId";
                var param = new { PortfolioId = portfolioId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzPortfolio>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzPortfolioListByPortfolioId - AzPortfolioDA.cs", "portfolioId : " + portfolioId);
            }
            return list;
        }
    }
}
