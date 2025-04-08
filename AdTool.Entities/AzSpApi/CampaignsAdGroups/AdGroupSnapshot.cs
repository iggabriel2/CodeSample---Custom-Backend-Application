using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.CampaignsAdGroups
{
    public class AdGroupSnapshot
    {
        public long adGroupId { get; set; }
        public string name { get; set; }
        public long campaignId { get; set; }
        public decimal defaultBid { get; set; }
        public string state { get; set; }
        public string ClientId { get; set; }
        public int CountryId { get; set; }

        //ClientId-CountryId
        public string partitionKey { get; set; }

        //ClientId-CountryId-CampaignId-adGroupId
        public string id { get; set; }
    }
}
