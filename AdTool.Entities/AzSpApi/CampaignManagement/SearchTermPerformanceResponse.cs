using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class SearchTermPerformanceResponse
    {
        public List<SearchTermsWithData> SearchTerms { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public SearchTermPerformanceResponse()
        {
            APIAuthorization = new APIAuthorization();
            SearchTerms = new List<SearchTermsWithData>();
        }
    }

    public class SearchTermsWithData
    {
        public string SearchTerm { get; set; }
        public decimal Cost { get; set; }
        public decimal CPC { get; set; }
        public decimal ConversionRate { get; set; }
        public string CountryName { get; set; }
        public int Clicks { get; set; }
        public int Impressions { get; set; }
        public int CountryId { get; set; }
        public long PageReads { get; set; }
        public int Purchases14d { get; set; }

        //producttarget or keyword
        public string SimpleKeywordType { get; set; }
        public string Status { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal CTR { get; set; }
        public decimal ACOS { get; set; }
        public decimal Sales { get; set; }
        public string Reviewed { get; set; }

        public List<RelatedCampaigns> RelatedCampaigns { get; set; }

        public SearchTermsWithData()
        {
            RelatedCampaigns = new List<RelatedCampaigns>();
        }

    }

    public class RelatedCampaigns
    {
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignState { get; set; }
        public string Status { get; set; }
        public decimal Cost { get; set; }
        public int Clicks { get; set; }
        public int Impressions { get; set; }
        public long PageReads { get; set; }
        public int Purchases14d { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal CPC { get; set; }
        public string Keyword { get; set; }
        public string KeywordType { get; set; }
        public string AdGroup { get; set; }
        public string KeywordId { get; set; }
        public string AdGroupName { get; set; }
        public decimal CTR { get; set; }
        public decimal ACOS { get; set; }
        public decimal Sales { get; set; }
        public string Reviewed { get; set; }
        public int UsageTypeId { get; set; }
    }
}
