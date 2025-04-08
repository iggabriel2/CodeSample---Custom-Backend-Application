using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class KeywordPerformanceByMonth
    {
        public string KeywordId { get; set; }
        public decimal Cost { get; set; } = 0;
        public int Clicks { get; set; } = 0;
        public string KeywordType { get; set; }
        public int Impressions { get; set; } = 0;
        public int KindleEditionNormalizedPagesRead14d { get; set; } = 0;
        public int purchases14d { get; set; } = 0;
        public int Country { get; set; }
        public Guid ClientId { get; set; }
        public DateTime ReportMonthDate { get; set; }
        public string CampaignId { get; set; }
        public string AdGroupId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CountryName { get; set; }
        public string CampaignName { get; set; }
        public decimal ConversionRate { get; set; } = 0;
        public decimal CPC { get; set; } = 0;
        public string CampaignState { get; set; }
        public int QAPCampaignId { get; set; }
        public decimal AttributedSalesSameSku14d { get; set; }
        public string UsageType { get; set; }
        public decimal CTR { get;set; } = 0;
        public int ACOS { get;set ;} = 0;

    }
}
