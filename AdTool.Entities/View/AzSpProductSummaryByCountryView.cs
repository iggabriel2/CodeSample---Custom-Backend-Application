namespace AdTool.WebUI.Models.AzSp.ProductConfig
{
    public class AzSpProductSummaryByCountryView
    { 
        public int AzSpCountryCampConfigId { get; set; }
        public int QAPProductId { get; set; }
        public Guid ClientId { get; set; }
        public int CountryId { get; set; }
        public int? ConversionGoal { get; set; }
        public string Country { get; set; }
        public int ActiveResearchCampaignsCount { get; set; }
        public string Tier1CampaignState { get; set; }
        public string PerformanceCampaignState { get; set; }
    }
}
