using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Payments;
using AdTool.Entities.View;
using AdTool.PaymentProcessor.BusinessObjects;
using AdTool.PaymentProcessor.Data;
using AdTool.PaymentProcessor.Utils;
using Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.BusinessLogic
{
    public class StripeChangePaymentPlanLogic
    {
        public async Task<SimplePaymentResponse> ChangePaymentPlan(ChangePaymentPlanObject subscription)
        {
            SimplePaymentResponse simplePaymentResponse = new SimplePaymentResponse();
            simplePaymentResponse.Success = false;

            try
            {

                string appUserId = await PaymentEncryption.DecryptString(subscription.processDate, subscription.AppUserId);

                RetrieveData rd = new RetrieveData();
                AppUserPaymentInfo appUserPaymentInfo = await rd.GetPaymentInfo(Convert.ToInt32(appUserId));

                StripeConfiguration.ApiKey = AppSettings.StripeKey();
                var options = new SubscriptionUpdateOptions();

                //settings for plan update
                if (subscription.isPlanChange)
                {
                    options.ProrationBehavior = "always_invoice";
                    options.Items = new List<SubscriptionItemOptions>();
                    options.Items.Add(new SubscriptionItemOptions { Id = appUserPaymentInfo.SubscriptionItemId, Price = subscription.apiId });
                }

                //settings if it's upgrade from free trial
                if (subscription.isFreeTrial)
                {
                    options.TrialEnd = SubscriptionTrialEnd.Now;
                }


                var service = new SubscriptionService();
                var subscriptionUpdate = service.Update(appUserPaymentInfo.SubscriptionId, options);

                if (subscriptionUpdate != null && subscriptionUpdate.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(subscriptionUpdate.Id))
                {
                    simplePaymentResponse.Success = true;
                    SaveData sd = new SaveData();

                    //update database
                    //end free trial is set to true because its the only option for upgrades. as additional plans are added will need to update this logic
                    var appUserPlanInfo = await sd.UpdateSubscriptionTypeAndAmount(Convert.ToInt32(appUserId), subscription.PaymentPlan, subscription.PaymentAmount, true);

                    var updateResponse = await sd.UpdateSubscriptionItem(Convert.ToInt32(appUserId), subscriptionUpdate.Items.Data[0].Id, subscription.apiId);

                    if (!updateResponse)
                    {
                        await ErrorLogging.LogError("Call to Stripe worked. Failed to update subscription in our db. Update manually.", "ChangePaymentPlan", JsonSerializer.Serialize(subscription));
                    }
                }
                else
                {
                    simplePaymentResponse.Message = "Failed to update subscription. Please contact support.";
                    simplePaymentResponse.StatusCode = "0";
                    await ErrorLogging.LogError("Failed to update subscription.", "ChangePaymentPlan", JsonSerializer.Serialize(subscription));
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
                await ErrorLogging.LogError(ex.ToString(), "ChangePaymentPlanLogic", JsonSerializer.Serialize(subscription));
                simplePaymentResponse.Success = false;
                simplePaymentResponse.Message = "Failed to change the payment plan.";
            }

            return simplePaymentResponse;
        }

    }
}
