using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.BusinessObjects
{
    public class AppUserPaymentInfo
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public DateTime StartDate { get; set; }
        public string SubscriptionId { get; set; }
        public string CustomerProfileId { get; set; }
        public string CustomerPaymentProfileId { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string SubscriptionStatus { get; set; }
        public string LastPaymentDate { get; set; }
        public string AssignedCustomerPaymentProfileId { get; set; }
        public string AssignedStripeProductId { get; set; }
        public string SubscriptionItemId { get; set; }
    }

    public class AppUserPaymentInfoInsert
    {
        public int AppUserId { get; set; }
        public string SubscriptionId { get; set; } = "";
        public string CustomerProfileId { get; set; } = "";
        public string CustomerPaymentProfileId { get; set; } = "";
        public string CCLastFour { get; set; }
        public string AssignedCustomerPaymentProfileId { get; set; }
        public string AssignedStripeProductId { get; set; }
        public string SubscriptionItemId { get; set; }
    }
}
