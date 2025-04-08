using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.Edit.Auth;
using AdTool.Entities.Logging;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AppUserPaymentInfoDA
    {
        public async Task<AppUserPaymentInfo> GetAppUserPaymentInfoByUserId (int userId)
        {
            AppUserPaymentInfo existinguser = new AppUserPaymentInfo();
            try
            {
                var sql = "SELECT * FROM AppUserPaymentInfo WHERE AppUserId = @UserId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@UserId", userId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    existinguser = (await connection.QueryAsync<AppUserPaymentInfo>(sql, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAppUserPaymentInfo - AppUserPaymentInfoDA", "UserId : " + userId);
            }
            return existinguser;
        }
    }
}
