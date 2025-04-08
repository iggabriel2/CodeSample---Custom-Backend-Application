using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignCreations
{
    public class CampaignResponse
    {
        public List<CountrySuccessOnCampaigns> CountrySuccess { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public CampaignResponse()
        {
            APIAuthorization = new APIAuthorization();
            CountrySuccess = new List<CountrySuccessOnCampaigns>();
        }
    }

    public class CountrySuccessOnCampaigns
    {
        public bool Success { get; set; }
        public int CountryId { get; set; }
        public List<string> RejectedKeywords { get; set; }
        public int DuplicateKeywords { get; set; }
        public List<string> InvalidAsins { get; set; }
        public int QapId { get; set; }
    }
}
