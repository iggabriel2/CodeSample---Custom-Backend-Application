using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class SaveSummaryReportAction
    {
        public string AzCampaignId { get; set; }
        public int CountryId { get; set; }
        public string SearchTerm { get; set; }
        public decimal DefaultBid { get; set; }
        public bool Product { get;set; }




        public bool Negative { get; set; } = false;
        public bool Promoted { get; set; } = false;

        public string AdGroup { get; set; }
        public Guid ClientId { get; set; }
        public string keyword { get; set; }
        public string keywordType { get; set; }
        public int QapProductId { get; set; }
        public string KeywordId { get; set; }


    }
}
