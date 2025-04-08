using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class KeywordPerformanceByCampaign
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
        public string Clicks { get; set; }
        public string Spend { get; set; }
        public string CTC { get; set; }
        public string KindlePageReads { get; set; }
        public string Orders { get; set; }
        public string Sales { get; set; }
    }
}
