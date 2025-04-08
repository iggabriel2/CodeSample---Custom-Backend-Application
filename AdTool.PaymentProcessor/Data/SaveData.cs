using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit.Auth;
using AdTool.Entities.Logging;
using AdTool.PaymentProcessor.BusinessObjects;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.Data
{
    public class SaveData
    {
        public async Task<bool> SavePaymentCodes(AppUserPaymentInfoInsert appUserPaymentInfoInsert)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("SavePaymentCodes", appUserPaymentInfoInsert, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SavePaymentCodes", JsonSerializer.Serialize(appUserPaymentInfoInsert));
                return false;
            }
        }

        public async Task<bool> UpdateUserFromFreeTrial(string customerId)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateUserFromFreeTrial", new { @customerId = customerId }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateUserFromFreeTrial", "Customer Id: " + customerId);
                return false;
            }
        }

        public async Task<bool> UpdateActiveStatus(int AppUserId, bool active)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateActiveStatus", new { @AppUserId = AppUserId, @active = active }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateActiveStatus", AppUserId.ToString() + " " + active.ToString());
                return false;
            }
        }

        public async Task<bool> UpdateSubscriptionCancellationDate(int AppUserId, DateTime CancellationDate)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateSubscriptionCancellationDate", new { @AppUserId = AppUserId, @CancellationDate = CancellationDate }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateSubscriptionCancellationDate", AppUserId.ToString() + " " + CancellationDate.ToString());
                return false;
            }
        }

        public async Task<bool> UpdateSubscriptionItem(int AppUserId, string SubscriptionItemId, string AssignedStripeProductId)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateSubscriptionItem", new { @AppUserId = AppUserId, @SubscriptionItemId = SubscriptionItemId, @AssignedStripeProductId = AssignedStripeProductId }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError("Nothing to do. User advised of failure. Error: " + ex.ToString(), "UpdateSubscriptionItem", AppUserId.ToString() + " " + SubscriptionItemId);
                return false;
            }
        }
        public async Task<bool> UpdateSubscriptionStatusToActive(int AppUserId, string SubscriptionStatus, string CCLastFour, string CustomerPaymentProfileId = null, string AssignedCustomerPaymentProfileId = null)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateSubscriptionStatusToActive", new { @AppUserId = AppUserId, @SubscriptionStatus = SubscriptionStatus, @CCLastFour = CCLastFour, @AssignedCustomerPaymentProfileId = AssignedCustomerPaymentProfileId, @CustomerPaymentProfileId = CustomerPaymentProfileId }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError("Nothing to do. User advised of failure. Error: " + ex.ToString(), "UpdateSubscriptionStatusToActive", AppUserId.ToString() + " " + SubscriptionStatus);
                return false;
            }
        }

        public async Task<bool> UpdateLastPaymentDate(string customerId, DateTime LastPaymentDate)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateAppUserLastPaymentDate", new { @customerId = customerId, @LastPaymentDate = LastPaymentDate }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateLastPaymentDate", "Customer Id: " + customerId + ", Last Payment Date: " + LastPaymentDate.ToString());
                return false;
            }
        }

        public async Task<bool> DeactivateSpecificAppUser(string CustomerProfileId, string SubscriptionStatus)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("DeactivateSpecificAppUser", new { @CustomerProfileId = CustomerProfileId, @SubscriptionStatus = SubscriptionStatus }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "DeactivateSpecificAppUser", "CustomerProfileId: " + CustomerProfileId + ", SubscriptionStatus: " + SubscriptionStatus);
                return false;
            }
        }

        public async Task<bool> DeleteSpecificAppUser(int Appuserid)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("DeleteSpecificAppUser", new { @appuserid = Appuserid }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "DeleteSpecificAppUser", "Appuserid: " + Appuserid);
                return false;
            }
        }

        public async Task<bool> SuspendSpecificAppUser(string CustomerProfileId)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("SuspendSpecificAppUser", new { @CustomerProfileId = CustomerProfileId }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SuspendSpecificAppUser", "CustomerProfileId: " + CustomerProfileId);
                return false;
            }
        }

        public async Task<bool> WebhookCancelSpecificAppUser(string CustomerProfileId)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("WebhookCancelSpecificAppUser", new { @CustomerProfileId = CustomerProfileId }, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "WebhookCancelSpecificAppUser", "CustomerProfileId: " + CustomerProfileId);
                return false;
            }
        }

        public async Task<bool> DeactivateExpiredAppUsers()
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("DeactivateExpiredAppUsers", commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "DeactivateExpiredAppUsers", "none");
                return false;
            }
        }

        public async Task<AppUserPlanInfo> UpdateSubscriptionTypeAndAmount(int AppUserId, int PaymentPlan, decimal SubscriptionAmount, bool endFreeTrial)
        {
            AppUserPlanInfo appUserPlanInfo = new AppUserPlanInfo();
            appUserPlanInfo.OldPlan = 0;
            appUserPlanInfo.OldSubscriptionAmount = 0;

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    appUserPlanInfo = (await connection.QueryAsync<AppUserPlanInfo>("UpdateSubscriptionTypeAndAmount", new { @AppUserId = AppUserId, @PaymentPlan = PaymentPlan, SubscriptionAmount = @SubscriptionAmount, EndFreeTrial = @endFreeTrial }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return appUserPlanInfo;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateSubscriptionTypeAndAmount", "AppUserId: " + AppUserId.ToString() + ", PaymentPlan: " + PaymentPlan.ToString() + ", SubscriptionAmount: " + SubscriptionAmount.ToString());
                return appUserPlanInfo;
            }
        }


        public async Task<bool> CreateClientIdHolder(int AppUserId)
        {
            bool success = false;
            try
            {
                Guid responseClientId;
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    responseClientId = (await connection.QueryAsync<Guid>("CreateClientIdHolder", new { @AppUserId = AppUserId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                }
                success = true;
                return success;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreateClientIdHolder";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "AppUserId: " + AppUserId.ToString();
                await logging.WriteToLog(logError);

                return success;
            }
        }

    }
}
