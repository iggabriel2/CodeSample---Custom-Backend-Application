using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class AppUserPaymentInfo
    {
        public int Id { get; set; }

        public int? AppUserId { get; set; }

        public DateTime? StartDate { get; set; }

        public string SubscriptionId { get; set; }

        public string CustomerProfileId { get; set; }

        public string CustomerPaymentProfileId { get; set; }

        public bool SubscriptionActive { get; set; }

        public string SubscriptionStatus { get; set; }

        public DateTime? CancellationDate { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public string CCLastFour { get; set; }
    }
}
