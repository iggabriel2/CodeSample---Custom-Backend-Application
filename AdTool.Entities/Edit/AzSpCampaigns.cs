using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class AzSpCampaigns
    {

        public int Id { get; set; }

        public string AZCampaignId { get; set; }

        public int? ProductId { get; set; }

        public string CampaignName { get; set; }

        public int? CountryId { get; set; }

        public bool? Active { get; set; }

        public int? AzSpCampaignUsageType { get; set; }

        public bool? AzSpPrimaryInUsageType { get; set; }

        public string AzPortfolioId { get; set; }

        public bool? GeneratedByUs { get; set; }

        public Guid? azClientId { get; set; }

        public bool IncludeInKeywordManagement { get; set; }

        public int? QAPPortfolioId { get; set; }

        public decimal? Budget { get; set; }

        public int? DynamicBiddingStrategy { get; set; }

        public string? State { get; set; }
        public string? TargetingType { get; set; }


    }
}
