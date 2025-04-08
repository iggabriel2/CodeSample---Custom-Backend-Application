using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports.Summary.Response
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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

    public class ReportRequestResponseRoot
    {
        public Configuration configuration { get; set; }
        public DateTime createdAt { get; set; }
        public string endDate { get; set; }
        public object failureReason { get; set; }
        public object fileSize { get; set; }
        public object generatedAt { get; set; }
        public string name { get; set; }
        public string reportId { get; set; }
        public string startDate { get; set; }
        public string status { get; set; }
        public DateTime updatedAt { get; set; }
        public object url { get; set; }
        public object urlExpiresAt { get; set; }
    }
}
