using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create
{

    //response
    public class Campaigns
    {
        public List<object> error { get; set; }
        public List<Success> success { get; set; }
        public Campaigns()
        {
            error = new List<object>();
            success = new List<Success>();
        }
    }

    public class CampaignResponseRoot
    {
        public Campaigns campaigns { get; set; }

        public CampaignResponseRoot()
        {
            campaigns = new Campaigns();
        }
    }

    public class Success
    {
        public string campaignId { get; set; }
        public int index { get; set; }
    }


    //send
    public class CampaignBudget
    {
        public string budgetType { get; set; }
        public decimal budget { get; set; }
    }

    public class Campaign
    {
        //public string endDate { get; set; }
        public string portfolioId { get; set; }
        public string name { get; set; }
        public string targetingType { get; set; }
        public string state { get; set; }
        public DynamicBidding dynamicBidding { get; set; }
        //public string startDate { get; set; }
        public CampaignBudget budget { get; set; }

        public Campaign()
        {
            budget = new CampaignBudget();
            dynamicBidding = new DynamicBidding();
        }
    }



    public class DynamicBidding
    {
        public List<PlacementBidding> placementBidding { get; set; }
        public string strategy { get; set; }

        public DynamicBidding()
        {
            placementBidding = new List<PlacementBidding>();
        }
    }

    public class PlacementBidding
    {
        public int percentage { get; set; }
        public string placement { get; set; }
    }

    public class CampaignQueryRoot
    {
        public List<Campaign> campaigns { get; set; }
        public CampaignQueryRoot()
        {
            campaigns = new List<Campaign>();
        }
    }
}
