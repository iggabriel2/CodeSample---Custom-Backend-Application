using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class SearchTermPerformanceRequest
    {
        public int CountryId { get; set; } = 0;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public APIAuthorizationRequest Authorization { get; set; }
        public SearchTermPerformanceRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
