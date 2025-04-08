using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class PaymentPlanPricing
    {
        public int PlanId { get; set; }
        public int ScheduleId { get; set; }
        public decimal Price { get; set; }
    }
}
