using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement
{
    public class AllProducts
    {
        public int QAPProductId { get; set; }
        public string Asin { get; set; }
        public string ProductName { get; set; }
        public string AvailableCountries { get; set; }
        public string AzImageURL { get; set; }
        public Guid ClientId { get; set; }
        public string CountryList { get; set; }
        public string CampaignCount { get; set; }
        public string ClickThroughRate { get; set; }
        public string KindleEditionNormalizedPagesRead14d { get; set; }
        public string purchases14d { get; set; }
        public string AttributedSalesSameSku14d { get; set; }
        public string UnitsSoldClicks14d { get; set; }
        public string ConversionRate { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
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
