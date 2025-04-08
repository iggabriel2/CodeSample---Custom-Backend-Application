using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create
{
    public class NegativeQueryRoot
    {
        public List<NegativeKeywords> negativeKeywords { get; set; }
        public NegativeQueryRoot()
        {
            negativeKeywords = new List<NegativeKeywords>();
        }
    }


    public class NegativeKeywords
    {
        public string campaignId { get; set; }
        public string state { get; set; }
        public string keywordText { get; set; }
        public string matchType { get; set; }
        public string adGroupId { get; set; }
    }

    //response
    public class NegativeKeywordsResponse
    {
        public List<ErrorNegativeKeyword> error { get; set; }
        public List<SuccessNegative> success { get; set; }
        public NegativeKeywordsResponse()
        {
            success = new List<SuccessNegative>();
        }
    }

    public class ErrorNegativeKeyword
    {
        public int index { get; set; }
        public List<object> errors { get; set; }
    }

    public class NegativeResponseRoot
    {
        public NegativeKeywordsResponse negativeKeywords { get; set; }
        public NegativeResponseRoot()
        {
            negativeKeywords = new NegativeKeywordsResponse();
        }
    }

    public class SuccessNegative
    {
        public string negativeKeywordId { get; set; }
        public int index { get; set; }
    }
}
