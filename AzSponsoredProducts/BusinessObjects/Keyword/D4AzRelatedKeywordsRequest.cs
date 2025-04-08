using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class D4AzRelatedKeywordsRequest
    {
        public string keyword { get; set; }
        public string language_name { get; set; }
        public int location_code { get; set; }
        public int limit { get; set; }
        public int depth { get; set; }
    }
}
