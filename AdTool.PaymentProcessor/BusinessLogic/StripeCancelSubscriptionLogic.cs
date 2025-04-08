using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Payments;
using AdTool.PaymentProcessor.BusinessObjects;
using AdTool.PaymentProcessor.Data;
using AdTool.PaymentProcessor.Utils;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Stripe;

namespace AdTool.PaymentProcessor.BusinessLogic
{
    public class StripeCancelSubscriptionLogic
    {
        public async Task<SimplePaymentResponse> CancelSubscription(CancelSubscriptionRequestObject subscription)
        {
            SimplePaymentResponse simplePaymentResponse = new SimplePaymentResponse();

            try
            {
                simplePaymentResponse.Success = false;

                string appUserId = await PaymentEncryption.DecryptString(subscription.processDate, subscription.AppUserId);

                RetrieveData rd = new RetrieveData();
                AppUserPaymentInfo appUserPaymentInfo = await rd.GetPaymentInfo(Convert.ToInt32(appUserId));

                StripeConfiguration.ApiKey = AppSettings.StripeKey();

                var service = new SubscriptionService();
                var cancelSubscriptionObject = service.Cancel(appUserPaymentInfo.SubscriptionId);

                if (cancelSubscriptionObject != null && cancelSubscriptionObject.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(cancelSubscriptionObject.Id))
                {
                    //find cancellation date
                    var cancellationDate = DateTime.Parse(appUserPaymentInfo.LastPaymentDate).AddMonths(1);

                    simplePaymentResponse.Success = true;
                    SaveData sd = new SaveData();
                    var updateResponse = await sd.UpdateSubscriptionCancellationDate(Convert.ToInt32(appUserId), cancellationDate);

                    if (!updateResponse)
                    {
                        simplePaymentResponse.Success = false;
                        simplePaymentResponse.Message = "Failed to cancel subscription. Please retry.";
                        await ErrorLogging.LogError("Call tp authorize.net worked. Failed to cancel subscription in our db. Empty API response. Confirm cancellation.", "CancelSubscription", JsonSerializer.Serialize(subscription));

                    }
                    else
                    {
                        simplePaymentResponse.Message = cancellationDate.ToString("MM/dd/yyyy");
                    }
                }
                else
                {
                    simplePaymentResponse.Message = "Failed to cancel subscription. Please contact support.";
                    simplePaymentResponse.StatusCode = "0";
                    await ErrorLogging.LogError("Failed to cancel subscription.", "CancelSubscription", JsonSerializer.Serialize(subscription));

                }
            }
            catch (StripeException se)
            {
                simplePaymentResponse.Success = false;
                simplePaymentResponse.Message = se.Message;
                return simplePaymentResponse;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "CancelSubscription", JsonSerializer.Serialize(subscription));
                simplePaymentResponse.Success = false;
                simplePaymentResponse.Message = "Failed to cancel subscription.";
            }

            return simplePaymentResponse;
        }

    }
}
