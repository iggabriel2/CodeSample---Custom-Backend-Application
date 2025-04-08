using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class BidChangeRequest
    {
        //if you want to udpate a specific campaign, send the campaign id. If you want to update an ad group, send the campaign id and the ad group id.
        public decimal bid { get; set; }

        //options are "up", "down", and "change". up - adusts current bid up by bid amount. down - adjusts current bid down by bid amount. change - changes current bid to new bid
        public string AdjustCurrentBid { get; set; } = "change";
        public int CountryId { get; set; }
        public int CampaignUsageType { get; set; }
        public List<string>? CampaignId { get; set; }
        public string? AdGroupId { get; set; }
        public int ProductId { get; set; }
        public APIAuthorizationRequest Authorization { get; set; }
        public BidChangeRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
