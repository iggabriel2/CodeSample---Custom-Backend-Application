using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.CampaignsAdGroups
{
    public class UpdateAdGroupRequest
    {
        public string name { get; set; }

        //options: "ENABLED" "PAUSED"
        public string state { get; set; }
        public string adGroupId { get; set; }
        public string campaignId { get; set; }
        public decimal defaultBid { get; set; }
        public int CountryId { get; set; }
        public APIAuthorizationRequest Authorization { get; set; }
        public UpdateAdGroupRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
