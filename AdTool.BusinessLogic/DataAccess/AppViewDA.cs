using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.Edit.Auth;
using AdTool.Entities.Logging;
using AdTool.Entities.View;
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
    public class AppViewDA
    {
        public async Task<List<PaymentPlanPricingView>> GetPaymentPlanPricingView(UIMessage message)
        {
            List<PaymentPlanPricingView> list = new List<PaymentPlanPricingView>();
            try
            {
                var sql = "SELECT price.*, " +
                    "plans.PlanName, " +
                    "sch.Schedule " +
                    "FROM PaymentPlanPricing price " +
                    "JOIN PaymentPlans plans ON price.PlanId = plans.id " +
                    "JOIN PaymentSchedules sch ON price.ScheduleId = sch.Id";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<PaymentPlanPricingView>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                message.ErrorMessages.Add("An error occured while processing this request.");
                await ErrorLogging.LogError(ex.ToString(), "GetPaymentSchedulesList - PaymentSchedulesDA", "");
            }
            return list;
        }

        public async Task<int> LockOutAccount(int userId)
        {
            DateTime date = DateTime.Now;
            int result = 1;
            try
            {
                var sql = "Update Appuser Set IsLockedOut = 1, FailedPwdAttemptCount = 5, LastLockOutDate = @Date where Id = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", userId);
                param.Add("@Date", date);

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "LockOutAccount - AuthDA.cs", "UsderId : " + userId);
            }
            return result;
        }

        public async Task<AppUser> UpdateFailedPaswordCount(int userId, int count)
        {
            AppUser user = new AppUser();
            try
            {
                var sql = "Update Appuser Set FailedPwdAttemptCount = @Count where Id = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", userId);
                param.Add("@Count", count);

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "LockOutAccount - AuthDA.cs", "UsderId : " + userId);
            }
            return user;
        }

        public async Task<AppUser> GetUserByUserName(string userName)
        {
            AppUser user = new AppUser();
            try
            {
                var sql = "SELECT * FROM AppUser WHERE UserName = @UserName";
                var param = new { UserName = userName };

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    user = (await connection.QueryAsync<AppUser>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetUserByUserName - AuthDA.cs", "Username : " + userName);
            }
            return user;
        }

        public async Task<AdminPasswordValues> GetAdminPassword()
        {
            AdminPasswordValues adminPasswordValues = new AdminPasswordValues();
            try
            {
                var sql = "SELECT top(1) * FROM AdminPassword";

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    adminPasswordValues = (await connection.QueryAsync<AdminPasswordValues>(sql)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAdminPassword - AuthDA.cs", "None");
            }
            return adminPasswordValues;
        }

        public async Task<AppUser> GetUserByUserId(int userId)
        {
            AppUser user = new AppUser();
            try
            {
                var sql = "SELECT * FROM AppUser WHERE Id = @Id";
                var param = new { Id = userId };

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    user = (await connection.QueryAsync<AppUser>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetUserByUserId - AuthDA.cs", "userId : " + userId);
            }
            return user;
        }

        public async Task<int> UpdateTempPassUser(string? pass, DateTime? passTime, int userId)
        {
            int result = 0;
            try
            {
                var sql = "UPDATE AppUser SET TempPass = @TempPass, TempPassDate = @TempPassDate WHERE Id = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", userId);
                param.Add("@TempPass", pass);
                param.Add("@TempPassDate", passTime);

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateTempPassUser - AuthDA.cs", " hashedPass: " + pass + " passTime : " + passTime + " userId: " + userId);
            }
            return result;
        }
    }
}
