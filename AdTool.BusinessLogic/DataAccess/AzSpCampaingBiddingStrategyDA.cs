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
    public class AzSpCampaingBiddingStrategyDA
    {
        public async Task<List<AzSpCampaingBiddingStrategy>> GetAzSpCampaingBiddingStrategyList() 
        {
            List<AzSpCampaingBiddingStrategy> list = new List<AzSpCampaingBiddingStrategy>();
            try 
            {
                var sql = "Select * from AzSpCampaingBiddingStrategy";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<AzSpCampaingBiddingStrategy>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpCampaingBiddingStrategyList - AzSpCampaingBiddingStrategyDA.cs", string.Empty);
            }
            return list;
        }
    }
}
