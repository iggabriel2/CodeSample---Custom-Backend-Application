using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.CampaignCreations
{
    public class CampaignRequest
    {
        //what type of campaign to make and where to put it
        public APIAuthorizationRequest Authorization { get; set; }
        public int CampaignType { get; set; }
        public int CampaignUsageType { get; set; }

        //set resubmit to 1 if resending fixed keywords/asins. Otherwise set to 0.
        public int Resubmit { get; set; }
        public List<ProductAsinAndCampaignName> ProductAsinsAndCampaignNames { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> KeywordTypes { get; set; }
        public List<string> Asins { get; set; }
        public List<NegativeKeywordsNewCampaign> NegativeKeywordsNewCampaigns { get; set; }
        public List<CountrySpecificRules> CountryRules { get; set; }


        public CampaignRequest() {
            Keywords = new List<string>();
            NegativeKeywordsNewCampaigns = new List<NegativeKeywordsNewCampaign>();
            CountryRules = new List<CountrySpecificRules>();
            Authorization = new APIAuthorizationRequest();
            ProductAsinsAndCampaignNames = new List<ProductAsinAndCampaignName>();
            KeywordTypes = new List<string>();
            Asins = new List<string>();
        }
    }

    public class NegativeKeywordsNewCampaign
    {
        public string NegativeKeyword { get; set; }
        public string BlockType { get; set; }
    }

    public class CountrySpecificRules
    {
        public int CountryId { get; set; }
        public decimal Budget { get; set; }

        //always send as decimal representation (0.51)
        public decimal Bid { get; set; }
        public int TopOfSearch { get; set; }
        public int ProductPages { get; set; }
        public string? SalesText { get; set; }
        public string BiddingStrategy { get; set; }
        public string AzPortfolioId { get; set; }
    }

    public class ProductAsinAndCampaignName
    {
        public int ProductId { get; set; }
        public string Asin { get; set; }
        public string CampaignName { get; set; }
    }
}
