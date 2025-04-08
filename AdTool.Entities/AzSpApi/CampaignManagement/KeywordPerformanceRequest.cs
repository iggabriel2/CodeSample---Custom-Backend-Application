using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class KeywordPerformanceRequest
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public int CountryId { get; set; } = 0;
        public APIAuthorizationRequest Authorization { get; set; }
        public KeywordPerformanceRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
