using AdTool.Entities.Edit.Auth;
using AdTool.PaymentProcessor.BusinessObjects;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.Data
{
    public class RetrieveData
    {
        public async Task<AppUserPaymentInfo> GetPaymentInfo(int appUserId)
        {
            AppUserPaymentInfo appUserPaymentInfo = new AppUserPaymentInfo();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    appUserPaymentInfo = (await connection.QueryAsync<AppUserPaymentInfo>("GetPaymentCodes", new { @AppUserId = appUserId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return appUserPaymentInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetAppUserEmail(int appUserId)
        {
            string email = "";

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    email = (await connection.QueryAsync<string>("GetAppUserEmail", new { @AppUserId = appUserId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return email;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetAppUserEmailByStripeCustomer(string CustomerProfileId)
        {
            string email = "";

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    email = (await connection.QueryAsync<string>("GetAppUserEmailByStripeCustomer", new { @CustomerProfileId = CustomerProfileId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return email;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetPaymentPlanApId(decimal Price)
        {
            string paymentPlanId = "";

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    paymentPlanId = (await connection.QueryAsync<string>("GetPaymentPlanApId", new { @Price = Price }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return paymentPlanId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<string>> GetSupportEmails()
        {
            List<string> emails = new List<string>();

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    emails = (await connection.QueryAsync<string>("GetSupportEmails", commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                emails.Add("support@faktoriq.com");
            }

            return emails;
        }
    }
}
