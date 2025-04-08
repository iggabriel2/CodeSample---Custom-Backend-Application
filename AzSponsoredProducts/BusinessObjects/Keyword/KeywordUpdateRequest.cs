using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class Keyword
    {
        public string keywordId { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
    }

    public class KeywordUpdateRequest
    {
        public List<Keyword> keywords { get; set; }
        public KeywordUpdateRequest() { 
            keywords = new List<Keyword>();
        }
    }
}
