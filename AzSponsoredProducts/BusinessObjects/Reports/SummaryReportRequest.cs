using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports.Summary
{
    public class Configuration
    {
        public string adProduct { get; set; }
        public List<string> groupBy { get; set; }
        public List<string> columns { get; set; }
        public List<Filter> filters { get; set; }
        public string reportTypeId { get; set; }
        public string timeUnit { get; set; }
        public string format { get; set; }

        public Configuration()
        {
            groupBy = new List<string>();
            columns = new List<string>();
            filters = new List<Filter>();
        }
    }

    public class Filter
    {
        public string field { get; set; }
        public List<string> values { get; set; }

        public Filter()
        {
            values = new List<string>();
        }
    }

    public class RootSummaryReportRequest
    {
        public string name { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public Configuration configuration { get; set; }
    }
}
