using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get
{
    //request
    public class NameFilter
    {
        public string queryTermMatchType { get; set; }
        public List<string> include { get; set; }
        public NameFilter() { 
            include = new List<string>();
        }
    }

    public class GetCampaignRequest
    {
        public NameFilter nameFilter { get; set; }
        public GetCampaignRequest() { 
            nameFilter = new NameFilter();
        }
    }

    public class AllCampaignsRequest
    {
        public string? nextToken { get; set; }
        public int? maxResults { get; set; }
    }

    //response



    public class Budget
    {
        public decimal budget { get; set; }
        public string budgetType { get; set; }
        public int effectiveBudget { get; set; }
    }

    public class Campaign
    {
        public string endDate { get; set; }
        public Budget budget { get; set; }
        public string campaignId { get; set; }
        public DynamicBidding dynamicBidding { get; set; }
        public string name { get; set; }
        public string portfolioId { get; set; }
        public string startDate { get; set; }
        public string state { get; set; }
        public string targetingType { get; set; }
        public Tags tags { get; set; }
        public ExtendedData extendedData { get; set; }
        public Campaign()
        {
            dynamicBidding = new DynamicBidding();
            extendedData = new ExtendedData();
            tags = new Tags();
            budget = new Budget();
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

    public class GetCampaignResponse
    {
        public List<Campaign> campaigns { get; set; }
        public int totalResults { get; set; }
        public string? nextToken { get; set; }
        public GetCampaignResponse()
        {
            campaigns = new List<Campaign>();
        }
    }




    public class ExtendedData
    {
        public DateTime lastUpdateDateTime { get; set; }
        public string servingStatus { get; set; }
        public List<ServingStatusDetail> servingStatusDetails { get; set; }
        public DateTime creationDateTime { get; set; }
        public ExtendedData()
        {
            servingStatusDetails = new List<ServingStatusDetail>();
        }
    }

    public class ServingStatusDetail
    {
        public string name { get; set; }
        public string helpUrl { get; set; }
        public string message { get; set; }
    }

    public class Tags
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

}
