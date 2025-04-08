using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class KeywordRequestByAdGroup
    {
        public string AdGroupId { get; set; }
        public int CountryId { get; set; } = 0;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public APIAuthorizationRequest Authorization { get; set; }
        public KeywordRequestByAdGroup()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
