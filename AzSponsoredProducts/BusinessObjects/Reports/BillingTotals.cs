using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class BillingTotals
    {
        public decimal Cost { get; set; }
        public int CountryId { get; set; }
        public int AppUserId { get; set; }
        public DateTime ReportMonthDate { get; set; }
        public bool? Charged { get; set; }
        public string PaymentProfile { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public bool ChargeOverage { get; set; } = true;

    }
}
