using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class KeywordSnapshot
    {
        public long keywordId { get; set; }
        public long adGroupId { get; set; }
        public long campaignId { get; set; }
        public string keywordText { get; set; }
        public string matchType { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
        public string ClientId { get; set; }
        public int CountryId { get; set; }

        //ClientId-CountryId
        public string partitionKey { get; set; }

        //this determins whether we have data in the db for this keyword
        public bool HasData { get; set; } = false;
        public string id { get; set; }
    }

 

}
