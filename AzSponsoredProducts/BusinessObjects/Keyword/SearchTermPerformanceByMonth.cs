using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class SearchTermPerformanceByMonth
    {
        public string SearchTerm { get; set; }
        public decimal Cost { get; set; }
        public int Clicks { get; set; }
        public int Impressions { get; set; }
        public int CountryId { get; set; }
        public long PageReads { get; set; }
        public int Purchases14d { get; set; }
        public string Keyword { get; set; }
        public string KeywordType { get; set; }
        public string CampaignId { get; set; }
        public bool Negative { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CountryName { get; set; }
        public string CampaignName { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal CPC { get; set; }
        public string CampaignState { get; set; }
        public string AdGroup { get; set; }
        public string KeywordId { get; set; }
        public decimal AttributedSalesSameSku14d { get; set; }
        public bool Reviewed { get; set; } = false;

    }
}
