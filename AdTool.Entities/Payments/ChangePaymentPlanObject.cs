using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Payments
{
    public class ChangePaymentPlanObject
    {
        public string AppUserId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string processDate { get; set; }

        //this needs to be sent so we know how to charge. api will update settings in appuser table
        public int PaymentPlan { get; set; }

        public bool isFreeTrial {get; set;}
        public string apiId { get; set; }
        public bool isPlanChange { get; set; }

    }
}
