using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update
{
    public class Budget
    {
        public string budgetType { get; set; }
        public decimal budget { get; set; }
    }

    public class Campaign
    {
        //public string portfolioId { get; set; }
        //public string endDate { get; set; }
        public string campaignId { get; set; }
        public string name { get; set; }
        public string targetingType { get; set; }
        public string state { get; set; }
        public DynamicBidding dynamicBidding { get; set; }
        //public string startDate { get; set; }
        public Budget budget { get; set; }
        //public Tags tags { get; set; }
        public Campaign()
        {
            dynamicBidding = new DynamicBidding();
            budget = new Budget();
            //tags = new Tags();
        }
    }

    public class DynamicBidding
    {
        //public List<PlacementBidding> placementBidding { get; set; }
        public string strategy { get; set; }
        //public DynamicBidding()
        //{
        //    placementBidding = new List<PlacementBidding>();
        //}
    }

    public class PlacementBidding
    {
        public int percentage { get; set; }
        public string placement { get; set; }
    }

    public class CampaignUpdateRequestAzApi
    {
        public List<Campaign> campaigns { get; set; }
        public CampaignUpdateRequestAzApi()
        {
            campaigns = new List<Campaign>();
        }
    }

    public class Tags
    {
        public string property1 { get; set; }
        public string property2 { get; set; }
    }
}
