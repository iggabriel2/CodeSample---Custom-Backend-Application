using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class KeywordDetailsForCosmos
    {
        public string CampaignId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CountryName { get; set; }
        public int CountryId { get; set; }
        public int QAPCampaignId { get; set; }
        public string UsageType { get; set; }

    }
    
    public class KeywordNegativesForCosmos
    {
        public string KeywordId { get; set; }
        public string SearchTerm { get; set; }
        public int? CountryId { get; set; }
        public bool Negative { get; set; }
        public bool Reviewed { get; set; } = false;
    }
}
