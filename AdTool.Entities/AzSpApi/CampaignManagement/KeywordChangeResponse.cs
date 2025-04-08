using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class KeywordChangeResponse
    {
        public bool Success { get; set; } = false;
        public APIAuthorization APIAuthorization { get; set; }
        public KeywordChangeResponse()
        {
            APIAuthorization = new APIAuthorization();
        }
    }
}
