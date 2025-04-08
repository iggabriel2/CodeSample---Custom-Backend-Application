using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.Edit;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzSpNegativeDefaultsDA
    {
        public async Task<int> SaveAzSpNegativeDefaults(AzSpNegativeDefaults entity)
        {
            int entityId = 0;

            try
            {
                var sql = "Insert into AzSpNegativeDefaults (NegativeKeyword, Phrase ,Exact, AzSpCountryCampaignConfigId) values (@NegativeKeyword, @Phrase , @Exact, @AzSpCountryCampaignConfigId); SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    entityId = await connection.QuerySingleAsync<int>(sql, entity);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SaveAzSpNegativeDefaults - AzSpNegativeDefaultsDA.cs", JsonSerializer.Serialize(entity));
            }
            return entityId;
        }

        public async Task<int> DeletezSpNegativeDefaultsByConfigId(int configId)
        {
            int response = 0;

            try
            {
                var sql = "DELETE FROM AzSpNegativeDefaults WHERE AzSpCountryCampaignConfigId = @AzSpCountryCampaignConfigId";
                var param = new DynamicParameters();
                param.Add("@AzSpCountryCampaignConfigId", configId);

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                response = -1;
                await ErrorLogging.LogError(ex.ToString(), "DeletezSpNegativeDefaultsByConfigId - AzSpNegativeDefaultsDA.cs", " configId: " + configId);
            }
            return response;
        }
        public async Task<List<AzSpNegativeDefaults>> GeAzSpNegativeDefaultsByConfigId(int configId)
        {
            List<AzSpNegativeDefaults> list = new List<AzSpNegativeDefaults>();
            try
            {
                var sql = "SELECT * FROM AzSpNegativeDefaults WHERE AzSpCountryCampaignConfigId = @AzSpCountryCampaignConfigId";
                var param = new { AzSpCountryCampaignConfigId = configId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpNegativeDefaults>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GeAzSpNegativeDefaultsByConfigId - AzSpNegativeDefaultsDA.cs", "configId : " + configId);
            }
            return list;
        }
    }
}
