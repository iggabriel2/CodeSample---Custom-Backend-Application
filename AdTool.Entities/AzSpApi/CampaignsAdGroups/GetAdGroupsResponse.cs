using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.CampaignsAdGroups
{
    public class GetAdGroupsResponseAPI
    {
        public APIAuthorization APIAuthorization { get; set; }
        public List<AdGroupSnapshotResponse> AdGroups { get; set; }
        public GetAdGroupsResponseAPI()
        {
            APIAuthorization = new APIAuthorization();
            AdGroups = new List<AdGroupSnapshotResponse>();
        }
    }
}
