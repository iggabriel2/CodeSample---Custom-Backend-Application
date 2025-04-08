using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.D4Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.CampaignsAdGroups
{
    public class UpdateAdGroupResponse
    {
        public bool Success { get; set; } = false;
        public APIAuthorization APIAuthorization { get; set; }
        public UpdateAdGroupResponse()
        {
            APIAuthorization = new APIAuthorization();
        }
    }
}
