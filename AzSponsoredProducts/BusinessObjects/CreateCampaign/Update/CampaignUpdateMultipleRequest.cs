using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update
{
    public class CampaignUpdateMultipleRequestObject
    {
        //public string portfolioId { get; set; }
        //public string endDate { get; set; }
        public string campaignId { get; set; }
        //public string name { get; set; }
        //public string targetingType { get; set; }
        //public string state { get; set; }
        public DynamicBiddingMultiple dynamicBidding { get; set; }
        //public string startDate { get; set; }
        //public Budget budget { get; set; }
        //public Tags tags { get; set; }
        public CampaignUpdateMultipleRequestObject()
        {
            dynamicBidding = new DynamicBiddingMultiple();
            //budget = new Budget();
            //tags = new Tags();
        }
    }

    public class DynamicBiddingMultiple
    {
        //public List<PlacementBidding> placementBidding { get; set; }
        public string strategy { get; set; }
        //public DynamicBidding()
        //{
        //    placementBidding = new List<PlacementBidding>();
        //}
    }

    public class CampaignUpdateRequestAzApiMultiple
    {
        public List<CampaignUpdateMultipleRequestObject> campaigns { get; set; }
        public CampaignUpdateRequestAzApiMultiple()
        {
            campaigns = new List<CampaignUpdateMultipleRequestObject>();
        }
    }
}
