using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.BusinessObjects
{
    public class ProcessSubscriptionAlertObject
    {
        public string? notificationId { get; set; }
        public string? eventType { get; set; }
        public DateTime? eventDate { get; set; }
        public string? webhookId { get; set; }
        public Payload payload { get; set; }
        public ProcessSubscriptionAlertObject()
        {
            payload = new Payload();
        }
    }

    public class Payload
    {
        public string? entityName { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
        public decimal? amount { get; set; }
        public string? status { get; set; }
        public SubscriptionAlertProfile? profile { get; set; }
        public Payload()
        {
            profile = new SubscriptionAlertProfile();
        }
    }

    public class SubscriptionAlertProfile
    {
        public int? customerProfileId { get; set; }
        public int? customerPaymentProfileId { get; set; }
        public int? customerShippingAddressId { get; set; }
    }

}
