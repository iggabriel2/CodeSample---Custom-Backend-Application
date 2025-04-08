using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignCreations
{
    public class CampaignNameRequest
    {
        public string CampaignName { get; set; }
        public List<int> RequestedCountries { get; set; }
        public APIAuthorizationRequest Authorization { get; set; }
        public CampaignNameRequest()
        {
            Authorization = new APIAuthorizationRequest();
            RequestedCountries = new List<int>();
        }
    }
}
