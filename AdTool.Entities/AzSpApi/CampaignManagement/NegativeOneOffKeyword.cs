using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class NegativeOneOffKeyword
    {
        public APIAuthorizationRequest Authorization { get; set; }
        public string SimpleKeywordType { get; set; }
        public string KeywordId { get; set; }
        public string KeywordType { get; set; }
        public string SearchTerm { get; set; }
        public int CountryId { get; set; }
        public string AzCampaignId { get; set; }
        public string AdGroup { get; set; }
        public NegativeOneOffKeyword()
        {
            Authorization = new APIAuthorizationRequest();
        }
    }
}
