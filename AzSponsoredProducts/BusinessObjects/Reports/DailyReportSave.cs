using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class DailyReportSave
    {
        public DateTime ReportDate { get; set; }
        public int Impressions { get; set; }
        public int Clicks { get; set; }
        public int Orders { get; set; }
        public decimal CPC { get; set; }
        public Guid ClientId { get; set; }
        public int CountryId { get; set; }
        public decimal Cost { get; set; }
    }
}
