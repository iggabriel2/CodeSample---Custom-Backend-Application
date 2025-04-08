using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get
{
    //request

    public class CampaignIdFilter
    {
        public List<string> include { get; set; }
        public CampaignIdFilter()
        {
            include = new List<string>();
        }
    }

    public class GetAdGroups
    {
        public CampaignIdFilter campaignIdFilter { get; set; }
        public StateFilter stateFilter { get; set; }
        public GetAdGroups()
        {
            stateFilter = new StateFilter();
            campaignIdFilter = new CampaignIdFilter();
        }
    }

    public class StateFilter
    {
        public List<string> include { get; set; }
        public StateFilter()
        {
            include = new List<string>();
        }
    }


    //response

    public class AdGroup
    {
        public string campaignId { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string adGroupId { get; set; }
        public decimal defaultBid { get; set; }
        public ExtendedData2 extendedData { get; set; }
        public AdGroup()
        {
            extendedData = new ExtendedData2();
        }
    }

    public class ExtendedData2
    {
        public DateTime lastUpdateDateTime { get; set; }
        public string servingStatus { get; set; }
        public List<ServingStatusDetail2> servingStatusDetails { get; set; }
        public DateTime creationDateTime { get; set; }
        public ExtendedData2()
        {
            servingStatusDetails = new List<ServingStatusDetail2>();
        }
    }

    public class GetAdGroupResponse
    {
        public int totalResults { get; set; }
        public List<AdGroup> adGroups { get; set; }
        public string nextToken { get; set; }
        public GetAdGroupResponse()
        {
            adGroups = new List<AdGroup>();
        }
    }

    public class ServingStatusDetail2
    {
        public string name { get; set; }
        public string helpUrl { get; set; }
        public string message { get; set; }
    }




}
