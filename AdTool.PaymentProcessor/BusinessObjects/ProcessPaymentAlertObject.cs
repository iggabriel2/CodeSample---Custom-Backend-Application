using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.BusinessObjects
{
    public class PaymentPayload
    {
        public int? responseCode { get; set; }
        public string? merchantReferenceId { get; set; }
        public string? authCode { get; set; }
        public string? avsResponse { get; set; }
        public decimal? authAmount { get; set; }
        public string? entityName { get; set; }
        public string? id { get; set; }
    }

    public class ProcessPaymentAlertObject
    {
        public string? notificationId { get; set; }
        public string? eventType { get; set; }
        public DateTime? eventDate { get; set; }
        public string? webhookId { get; set; }
        public PaymentPayload payload { get; set; }
        public ProcessPaymentAlertObject()
        {
            payload = new PaymentPayload();
        }
    }


}
