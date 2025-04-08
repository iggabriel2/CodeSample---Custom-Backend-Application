using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class AllCampaigns
    {
        public string CountryId { get; set; }
        public string Country { get; set; }
        public int QAPCampaignId { get; set; }
        public string AZCampaignId { get; set; }
        public string CampaignName { get; set; }
        public int QAPProductId { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public string Usage { get; set; }
        public int Clicks { get; set; } = 0;
        public decimal Spend { get; set; } = 0;
        public decimal CTC { get; set; } = 0;
        public int KindlePageReads { get; set; } = 0;
        public int Orders { get; set; } = 0;
        public decimal Sales { get; set; } = 0;
        public int UsageTypeId { get; set; }
        public int Impressions { get; set; } = 0;
    }
}
