using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignsAdGroups
{
    public class GetCampaignRequestApi
    {
        public int? countryId { get; set; }

        public string? campaignName { get; set; }
        public string? campaignStatus { get; set; }
        public int? productId { get; set; }
        public DateTime? monthYearFrom { get; set; }
        public DateTime? monthYearTo { get; set; }
        public int? campaignUsage { get; set; }
        public Guid? clientId { get; set; }

        public APIAuthorizationRequest Authorization { get; set; }
        public GetCampaignRequestApi()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
