using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.NegativeKeyword.Get
{
    public class AdGroupIdFilter
    {
        public List<string> include { get; set; }
        public AdGroupIdFilter()
        {
            include = new List<string>();
        }
    }

    public class GetNegativeKeywordRequest
    {
        public AdGroupIdFilter adGroupIdFilter { get; set; }
        public GetNegativeKeywordRequest()
        {
            adGroupIdFilter = new AdGroupIdFilter();
        }
    }

    //response

    public class AdGroupNegativeKeyword
    {
        public string campaignId { get; set; }
        public string keywordId { get; set; }
        public string keywordText { get; set; }
        public string matchType { get; set; }
        public string state { get; set; }
        public string adGroupId { get; set; }
    }

    public class GetNegativeKeywordResponse
    {
        public List<AdGroupNegativeKeyword> negativeKeywords { get; set; }
        public int totalResults { get; set; }
        public GetNegativeKeywordResponse()
        {
            negativeKeywords = new List<AdGroupNegativeKeyword>();
        }
    }


}
