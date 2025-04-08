using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class PaymentPlanPricingView
    {
        public int PlanId { get; set; }
        public int ScheduleId { get; set; }
        public decimal Price { get; set; }
        public string PlanName { get; set; }
        public string Schedule { get; set; }
        public string ApiId { get; set; }
        public string TestApiId { get; set; }
    }
}
