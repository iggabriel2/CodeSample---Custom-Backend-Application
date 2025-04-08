using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Payments;
using AdTool.PaymentProcessor.BusinessObjects;
using AdTool.PaymentProcessor.Utils;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdTool.PaymentProcessor.Data;
using Stripe;
using Azure.Core;
using System.Text.Json.Serialization;

namespace AdTool.PaymentProcessor.BusinessLogic
{
    public class StripeUpdateCCSubscriptionLogic
    {
        public async Task<SimplePaymentResponse> UpdateCreditCard(CreateSubscriptionRequestObject subscription)
        {
            SimplePaymentResponse simplePaymentResponse = new SimplePaymentResponse();

            try
            {
                //placeholders
                string paymentMethodId = "";
                string customerId = "";
                string subscriptionId = "";
                string ApiId = "";

                simplePaymentResponse.Success = false;

                //decrypt everything
                string appUserId = await PaymentEncryption.DecryptString(subscription.processDate, subscription.AppUserId);
               

                string CCLastFour = subscription.fourDigits;

                RetrieveData rd = new RetrieveData();
                AppUserPaymentInfo appUserPaymentInfo = await rd.GetPaymentInfo(Convert.ToInt32(appUserId));

                StripeConfiguration.ApiKey = AppSettings.StripeKey();

               
                //attach payment to customer
                var options3 = new PaymentMethodAttachOptions
                {
                    Customer = appUserPaymentInfo.CustomerProfileId,
                };
                var service3 = new PaymentMethodService();
                var attachNewPayment = service3.Attach(
                    subscription.paymentMethod,
                    options3);

                if (attachNewPayment != null && attachNewPayment.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(attachNewPayment.Id))
                {
                            
                    //pay past invoices
                    bool allPaymentsSucceeded = true;

                    var options5 = new InvoiceListOptions
                    {
                        Status = "draft",
                    };
                    var service5 = new InvoiceService();
                    StripeList<Invoice> invoices = service5.List(
                        options5);

                    try
                    {
                        foreach(var invoice in invoices)
                        {
                            //update each invoice to auto-advance
                            var options6 = new InvoiceUpdateOptions
                            {
                                AutoAdvance = true,
                                DefaultPaymentMethod = attachNewPayment.Id
                            };
                            var service6 = new InvoiceService();
                            service6.Update(
                                invoice.Id,
                                options6);

                            //finalize the invoice
                            var service7 = new InvoiceService();
                            service7.FinalizeInvoice(
                                invoice.Id);


                        }

                        //get all open invoices
                        var options9 = new InvoiceListOptions
                        {
                            Status = "open",
                        };
                        var service9 = new InvoiceService();
                        StripeList<Invoice> invoices2 = service9.List(
                            options9);

                        foreach (var invoice in invoices2)
                        {
                            //payment method
                            var options61 = new InvoiceUpdateOptions
                            {
                                DefaultPaymentMethod = attachNewPayment.Id
                            };
                            var service61 = new InvoiceService();
                            service61.Update(
                                invoice.Id,
                                options61);

                            //pay the invoice
                            var service8 = new InvoiceService();
                            var invoicePaid = service8.Pay(invoice.Id);

                            if (invoicePaid == null || invoicePaid.Status.ToLower() != "paid")
                            {
                                allPaymentsSucceeded = false;
                            }

                        }
                    }
                    catch (StripeException se)
                    {
                        await ErrorLogging.LogError("Failed to pay outstanding invoices for customer on cc update. Error: " + se.ToString(), "UpdateCreditCard", JsonSerializer.Serialize(subscription));
                        simplePaymentResponse.Success = false;
                        simplePaymentResponse.Message = "Failed to pay outstanding invoices. Please try a different card.";
                        return simplePaymentResponse;
                    }
                    catch (Exception ex)
                    {
                        await ErrorLogging.LogError("Failed to pay outstanding invoices for customer on cc update. Error: " + ex.ToString(), "UpdateCreditCard", JsonSerializer.Serialize(subscription));
                    }

                    if (allPaymentsSucceeded)
                    {


                        //attach default payment method
                        var invoiceSettings = new CustomerInvoiceSettingsOptions
                        {
                            DefaultPaymentMethod = attachNewPayment.Id
                        };

                        var options31 = new CustomerUpdateOptions
                        {
                            InvoiceSettings = invoiceSettings
                        };
                        var service31 = new CustomerService();
                        var defaultPaymentMehod = service31.Update(appUserPaymentInfo.CustomerProfileId, options31);


                        if (defaultPaymentMehod != null && defaultPaymentMehod.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(defaultPaymentMehod.Id))
                        {
                            //attach payment to subscription
                            var options4 = new SubscriptionUpdateOptions
                            {
                                DefaultPaymentMethod = attachNewPayment.Id
                            };
                            var service4 = new SubscriptionService();
                            var paymentAttachedToSubscription = service4.Update(
                                appUserPaymentInfo.SubscriptionId,
                                options4);



                            if (paymentAttachedToSubscription != null && paymentAttachedToSubscription.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(paymentAttachedToSubscription.Id))
                            {
                                //detach old payment from customer
                                var service2 = new PaymentMethodService();
                                var detachValue = service2.Detach(appUserPaymentInfo.AssignedCustomerPaymentProfileId);

                                if (detachValue != null && detachValue.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(detachValue.Id))
                                {
                                    //nothing to do but send error on failure
                                }
                                else
                                {
                                    await ErrorLogging.LogError("Failed to detach payment method.", "UpdateCreditCard", JsonSerializer.Serialize(subscription));
                                }

                                simplePaymentResponse.Success = true;
                                SaveData sd = new SaveData();
                                var updateResponse = await sd.UpdateSubscriptionStatusToActive(Convert.ToInt32(appUserId), "active", CCLastFour, subscription.paymentMethod, attachNewPayment.Id);

                            }
                            else
                            {
                                simplePaymentResponse.Message = "Failed to attach default payment method";
                                return simplePaymentResponse;
                            }
                        }
                        else
                        {
                            simplePaymentResponse.Message = "Failed to update subscription. Please try a different card.";
                            simplePaymentResponse.StatusCode = "0";
                            await ErrorLogging.LogError("Failed to update payment method on subscription.", "UpdateCreditCard", JsonSerializer.Serialize(subscription));

                        }
                    }
                    else
                    {
                        simplePaymentResponse.Message = "Failed to pay outstanding invoices. Please try a different card.";
                        simplePaymentResponse.Success = false;
                        SaveData sd = new SaveData();
                        var updateResponse = await sd.UpdateSubscriptionStatusToActive(Convert.ToInt32(appUserId), appUserPaymentInfo.SubscriptionStatus, CCLastFour, subscription.paymentMethod, attachNewPayment.Id);

                    }
                }
                else
                {
                    simplePaymentResponse.Message = "Failed to update credit card. Please contact support.";
                    simplePaymentResponse.StatusCode = "0";
                    await ErrorLogging.LogError("Failed to attach payment method.", "UpdateCreditCard", JsonSerializer.Serialize(subscription));

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
                await ErrorLogging.LogError(ex.ToString(), "UpdateCreditCard", JsonSerializer.Serialize(subscription));
                simplePaymentResponse.Success = false;
                simplePaymentResponse.Message = "Failed to update credit card.";
            }

            return simplePaymentResponse;
        }
    }
}
