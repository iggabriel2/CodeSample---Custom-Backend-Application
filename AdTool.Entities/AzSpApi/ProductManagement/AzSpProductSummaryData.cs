using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.ProductManagement
{
    public class AzSpProductSummaryData
    {
        public string CountryList { get; set; }
        public string Asin { get; set; }
        public string AzImageUrl { get; set; }
        public string ResearchCampaigns { get; set; }
        public int QAPProductId { get; set; }
        public string ProductName { get; set; }
        public string Clicks { get; set; }
        public decimal Spend { get; set; } = 0;
        public decimal CTC { get; set; } = 0;
        public string KindlePageReads { get; set; }
        public string Orders { get; set; }
        public decimal Sales { get; set; } = 0;
        public int Impressions { get; set; } = 0;
        public decimal CTR { get; set; } = 0;
        public decimal Conversion { get; set; } = 0;
        public decimal ACOS { get; set; } = 0;
    }
}
