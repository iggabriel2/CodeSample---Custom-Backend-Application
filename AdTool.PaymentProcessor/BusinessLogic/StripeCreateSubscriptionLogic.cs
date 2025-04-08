using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Payments;
using AdTool.PaymentProcessor.BusinessObjects;
using AdTool.PaymentProcessor.Data;
using AdTool.PaymentProcessor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Configuration;
using Azure.Core;
using System.ComponentModel.DataAnnotations;
using Azure;
using AdTool.Entities.Edit.Auth;
using Microsoft.Identity.Client;
using AdTool.Entities.EmailSending;
using Stripe;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using AdTool.BusinessLogic.DataAccess;

namespace AdTool.PaymentProcessor.BusinessLogic
{
    public class StripeCreateSubscriptionLogic
    {

        public int FreeTrailDays = 15;
        public async Task<SimplePaymentResponse> CreateSubscription(CreateSubscriptionRequestObject subscription)
        {
            SimplePaymentResponse simplePaymentResponse = new SimplePaymentResponse();
            bool paymentSuccess = false;
            string appUserId = await PaymentEncryption.DecryptString(subscription.processDate, subscription.AppUserId);

            try 
            {
                try
                {
                    //placeholders
                    string paymentMethodId = "";
                    string customerId = "";
                    string subscriptionId = "";
                    string ApiId = "";

                    StripeConfiguration.ApiKey = AppSettings.StripeKey();

                    paymentMethodId = subscription.paymentMethod;

                    RetrieveData retrieveData = new RetrieveData();
                    SaveData saveData = new SaveData();

                    var customerEmail = await retrieveData.GetAppUserEmail(Convert.ToInt32(appUserId));

                    //make customer id
                    var options2 = new CustomerCreateOptions
                    {
                        Email = customerEmail,
                        Name = subscription.firstName + " " + subscription.lastName
                    };
                    var service2 = new CustomerService();
                    var createCustomer = service2.Create(options2);

                    if (createCustomer != null && createCustomer.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (!string.IsNullOrEmpty(createCustomer.Id))
                        {
                            customerId = createCustomer.Id;

                            //attach payment method
                            var options3 = new PaymentMethodAttachOptions
                            {
                                Customer = customerId,
                            };
                            var service3 = new PaymentMethodService();
                            var paymentMethodAttached = service3.Attach(
                                paymentMethodId,
                                options3);

                            if (paymentMethodAttached != null && paymentMethodAttached.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                //attach payment method
                                var invoiceSettings = new CustomerInvoiceSettingsOptions
                                {
                                    DefaultPaymentMethod = paymentMethodId
                                };

                                var options31 = new CustomerUpdateOptions
                                {
                                    InvoiceSettings = invoiceSettings
                                };
                                var service31 = new CustomerService();
                                var defaultPaymentMehod = service31.Update(customerId, options31);


                                if (defaultPaymentMehod != null && defaultPaymentMehod.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(defaultPaymentMehod.Id))
                                {
                                    //create subscription
                                    if (!string.IsNullOrEmpty(paymentMethodAttached.Id))
                                    {
                                        var options4 = new SubscriptionCreateOptions
                                        {
                                            Customer = customerId,
                                            Items = new List<SubscriptionItemOptions>
                                                {
                                                new SubscriptionItemOptions
                                                {
                                                    Price = subscription.apiId,
                                                 
                                                },
                                                },
                                            PaymentBehavior = "error_if_incomplete",
                                            CollectionMethod = "charge_automatically",
                                            DefaultPaymentMethod = paymentMethodAttached.Id,
                                            TrialPeriodDays = subscription.isFreeTrial ? FreeTrailDays : null,
                                            Coupon = subscription.couponApi,

                                        };
                                        var service4 = new SubscriptionService();
                                        var subscriptionStatus = service4.Create(options4);

                                        if (subscriptionStatus != null && subscriptionStatus.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK)
                                        {
                                            //when on free trial the status is "trialing", when on regular plan the status is "active"
                                            if (!string.IsNullOrEmpty(subscriptionStatus.Id) && (subscriptionStatus.Status.ToLower() == "active" || subscriptionStatus.Status.ToLower() == "trialing"))
                                            {
                                                subscriptionId = subscriptionStatus.Id;

                                                //save subscription values to db
                                                AppUserPaymentInfoInsert appInfo = new AppUserPaymentInfoInsert();
                                                appInfo.AppUserId = Convert.ToInt32(appUserId);
                                                appInfo.SubscriptionId = subscriptionId;
                                                appInfo.CustomerProfileId = customerId;
                                                appInfo.CustomerPaymentProfileId = paymentMethodId;
                                                appInfo.CCLastFour = subscription.fourDigits;
                                                appInfo.AssignedCustomerPaymentProfileId = paymentMethodAttached.Id;
                                                appInfo.AssignedStripeProductId = subscription.apiId;
                                                appInfo.SubscriptionItemId = subscriptionStatus.Items.Data[0].Id;

                                               
                                                var saveRepsonse = await saveData.SavePaymentCodes(appInfo);

                                                //update active status
                                                var activeStatus = await saveData.UpdateActiveStatus(Convert.ToInt32(appUserId), true);


                                                //this is for single users, not agencies. Will need to exclude on agencies and make them set up their first account when they first log in.
                                                var activeClientId = await saveData.CreateClientIdHolder(Convert.ToInt32(appUserId));


                                                if (!activeStatus || !activeClientId)
                                                {
                                                    await ErrorLogging.LogError("activation failed", "CreateSubscription", "Payment and Subscription Succeeded. Activation failed. Activate appuserid " + appUserId);

                                                    RetrieveData rd = new RetrieveData();
                                                    List<string> emails = await rd.GetSupportEmails();

                                                    //make tokens for the email
                                                    EmailToken appUserToken = new EmailToken();
                                                    appUserToken.TokenName = "appUserId";
                                                    appUserToken.TokenValue = appUserId;
                                                    List<EmailToken> emailTokens = new List<EmailToken>();
                                                    emailTokens.Add(appUserToken);

                                                    //get the template for the email
                                                    var template = await EmailTemplate.GetTemplate("ActivationFailed", emailTokens);

                                                    //send the email
                                                    await EmailSender.sendEmail(template.Body, template.Subject, emails);
                                                }

                                                if (!saveRepsonse)
                                                {
                                                    simplePaymentResponse.Message = "Payment succeeded. Subscription created. Failed to save payment codes in our database. Carry on. Will fix manually.";
                                                    await ErrorLogging.LogError("Payment succeeded. Subscription created. Failed to save payment codes in our database. Fix manually. App user id: " + appUserId + ". Subscription Id: " + subscriptionId + "Customer Profile Id: " + customerId + ". Customer Payment Profile Id: " + paymentMethodId + ".", "CreateSubscription - one-time charge", JsonSerializer.Serialize(subscription));

                                                    RetrieveData rd = new RetrieveData();
                                                    List<string> emails = await rd.GetSupportEmails();
                                                    await EmailSender.sendEmail("Payment succeeded. Subscription created. Failed to save payment codes to our db. App user id: " + appUserId + ". Subscription Id: " + subscriptionId + "Customer Profile Id:" + customerId + ". Customer Payment Profile Id: " + paymentMethodId + ".", "Fix Successful Subscription", emails);
                                                }

                                                simplePaymentResponse.Message = "Subscription created";
                                                simplePaymentResponse.Success = true;
                                                return simplePaymentResponse;

                                            }
                                            else
                                            {
                                                await saveData.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                                                simplePaymentResponse.Message = "Failed to make subscription";
                                                return simplePaymentResponse;
                                            }
                                        }
                                        else
                                        {
                                            await saveData.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                                            simplePaymentResponse.Message = "Null response";
                                            return simplePaymentResponse;
                                        }
                                    }
                                    else
                                    {
                                        await saveData.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                                        simplePaymentResponse.Message = "Failed to attach payment method.";
                                        return simplePaymentResponse;
                                    }
                            }
                            else
                            {
                                    await saveData.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                                    simplePaymentResponse.Message = "Failed to attach default payment method";
                                return simplePaymentResponse;
                            }
                        }
                            else
                            {
                                await saveData.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                                simplePaymentResponse.Message = "Null response";
                                return simplePaymentResponse;
                            }
                        }
                        else
                        {
                            await saveData.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                            simplePaymentResponse.Message = "Failed to make user";
                            return simplePaymentResponse;
                        }
                    }
                    else
                    {
                        await saveData.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                        simplePaymentResponse.Message = "Null response";
                        return simplePaymentResponse;
                    }
                }
                catch(StripeException se)
                {
                    SaveData sv = new SaveData();
                    await sv.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                    simplePaymentResponse.Success = false;
                    simplePaymentResponse.Message = se.Message;
                    return simplePaymentResponse;
                }
                catch(Exception ex)
                {
                    SaveData sv = new SaveData();
                    await sv.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                    await ErrorLogging.LogError(ex.ToString(), "CreateSubscription", JsonSerializer.Serialize(subscription));
                    simplePaymentResponse.Success = false;
                    simplePaymentResponse.Message = "Payment failed. Exception logged.";
                    return simplePaymentResponse;
                }
            }
            catch (Exception ex)
            {
                SaveData sv = new SaveData();
                await sv.DeleteSpecificAppUser(Convert.ToInt32(appUserId));
                await ErrorLogging.LogError(ex.ToString(), "CreateSubscription", JsonSerializer.Serialize(subscription));
                simplePaymentResponse.Message = "General error in exception. Entire payment process failed. Check error log.";
                return simplePaymentResponse;
            }
        }

    }
}
