using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignsAdGroups
{
    public class GetCampaignResponseApi
    {
        public APIAuthorization APIAuthorization { get; set; }
        public List<AzSpCampaignSummary> CampaignSummaryData { get; set; }
        public GetCampaignResponseApi()
        {
            APIAuthorization = new APIAuthorization();
            CampaignSummaryData = new List<AzSpCampaignSummary>();
        }
    }
}
