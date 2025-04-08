using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.BusinessObjects
{
    public class Message
    {
        public string code { get; set; }
        public string text { get; set; }
    }

    public class Messages
    {
        public string resultCode { get; set; }
        public List<Message> message { get; set; }
        public Messages() { 
            message = new List<Message>();
        }
    }

    public class Profile
    {
        public string customerProfileId { get; set; }
        public string customerPaymentProfileId { get; set; }
        public string customerAddressId { get; set; }
    }

    public class SubscriptionResponse
    {
        public string subscriptionId { get; set; }
        public Profile profile { get; set; }
        public string refId { get; set; }
        public Messages messages { get; set; }
        public SubscriptionResponse()
        {
            profile = new Profile();
            messages = new Messages();
        }
    }

}
