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
    public class AzCampaignTypeDA
    {
        public async Task<List<AzCampaignType>> GetAzCampaignTypeList()
        {
            List<AzCampaignType> list = new List<AzCampaignType>();
            try
            {
                var sql = "SELECT * FROM AzCampaignType";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzCampaignType>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzCampaignTypeList - AzCampaignTypeDA.cs", string.Empty);
            }
            return list;
        }
    }
}