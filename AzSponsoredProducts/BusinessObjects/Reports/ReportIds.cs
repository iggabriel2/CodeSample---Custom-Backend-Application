using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class ReportIds
    {
        public string DailyReportId { get; set; }
        public string DailyReportUrl { get; set; }
        public bool DailyUrlAcquired { get; set; } = false;

        public string MonthlyReportId { get; set; }
        public string MonthlyReportUrl { get; set; }
        public bool MonthlyUrlAcquired { get; set; } =false;

        public string LastMonthlyReportId { get; set; }
        public string LastMonthlyReportUrl { get; set; }
        public bool LastMonthUrlAcquired { get; set; } = false;

        public string KeywordMonthlyReportId { get; set; }
        public string KeywordMonthlyReportUrl { get; set; }
        public bool KeywordMonthlyUrlAcquired { get; set; } = false;

        public string KeywordLastMonthlyReportId { get; set; }
        public string KeywordLastMonthlyReportUrl { get; set; }
        public bool KeywordLastMonthUrlAcquired { get; set; } = false;

        public string KeywordSnapshotId { get; set; }
        public string KeywordSnapshotUrl { get; set; }
        public bool KeywordSnapshotUrlAcquired { get; set; } = false;

        public string AdGroupSnapshotId { get; set; }
        public string AdGroupSnapshotUrl { get; set; }
        public bool AdGroupSnapshotUrlAcquired { get; set; } = false;

        public string ProductTargetSnapshotId { get; set; }
        public string ProductTargetSnapshotUrl { get; set; }
        public bool ProductTargetSnapshotUrlAcquired { get; set; } = false;

        public bool ProcessLastMonth { get; set; } = false;
        public ClientProfileCodes ClientProfileCode { get; set; }
        public ReportIds() { 
            ClientProfileCode = new ClientProfileCodes();
        }
    }
}
