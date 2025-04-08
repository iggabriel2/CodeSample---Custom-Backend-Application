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
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class AppUserDA
    {
        public async Task<int> InsertAppUser(AppUser user)
        {
            int userId = 0;
            try
            {
                var sql = "INSERT INTO APPUSER (UserName ,IsActive , Email, EmailConfirmed, PasswordHash, PhoneNumber, PhoneNumberConfirmed, LastLoginDate, LastLockOutDate, " +
                    "IsLockedOut, FailedPwdAttemptDate, FailedPwdAttemptCount, LastPwdChangedDate, TwoFactorEnabled, PaymentPlan, JoinDate, FirstName, LastName, SubscriptionAmount, PaymentSchedule, AgreeWithPromoEmail, AccountType, PromoCode, FreeTrial) " +
                    "VALUES (@UserName , @IsActive , @Email, @EmailConfirmed, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @LastLoginDate, @LastLockOutDate, @IsLockedOut, @FailedPwdAttemptDate, " +
                    "@FailedPwdAttemptCount, @LastPwdChangedDate, @TwoFactorEnabled, @PaymentPlan, @JoinDate, @FirstName, @LastName , @SubscriptionAmount, @PaymentSchedule, @AgreeWithPromoEmail, @AccountType, @PromoCode, @FreeTrial); SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                   userId =  await connection.QuerySingleAsync<int>(sql, user);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "InsertAppUser - AppUserDA.cs";
                logError.Parameters = JsonSerializer.Serialize(user);
                await logging.WriteToLog(logError);
            }
            return userId;
        }

        public async Task<AppUser> GetAppUserByUserName(string username, UIMessage message)
        {
            AppUser existinguser = new AppUser();
            try
            {
                var sql = "SELECT * FROM APPUSER WHERE Username = @UserName";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Username", username);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    existinguser = (await connection.QueryAsync<AppUser>(sql, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                message.ErrorMessages.Add("An error occured while processing this request.");
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "InsertAppUser - AppUserDA.cs";
                logError.Parameters = "Username: " + username;
                await logging.WriteToLog(logError);
            }
            return existinguser;
        }

        public async Task<AppUser> GetAppUserById(int appUserId, UIMessage message)
        {
            AppUser existinguser = new AppUser();
            try
            {
                var sql = "SELECT * FROM APPUSER WHERE Id = @AppUserId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@AppUserId", appUserId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    existinguser = await connection.QuerySingleAsync<AppUser>(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                message.ErrorMessages.Add("An error occured while processing this request.");
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAppUserById - AppUserDA.cs";
                logError.Parameters = "Email : " + appUserId;
                await logging.WriteToLog(logError);
            }
            return existinguser;
        }

        public async Task<AppUser> GetAppUserByIdandUserName(int appUserId, string userName, UIMessage message)
        {
            AppUser existinguser = new AppUser();
            try
            {
                var sql = "SELECT * FROM APPUSER WHERE Id = @AppUserId AND UserName = @Username";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@AppUserId", appUserId);
                queryParameters.Add("@UserName", userName);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    existinguser = await connection.QuerySingleAsync<AppUser>(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                message.ErrorMessages.Add("An error occured while processing this request.");
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "InsertAppUser - AppUserDA.cs";
                logError.Parameters = "Email : " + appUserId;
                await logging.WriteToLog(logError);
            }
            return existinguser;
        }
        public async Task<int> UpdateAppUserFullName(int userId, string firstName, string lastName)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AppUser " +
                    "SET FirstName = @FirstName, " +
                    "LastName = @LastName " +
                    "WHERE Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", userId);
                queryParameters.Add("@FirstName", firstName);
                queryParameters.Add("@LastName", lastName);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAppUserFullName - AppUserDA.cs", "userId : " + userId + " , firstname: " + firstName + " , lastname: " + lastName );
            }
            return result;
        }

        public async Task<int> UpdateAppUserEmail(int userId, string email)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AppUser " +
                    "SET Email = @Email, " +
                    "EmailConfirmed = 0 " +
                    "WHERE Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", userId);
                queryParameters.Add("@Email", email);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAppUserFullName - AppUserDA.cs", "userId : " + userId + " , Email: " + email);
            }
            return result;
        }

        public async Task<AppUser> GetAppUserByClientId(Guid clientId)
        {
            AppUser existinguser = new AppUser();
            try
            {
                var sql = "SELECT AppUser.*" +
                          "FROM AppUser JOIN AzSpClient ON AppUser.Id = AzSpClient.AppUserId " +
                          "WHERE AzSpClient.Id = @ClientId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", clientId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    existinguser = await connection.QuerySingleAsync<AppUser>(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetAppUserByClientId - AppUserDA.cs", "clientId : " + clientId);
            }
            return existinguser;
        }

        public async Task<int> SetAppUserEmailConfirmed(int userId, bool isConfirmed)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AppUser " +
                    "SET EmailConfirmed = @IsConfirmed, " +
                    "EmailCode = NULL, " +
                    "EmailCodeDate = NULL " +
                    "WHERE Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", userId);
                queryParameters.Add("@IsConfirmed", isConfirmed);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "SetAppUserEmailConfirmed - AppUserDA.cs", "userId : " + userId + " , isConfirmed: " + isConfirmed);
            }
            return result;
        }

        public async Task<int> SetTempEmailCode(int userId, string code, DateTime date)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AppUser " +
                    "SET EmailCode = @Code, " +
                    "EmailCodeDate = @Date " +
                    "WHERE Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", userId);
                queryParameters.Add("@Code", code);
                queryParameters.Add("@Date", date);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "SetTempEmailCode - AppUserDA.cs", "userId : " + userId + " , code: " + code + " , date :" + date);
            }
            return result;
        }

        public async Task<int> UpdateAppUserPassword(int userId, string password)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AppUser " +
                    "SET PasswordHash = @Password " +
                    "WHERE Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", userId);
                queryParameters.Add("@Password", password);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAppUserPassword - AppUserDA.cs", "userId : " + userId + " pass : " + password);
            }
            return result;
        }

        public async Task<int> UpdateAppUserSubscriptionAmount(int userId, decimal amount)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE AppUser " +
                    "SET SubscriptionAmount = @SubscriptionAmount " +
                    "WHERE Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", userId);
                queryParameters.Add("@SubscriptionAmount", amount);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateAppUserSubscriptionAmount - AppUserDA.cs", "userId : " + userId + " amount : " + amount);
            }
            return result;
        }
    }
}
