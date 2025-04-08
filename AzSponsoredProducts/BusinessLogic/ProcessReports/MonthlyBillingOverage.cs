using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using Google.Ads.GoogleAds.V11.Services;
using Google.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdTool.PaymentProcessor.BusinessObjects;
using AdTool.PaymentProcessor.Utils;
using System.Transactions;
using Configuration;
using Stripe;
using AdTool.Entities.Edit;
using AdTool.Entities.Payments;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class MonthlyBillingOverage
    {
        public async Task<bool?> BillOverage(Guid ClientId)
        {
            //try
            //{
            //    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            //    ExchnageRateApiUtils exchnageRateApiUtils = new ExchnageRateApiUtils();
            //    httpResponseMessage = await exchnageRateApiUtils.CallExchangeRateApi();

            //    if (httpResponseMessage.IsSuccessStatusCode)
            //    {
            //        //get exchange rates
            //        var exchangeRates = await JsonSerializer.DeserializeAsync<ExchnageRates>(httpResponseMessage.Content.ReadAsStream());

            //        if (exchangeRates == null || exchangeRates.success != true)
            //        {
            //            await ErrorLogging.LogError("Failure code: " + httpResponseMessage.IsSuccessStatusCode.ToString(), "BillOverage - top level", "NA");
            //            return null;
            //        }

            //        List<BillingTotals> billingTotals = new List<BillingTotals>();

            //        //get this app user with money due
            //        RetrieveReportData retrieveReportData = new RetrieveReportData();
            //        billingTotals = await retrieveReportData.GetOverageAmounts(ClientId);

            //        //get unique appusers
            //        List<int> appUsers = new List<int>();
            //        appUsers = billingTotals.Select(t => t.AppUserId).Distinct().ToList();

            //        //for each user - 
            //        foreach(var appUserId in appUsers)
            //        {
            //            //unique months to calculate
            //            List<System.DateTime> monthsToCalculate = new List<System.DateTime>();
            //            monthsToCalculate = billingTotals.Where(x => x.AppUserId == appUserId).Select(t => t.ReportMonthDate).Distinct().ToList();

            //            foreach(var reportMonth in monthsToCalculate)
            //            {
            //                //sum total spend by country
            //                List<BillingTotals> billingTotalsForThisUserAndMonth = new List<BillingTotals>();
            //                OverageAmounts overageForThisUser = new OverageAmounts();

            //                billingTotalsForThisUserAndMonth = billingTotals.Where(x => x.AppUserId == appUserId && x.ReportMonthDate == reportMonth).ToList();

            //                decimal totalOverage = 0;

            //                //convert to US dollars
            //                foreach (var billingTotal in billingTotalsForThisUserAndMonth)
            //                {
            //                    decimal exchangeRate = 0;

            //                    if (billingTotal.CountryId == 1)
            //                    {
            //                        decimal usAmount = billingTotal.Cost;
            //                        totalOverage += usAmount;
            //                    }
            //                    else if (billingTotal.CountryId == 2)
            //                    {
            //                        decimal ukAmount = billingTotal.Cost;
            //                        decimal ukExchangeRate = exchangeRates.rates.GBP;
            //                        decimal convertedAmount = await GeneralStaticUtils.SafeDivision(ukAmount, ukExchangeRate);
            //                        totalOverage += convertedAmount;
            //                    }
            //                    else if (billingTotal.CountryId == 3)
            //                    {
            //                        decimal caAmount = billingTotal.Cost;
            //                        decimal caExchangeRate = exchangeRates.rates.CAD;
            //                        decimal convertedAmount = await GeneralStaticUtils.SafeDivision(caAmount, caExchangeRate);
            //                        totalOverage += convertedAmount;
            //                    }
            //                    else if (billingTotal.CountryId == 4)
            //                    {
            //                        decimal auAmount = billingTotal.Cost;
            //                        decimal auExchangeRate = exchangeRates.rates.AUD;
            //                        decimal convertedAmount = await GeneralStaticUtils.SafeDivision(auAmount, auExchangeRate);
            //                        totalOverage += convertedAmount;
            //                    }
            //                }

            //                //deduct 1% to make sure we are not estimating higher than Amazon
            //                totalOverage = await GeneralStaticUtils.Round(totalOverage * (decimal).99);

            //                overageForThisUser.BillingMonth = reportMonth;
            //                overageForThisUser.Charged = false;
            //                overageForThisUser.AppUserId = appUserId;
            //                overageForThisUser.TotalUS = totalOverage;

            //                SaveReportData saveReportData = new SaveReportData();
            //                bool success = await saveReportData.EditOrUpdateBilledAmounts(overageForThisUser);

            //            }

            //            //if last month hasn't been billed after third of this month
            //            var thirdDayOfMonth = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 3);
            //            var month = new System.DateTime(thirdDayOfMonth.Year, thirdDayOfMonth.Month, 1);
            //            var firstDayOfPreviousMonth = month.AddMonths(-1);


            //            BillingTotals billingItemForThisUser = new BillingTotals();
            //            billingItemForThisUser = billingTotals.Where(x => x.AppUserId == appUserId).FirstOrDefault();

            //            OverageAmounts overageAmounts = new OverageAmounts();
            //            overageAmounts = await retrieveReportData.GetLastMonthTotalSpend(appUserId, firstDayOfPreviousMonth);

            //            if (System.DateTime.Now >= thirdDayOfMonth && billingItemForThisUser.ChargeOverage && overageAmounts != null && overageAmounts.TotalUS > 10000)
            //            {
            //                //charge customer profile through stripe for anything over 10k

            //                //get payment codes
            //                PaymentProcessor.Data.RetrieveData paymentProcessorData = new PaymentProcessor.Data.RetrieveData();
            //                var paymentcodes = await paymentProcessorData.GetPaymentInfo(appUserId);

            //                //get amount to charge
            //                decimal chargeAmountRaw = (overageAmounts.TotalUS - 10000) * (decimal).02;
            //                chargeAmountRaw = await GeneralStaticUtils.Round(chargeAmountRaw);


            //                decimal value = chargeAmountRaw * 100; //get it all to cents
            //                int chargeAmount = (int)value;

            //                StripeConfiguration.ApiKey = AppSettings.StripeKey();

            //                var options = new InvoiceItemCreateOptions
            //                {
            //                    Customer = paymentcodes.CustomerProfileId,
            //                    Amount = chargeAmount,
            //                    Currency = "usd",
            //                    Description = "Overage Charges"
            //                };
            //                var service = new InvoiceItemService();
            //                var overageCreation = service.Create(options);

            //                if (overageCreation != null && overageCreation.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(overageCreation.Id))
            //                {

            //                    //upon success, update billing table
            //                    OverageAmounts amountToUpdate = new OverageAmounts();
            //                    amountToUpdate.BillingMonth = firstDayOfPreviousMonth;
            //                    amountToUpdate.Charged = true;
            //                    amountToUpdate.AppUserId = appUserId;
            //                    amountToUpdate.TotalUS = overageAmounts.TotalUS;

            //                    SaveReportData saveReportData = new SaveReportData();
            //                    bool success = await saveReportData.EditOrUpdateBilledAmounts(amountToUpdate);

            //                    //send email to customer advising them of monthly overage charge

            //                    if (!string.IsNullOrEmpty(billingItemForThisUser.EmailAddress))
            //                    {
            //                        List<string> emails = new List<string>();
            //                        emails.Add(billingItemForThisUser.EmailAddress);
            //                        string subject = "Monthly Usage Billing";

            //                        string message = "Hi " + billingItemForThisUser.FirstName + ",<br/><br/>Last month, you spent $" + overageAmounts.TotalUS.ToString() + " on Amazon sponsored product ads across all markets we manage." +
            //                            "<br/><br/>As per the user agreement, we will bills 2% of the $" + (chargeAmount - 10000).ToString() + " that exceeds $10,000 for a total bill of $" + chargeAmount +
            //                            " on your next subscription renewal.<br/><br/>Thanks for being part of the FaktorIQ family!<br/><br/>FaktorIQ";

            //                        await EmailSender.sendEmail(message, subject, emails);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if (overageAmounts != null)
            //                {
            //                    //upon success, update billing table
            //                    OverageAmounts amountToUpdate = new OverageAmounts();
            //                    amountToUpdate.BillingMonth = firstDayOfPreviousMonth;
            //                    amountToUpdate.Charged = true;
            //                    amountToUpdate.AppUserId = appUserId;
            //                    amountToUpdate.TotalUS = overageAmounts.TotalUS;

            //                    SaveReportData saveReportData = new SaveReportData();
            //                    bool success = await saveReportData.EditOrUpdateBilledAmounts(amountToUpdate);
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        await ErrorLogging.LogError("Failure code: "+ httpResponseMessage.IsSuccessStatusCode.ToString(), "BillOverage - top level", "NA");
            //    }
            //}
            //catch(Exception ex)
            //{
            //    await ErrorLogging.LogError(ex.ToString(), "BillOverage - top level", "NA");
            //}

            return null;

        }
    }
}
