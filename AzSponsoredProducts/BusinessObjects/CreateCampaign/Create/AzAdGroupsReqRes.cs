using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create
{
    //request

    public class AdGroupRequestRoot
    {
        public List<APIAdGroupsRequest> adGroups { get; set; }
        public AdGroupRequestRoot()
        {
            adGroups = new List<APIAdGroupsRequest>();
        }

    }
    public class APIAdGroupsRequest
    {
        public string name { get; set; }
        public string campaignId { get; set; }
        public float defaultBid { get; set; }
        public string state { get; set; }
    }


    //response
    public class AdGroup
    {
        public string adGroupId { get; set; }
        public string name { get; set; }
    }

    public class AdGroups
    {
        public List<object> error { get; set; }
        public List<SuccessAdGroup> success { get; set; }

        public AdGroups()
        {
            error = new List<object>();
            success = new List<SuccessAdGroup>();
        }
    }

    public class AdGroupResposeRoot
    {
        public AdGroups adGroups { get; set; }

        public AdGroupResposeRoot()
        {
            adGroups = new AdGroups();
        }
    }

    public class SuccessAdGroup
    {
        public AdGroup adGroup { get; set; }
        public string adGroupId { get; set; }
        public int index { get; set; }

        public SuccessAdGroup()
        {
            adGroup = new AdGroup();
        }
    }


    public class v2AdGroupResponseRoot
    {
        public long adGroupId { get; set; }
        public string code { get; set; }
        public string details { get; set; }
        public string description { get; set; }
    }

}
