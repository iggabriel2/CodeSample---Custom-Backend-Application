using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    //by ad group
    public class KeywordListRequest
    {
        public AdGroupIdFilter adGroupIdFilter { get; set; }
        public KeywordListRequest() 
        { 
            adGroupIdFilter = new AdGroupIdFilter();
        }
    }

    public class AdGroupIdFilter
    {
        public List<string> include { get; set; }
        public AdGroupIdFilter()
        {
            include = new List<string>();
        }
    }

    //by keyword ids

    public class KeywordListRequestByKeywordIds
    {
        public keywordIdFilter keywordIdFilter { get; set; }
        public KeywordListRequestByKeywordIds()
        {
            keywordIdFilter = new keywordIdFilter();
        }
    }

    public class keywordIdFilter
    {
        public List<string> include { get; set; }
        public keywordIdFilter()
        {
            include = new List<string>();
        }
    }
}
