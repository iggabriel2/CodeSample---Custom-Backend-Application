using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class AzSpCountryCampaignConfigView
    {
        public int AzSpCountryCampConfigId { get; set; }

        public int QAPProductId { get; set; }

        public int CountryId { get; set; }

        public int? BiddingStrategyId { get; set; }

        public int? TopOfSearch { get; set; }

        public int? ProductPages { get; set; }

        public int ResearchPortfolioId { get; set; }

        public bool UseTier1 { get; set; }

        public int Tier1CampaignId { get; set; }

        public int? Tier1TresholdSales { get; set; }

        public int? Tier1TresholdPageReads { get; set; }

        public bool UsePerformance { get; set; }

        public int PerformanceCampaignId { get; set; }

        public int? PerformTresholdSales { get; set; }

        public int? PerformTresholdPageReads { get; set; }

        public bool ApplyNegative { get; set; }

        public int? ConversionGoal { get; set; }

        public decimal? Tier1DefaultBid { get; set; }

        public decimal? Tier1DefaultBudget { get; set; }

        public decimal? ResearchDefaultBid { get; set; }

        public decimal? ResearchDefaultBudget { get; set; }

        public decimal? PerformanceDefBid { get; set; }

        public decimal? PerformanceDefBudget { get; set; }

        public int? Tier1PortfolioId { get; set; }
        public int? PerformancePortfolioId { get; set; }
        public bool ExcludeAudibleKeywordsFromNegative { get; set; }
        public string ResearchPortfolioName { get; set; }
        public string Tier1CampaignName { get; set; }
        public string Tier1PortfolioName { get; set; }
        public string PerformanceCampaignName { get; set; }
        public string PerformancePortfolioName { get; set; }
        public Guid ClientId { get; set; }

        public string Tier1CampaignState { get; set; }
        public string PerformanceCampaignState { get; set; }
        public int? TargetACOS { get; set; }
        public AzSpCountryCampaignConfigView()
        {
            Tier1CampaignName = string.Empty;
            Tier1PortfolioName = string.Empty;
            PerformanceCampaignName = string.Empty;
            PerformancePortfolioName = string.Empty;
            ResearchPortfolioName = string.Empty;
        }
    }
}
