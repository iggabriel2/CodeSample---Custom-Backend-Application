using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class OverageAmounts
    {
        public int? AppUserId { get; set; }
        public DateTime BillingMonth { get; set; }
        public decimal TotalUS { get; set; }
        public bool? Charged { get; set; }

    }
}
