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
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class CountriesDA
    {

        public async Task<List<Countries>> GetListOfCountries()
        {
            List<Countries> list = new List<Countries>();
            try
            {
                var sql = "Select * from Countries";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<Countries>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetListOfCountries - CountriesDA.cs", string.Empty);
            }

            return list;
        }

        public async Task<List<Countries>> GetAzSpCountriesByClientId(Guid clientId)
        {
            List<Countries> list = new List<Countries>();
            try
            {
                var sql = "Select c.Id, c.Country from Countries c inner join AzSpClientProfileCodes codes on c.Id = codes.CountryId where codes.ClientId = @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<Countries>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCountriesByClientId - CountriesDA.cs", "clientid : " + clientId);
            }

            return list;
        }
        public async Task<Countries> GetCountryByCountryId(int countryId)
        {
            Countries country = new Countries();
            try
            {
                var sql = "Select * from Countries where Id = @CountryId";
                var param = new { CountryId = countryId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    country = (await connection.QueryAsync<Countries>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetCountryByCountryId - CountriesDA.cs", "countryId : " + countryId);
            }

            return country;
        }
    }
}
