using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class AllSearchTerms
    {
        public string SearchTerm { get; set; }
        public string CampaignId { get; set; }
        public Guid ClientId { get; set; }
        public string CampaignName { get; set; }
        public int Country { get; set; }
        public int ProductId { get; set; }
        public bool GeneratedByUs { get; set; }
        public bool Active { get; set; }
        public string KeywordType { get; set; }
        public string Keyword { get; set; }
        public string AdGroupId { get; set; }
        public int Clicks { get; set; }
        public int Orders { get; set; }
        public int Pages { get; set; }
        public string KeywordId { get; set; }

    }

}
