using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Save
{
    public class CampaignSave
    {
        public string AZCampaignId { get; set; }
        public string AzPortfolioId { get; set; }
        public int ProductId { get; set; }
        public string CampaignName { get; set; }
        public int CountryId { get; set; }
        public bool Active { get; set; }
        public Guid AzClientId { get; set; }
        public int AzSpCampaignUsageType { get; set; }
        public bool AzSpPrimaryInUsageType { get; set; }
        public bool GeneratedByUs { get; set; }
        public decimal Budget { get; set; }
        public string State { get; set; } = "enabled";
        public string TargetingType { get; set; }
        public int DynamicBiddingStrategy { get; set; }
    }

    public class CampaignSaveBatch
    {
        public Guid BulkId { get; set; }
        public string AZCampaignId { get; set; }
        public string AzPortfolioId { get; set; }
        public int ProductId { get; set; }
        public string CampaignName { get; set; }
        public int DynamicBiddingStrategy { get; set; }
        public int CountryId { get; set; }
        public bool Active { get; set; }
        public Guid AzClientId { get; set; }
        public int AzSpCampaignUsageType { get; set; }
        public bool AzSpPrimaryInUsageType { get; set; }
        public bool GeneratedByUs { get; set; }
        public decimal Budget { get; set; }
        public string State { get; set; }
        public string TargetingType { get; set; }
    }
}
