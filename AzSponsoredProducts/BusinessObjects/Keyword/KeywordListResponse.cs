using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class ExtendedDatab
    {
    }

    public class Keywordb
    {
        public string keywordId { get; set; }
        public string nativeLanguageKeyword { get; set; }
        public string nativeLanguageLocale { get; set; }
        public string campaignId { get; set; }
        public string matchType { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
        public string adGroupId { get; set; }
        public string keywordText { get; set; }
        public ExtendedDatab extendedData { get; set; }
        public Keywordb()
        {
            extendedData = new ExtendedDatab();
        }
    }

    public class KeywordListResponse
    {
        public int totalResults { get; set; }
        public List<Keywordb> keywords { get; set; }
        public string nextToken { get; set; }
        public KeywordListResponse() 
        { 
            keywords = new List<Keywordb>();
        }
    }

}
