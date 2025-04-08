using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class AzSpProductSummary
    {
        public int QAPProductId { get; set; }
        public string  Asin { get; set; }
        public string ProductName { get; set; }
        public string AzImageURL { get; set; }
        public Guid ClientId { get; set; }
        public string CountryList { get; set; }
        public string CampaignCount { get; set; }
        public string Clicks { get; set; }
        public string ClickThroughRate { get; set; }
        public string KindleEditionNormalizedPagesRead14d { get; set; }
        public string purchases14d { get; set; }
        public string AttributedSalesSameSku14d { get; set; }
        public string UnitsSoldClicks14d { get; set; }
        public string ConversionRate { get; set; }
    }
}
