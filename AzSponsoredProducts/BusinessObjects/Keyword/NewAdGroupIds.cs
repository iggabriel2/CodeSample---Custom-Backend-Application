using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class NewAdGroupIds
    {
        public string OldAdGroupId { get; set; }
        public string NewAdGroupId { get; set; }
        public string CampaignId { get; set; }
        public string MatchType { get; set; }
        public decimal Bid { get; set; }
        public int ProductId { get; set; }

        //used only when identifying invalid keywords
        public string KeywordText { get; set; }
    }
}
