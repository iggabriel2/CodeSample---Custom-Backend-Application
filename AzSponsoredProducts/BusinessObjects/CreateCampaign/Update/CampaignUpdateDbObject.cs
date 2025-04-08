using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update
{
    public class CampaignUpdateDbObject
    {
        public string CampaignId { get; set; }
        public int CountryId { get; set; }
        public Guid ClientId { get; set; }
        public string State { get; set; }

        public decimal Budget { get; set; }

        public string CampaignName { get; set; }
        public int DynamicBiddingStrategy { get; set; }
        public bool Active { get; set; }
    }
}
