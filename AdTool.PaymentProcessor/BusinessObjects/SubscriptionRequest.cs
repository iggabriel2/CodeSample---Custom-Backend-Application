using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.BusinessObjects
{
    public class ARBCreateSubscriptionRequest
    {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public Subscription subscription { get; set; }
        public ARBCreateSubscriptionRequest() { 
            merchantAuthentication = new MerchantAuthentication();
            subscription = new Subscription();
        }
    }

    public class BillTo
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class CreditCard
    {
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string cardCode { get; set; }
    }

    public class Interval
    {
        public string length { get; set; }
        public string unit { get; set; }
    }

    public class Payment
    {
        public CreditCard creditCard { get; set; }
        public Payment()
        {
            creditCard = new CreditCard();
        }
    }

    public class PaymentSchedule
    {
        public Interval interval { get; set; }
        public string startDate { get; set; }
        public string totalOccurrences { get; set; }
        public PaymentSchedule()
        {
            interval = new Interval();
        }
    }

    public class SubscriptionRequest
    {
        public ARBCreateSubscriptionRequest ARBCreateSubscriptionRequest { get; set; }
        public SubscriptionRequest()
        {
            ARBCreateSubscriptionRequest = new ARBCreateSubscriptionRequest();
        }
    }

    public class Subscription
    {
        public string name { get; set; } = "FaktorIQ";
        public PaymentSchedule paymentSchedule { get; set; }
        public string amount { get; set; }
        public Payment payment { get; set; }
        public BillTo billTo { get; set; }
        public Subscription()
        {
            billTo = new BillTo();
            paymentSchedule = new PaymentSchedule();
            payment = new Payment();
        }
    }


}
