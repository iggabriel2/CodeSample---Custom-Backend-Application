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
    public class AzSpCampaingKeywordTypeDA
    {
        public async Task<List<AzSpCampaingKeywordType>> GetAzSpCampaingKeywordTypeList()
        {
            List<AzSpCampaingKeywordType> list = new List<AzSpCampaingKeywordType>();
            try
            {
                var sql = "Select * from AzSpCampaingKeywordType";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpCampaingKeywordType>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCampaingKeywordTypeList - AzSpCampaingKeywordTypeDA.cs", string.Empty);
            }
            return list;
        }
    }
}
