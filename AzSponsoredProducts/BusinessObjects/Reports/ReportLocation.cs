using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class ReportLocation
    {
        public class Configuration
        {
            public string adProduct { get; set; }
            public List<string> columns { get; set; }
            public List<Filter> filters { get; set; }
            public string format { get; set; }
            public List<string> groupBy { get; set; }
            public string reportTypeId { get; set; }
            public string timeUnit { get; set; }

            public Configuration()
            {
                columns = new List<string>();
                filters = new List<Filter>();
                groupBy = new List<string>();
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

        public class ReportLocationRoot
        {
            public Configuration configuration { get; set; }
            public DateTime? createdAt { get; set; }
            public string endDate { get; set; }
            public object failureReason { get; set; }
            public int? fileSize { get; set; }
            public DateTime? generatedAt { get; set; }
            public string name { get; set; }
            public string reportId { get; set; }
            public string startDate { get; set; }
            public string status { get; set; }
            public DateTime? updatedAt { get; set; }
            public string url { get; set; }
            public DateTime? urlExpiresAt { get; set; }
        }

    }
}
