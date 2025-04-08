using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class PromoNegativeRules
    {
        public bool ExcludeAudibleKeywordsFromNegative { get; set; }
        public bool UseTier1 { get; set; }
        public bool UsePerformance { get; set; }
        public int CountryID { get; set; }
        public int Tier1TresholdSales { get; set; }
        public int Tier1TresholdPageReads { get; set; }
        public int PerformTresholdSales { get; set; }
        public int PerformTresholdPageReads { get; set; }
        public bool ApplyNegative { get; set; }
        public int ConversionGoal { get; set; }
        public int QAPProductID { get; set; }
        public Guid ClientId { get; set; }
        public decimal Tier1DefaultBid { get; set; }
        public decimal PerformanceDefBid { get; set; }
        public decimal? TargetACOS { get; set; }

    }
}
