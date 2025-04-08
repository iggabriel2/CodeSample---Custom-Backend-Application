using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class CampaignUpdateMultipleRequest
    {
        public List<CampaignToUpdate> CampaignsToUpdate { get; set; }
        public APIAuthorizationRequest Authorization { get; set; }
        public int CountryId { get; set; }

        //do not send
        //public string TargetingType { get; set; }
        public CampaignUpdateMultipleRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }

    public class CampaignToUpdate
    {

        //public string CampaignName { get; set; }
        public string CampaignId { get; set; }
        //public decimal Budget { get; set; }
        //FUTURE: public int TopOfSearch { get; set; }
        //FUTURE: public int ProductPages { get; set; }

        //down, updown, or manual, like when you create a campaign. In the db, dynamicbiddingstrategy is 1 for down, 2 for updown, and 3 for manual
        public string DynamicBiddingStrategy { get; set; }
        //options: "ENABLED" "PAUSED"
        //public string state { get; set; }
    }
}
