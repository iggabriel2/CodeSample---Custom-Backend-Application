using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create
{
    public class AdGroupPairs
    {
        public string AzAdGroupName { get; set; }
        public string AzAdGroupId { get; set; }
        public int AzAdGroupUsageType { get; set; }
        public string AzSpCampaignId { get; set; }
        public Guid ClientId { get; set; }
        public int CountryId { get; set; }

    }
}
