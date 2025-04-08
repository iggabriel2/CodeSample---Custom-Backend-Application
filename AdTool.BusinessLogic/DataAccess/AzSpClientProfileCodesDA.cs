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
    public class AzSpClientProfileCodesDA
    {
        public async Task<List<AzSpClientProfileCodes>> GetAzSpClientProfileCodesByClientId(Guid clientId)
        {
            List<AzSpClientProfileCodes> list = new List<AzSpClientProfileCodes>();
            try
            {
                var sql = "SELECT * FROM AzSpClientProfileCodes WHERE ClientId= @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpClientProfileCodes>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpClientProfileCodesByClientId - AzSpClientProfileCodesDA.cs", "clientId : " + clientId);
            }
            return list;
        }

        public async Task<List<Countries>> GetAzSpClientProfileCodesWDescriptionByClientId(Guid clientId)
        {
            List<Countries> list = new List<Countries>();
            try
            {
                var sql = "SELECT codes.CountryId as Id, Countries.Country " +
                    "FROM AzSpClientProfileCodes codes " +
                    "JOIN Countries ON codes.CountryId = Countries.Id " +
                    "WHERE codes.ClientId = @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<Countries>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpClientProfileCodesWDescriptionByClientId - AzSpClientProfileCodesDA.cs", "clientId : " + clientId);
            }
            return list;
        }
    }
}
