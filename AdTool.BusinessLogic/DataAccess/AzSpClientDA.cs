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
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AzSpClientDA
    {
        public async Task<AzSpClient> GetAzSpClientByClientId(Guid clientId)
        {
            AzSpClient client = new AzSpClient();
            try
            {
                var sql = "SELECT * FROM AzSpClient WHERE Id= @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    client = (await connection.QueryAsync<AzSpClient>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpClientByClientId - AzSpClientDA.cs", "clientId : " + clientId);
            }
            return client;
        }

        public async Task<AzSpClient> GetAzSpClientByAppUserId(int userId)
        {
            AzSpClient client = new AzSpClient();
            try
            {
                var sql = "SELECT * FROM AzSpClient WHERE AppUserId= @UserId";
                var param = new { UserId = userId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    client = (await connection.QueryAsync<AzSpClient>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAzSpClientByAppUserId - AzSpClientDA.cs", "UsderId : " + userId);
            }
            return client;
        }

        public async Task<int> GetPaymentPlanByClientId(Guid clientId)
        {
            int paymentPlan = 0;
            try
            {
                var sql = "SELECT AppUser.PaymentPlan FROM AzSpClient client JOIN AppUser on client.AppUserId = AppUser.Id WHERE client.Id = @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    paymentPlan = (await connection.QueryAsync<int>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                paymentPlan = -1;
                await ErrorLogging.LogError(ex.ToString(), "GetPaymentPlanByClientId - AzSpClientDA.cs", "clientId : " + clientId);
            }
            return paymentPlan;
        }

        public async Task<int> GetAccountTypeByClientId(Guid clientId)
        {
            //default to Seller
            int accountType = 1;
            try
            {
                var sql = "SELECT AppUser.AccountType FROM AzSpClient client JOIN AppUser on client.AppUserId = AppUser.Id WHERE client.Id = @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    accountType = (await connection.QueryAsync<int>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                accountType = 1;
                await ErrorLogging.LogError(ex.ToString(), "GetAccountTypeByClientId - AzSpClientDA.cs", "clientId : " + clientId);
            }
            return accountType;
        }
    }
}
