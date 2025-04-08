using AdTool.BusinessLogic.Utilities;
using AdTool.PaymentProcessor.BusinessObjects;
using AdTool.PaymentProcessor.Utils;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Transactions;
using AdTool.PaymentProcessor.Data;
using Stripe;

namespace AdTool.PaymentProcessor.BusinessLogic
{
    public class StripeUpdatePaymentAlertLogic
    {
        public async Task<bool> ProcessAlert(string customerId)
        {
            try
            {
                SaveData sd = new SaveData();
                await sd.UpdateLastPaymentDate(customerId, DateTime.Now);
            }
            catch(Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "ProcessAlert", "Customer ID: " + customerId);
            }

            //we always return false to the webhook, no matter what
            return false;
        }

        public async Task<bool> UpdateUserFromFreeTrial(string customerId)
        {
            try
            {
                SaveData sd = new SaveData();
                await sd.UpdateUserFromFreeTrial(customerId);
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "ProcessAlert", "Customer ID: " + customerId);
            }

            //we always return false to the webhook, no matter what
            return false;
        }
        public async Task<bool> CancelSubscription(string customerId)
        {
            try
            {
                SaveData sd = new SaveData();
                await sd.WebhookCancelSpecificAppUser(customerId);
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "CancelSubscription", "Customer ID: " + customerId);
            }

            //we always return false to the webhook, no matter what
            return false;
        }

        public async Task<bool> PaymentFailed(string customerId)
        {
            try
            {
                SaveData sd = new SaveData();
                await sd.SuspendSpecificAppUser(customerId);

                try
                {
                    RetrieveData rd = new RetrieveData();

                    string email = await rd.GetAppUserEmailByStripeCustomer(customerId);

                    if (email != null)
                    {
                        List<string> emails = new List<string>();
                        emails.Add(email);
                        await EmailSender.sendEmail("Hi, there.<br/><br/>Looks like your subscription failed to process. We will not be able to automatically manage any campaigns until this is addressed.<br/><br/><a href='https://app.faktoriq.com'>Log in to FaktorIQ to update your payment method.</a><br/><br/>Sincerely,<br/>The FaktorIQ Team", "FaktorIQ - Payment Failed", emails);
                    }

                }
                catch (Exception ex)
                {
                    await ErrorLogging.LogError(ex.ToString(), "SuspendSpecificAppUser - updated db but did not send email", "Customer ID: " + customerId);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SuspendSpecificAppUser", "Customer ID: " + customerId);
            }

            //we always return false to the webhook, no matter what
            return false;
        }
    }
}
