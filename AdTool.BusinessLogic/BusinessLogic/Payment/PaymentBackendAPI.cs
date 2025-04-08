using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using AdTool.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.BusinessLogic.Payment
{
    public class PaymentBackendAPI
    {
        #region Subscription
        public async Task<SimplePaymentResponse> CreateSubscription(CreateSubscriptionRequestObject subscriptionRequest)
        {
            SimplePaymentResponse? myResponse = new SimplePaymentResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(subscriptionRequest);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Payment/CreateSubscription", Guid.Empty);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimplePaymentResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "CreateSubscription - Api Call";
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);
                    myResponse.Message = "Failed to create subscription.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreateSubscription";
                logError.Parameters = JsonSerializer.Serialize(subscriptionRequest);
                await logging.WriteToLog(logError);

                myResponse.Message = "Failed to create subscription.";

            }
            return myResponse;
        }

        public async Task<SimplePaymentResponse> CancelSubscription(CancelSubscriptionRequestObject subscriptionRequest)
        {
            SimplePaymentResponse? myResponse = new SimplePaymentResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(subscriptionRequest);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Payment/CancelSubscription", Guid.Empty);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimplePaymentResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "CancelSubscription - Api Call";
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);
                    myResponse.Message = "Failed to cancel subscription.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CancelSubscription";
                logError.Parameters = JsonSerializer.Serialize(subscriptionRequest);
                await logging.WriteToLog(logError);

                myResponse.Message = "Failed to cancel subscription.";

            }
            return myResponse;
        }

        public async Task<SimplePaymentResponse> UpdateCreditCard(CreateSubscriptionRequestObject subscriptionRequest)
        {
            SimplePaymentResponse? myResponse = new SimplePaymentResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(subscriptionRequest);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Payment/UpdateCreditCard", Guid.Empty);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimplePaymentResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "UpdateCreditCard - Api Call";
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);
                    myResponse.Message = "Failed to update credit card.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateCreditCard";
                logError.Parameters = JsonSerializer.Serialize(subscriptionRequest);
                await logging.WriteToLog(logError);

                myResponse.Message = "Failed to update credit card.";

            }
            return myResponse;
        }

        public async Task<SimplePaymentResponse> ChangePaymentPlan(ChangePaymentPlanObject subscriptionRequest)
        {
            SimplePaymentResponse? myResponse = new SimplePaymentResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(subscriptionRequest);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Payment/ChangePaymentPlan", Guid.Empty);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimplePaymentResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "ChangePaymentPlan - Api Call";
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);
                    myResponse.Message = "Failed to update payment plan.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ChangePaymentPlan";
                logError.Parameters = JsonSerializer.Serialize(subscriptionRequest);
                await logging.WriteToLog(logError);

                myResponse.Message = "Failed to update payment plan.";

            }
            return myResponse;
        }
        #endregion
    }
}
