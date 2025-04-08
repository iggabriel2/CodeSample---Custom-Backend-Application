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
    public class AzSpCampaignUsageTypeDA
    {
        public async Task<List<AzSpCampaignUsageType>> GetAzSpCampaignUsageTypeList()
        {
            List<AzSpCampaignUsageType> list = new List<AzSpCampaignUsageType>();
            try
            {
                var sql = "SELECT * FROM AzSpCampaignUsageType";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpCampaignUsageType>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCampaignUsageTypeList - AzSpCampaignUsageTypeDA.cs", string.Empty);
            }
            return list;
        }
    }
}
