using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class KeywordBidTracker
    {
        public string id { get; set; }
        public string partitionKey { get; set; }
        public string ClientId { get; set; }
        public int CountryId { get; set; }
        public string keywordId { get; set; }
        public DateTime LastUpdated { get; set; }

        //Keyword or ProductTarget
        public string KeywordType { get; set; }
    }
}
