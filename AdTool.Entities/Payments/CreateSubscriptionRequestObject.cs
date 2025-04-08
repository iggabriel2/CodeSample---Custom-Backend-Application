using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Payments
{
    public class CreateSubscriptionRequestObject
    {
        public string AppUserId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fourDigits { get; set; }
        public string paymentMethod { get; set; }
        public string processDate { get; set; }

        //options are monthly and yearly - this is now controlled by the product id in Stripe and saved to our db here
        public string? paymentSchedule { get; set; }
        public string apiId { get; set; }
        public string? couponApi { get; set; }
        public bool isFreeTrial { get; set; }

    }
}
