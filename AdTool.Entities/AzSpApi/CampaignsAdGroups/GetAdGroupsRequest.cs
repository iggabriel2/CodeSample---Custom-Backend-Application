using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.CampaignsAdGroups
{
    public class GetAdGroupsRequest
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string CampaignId { get; set; }

        //do not send from frontend. this is for backend use only
        public int CountryId { get; set; } = 0;
        public APIAuthorizationRequest Authorization { get; set; }
        public GetAdGroupsRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
