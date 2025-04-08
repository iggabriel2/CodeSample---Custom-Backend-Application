using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class KeywordPerformanceResponse
    {
        public List<KeywordsWithData> keywords { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public KeywordPerformanceResponse()
        {
            APIAuthorization = new APIAuthorization();
            keywords = new List<KeywordsWithData>();
        }

    }

    public class KeywordsWithData
    { 
        public string KeywordText { get; set; }
        public string MatchType { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public List<Expression> expression { get; set; }
        public string expressionType { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ACOS { get; set; }
        public decimal CTR { get; set; }
        public int TotalClicks { get; set; }
        public int TotalImpressions { get; set; }
        public long TotalPageReads { get; set; }
        public int TotalPurchases14d { get; set; }
        public decimal TotalConversionRate { get; set; }
        public decimal TotalCPC { get; set; }

        //enabled, paused, archived, or mixed - "mixed" means at least one paused and one enabled and can be paused first. any paused status can be enabled
        public string OverallState { get; set; }
        public List<RelatedKeywordIds> RelatedKeywordIds { get; set; }
        public decimal TotalSales { get; set; }

        //producttarget or keyword
        public string KeywordType { get; set; }
        public KeywordsWithData()
        {
            expression = new List<Expression>();
            RelatedKeywordIds = new List<RelatedKeywordIds>();
        }

    }

    public class Expression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class RelatedKeywordIds
    {
        public string KeywordId { get; set; }
        public string AdGroupId { get; set; }
        public string AdGroupName { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignState { get; set; }
        public string State { get; set; }
        public decimal Bid { get; set; }
        public decimal Cost { get; set; }
        public int Clicks { get; set; }
        public int Impressions { get; set; }
        public long PageReads { get; set; }
        public int Purchases14d { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal CPC { get; set; }
        public decimal CTR { get; set; }
        public decimal ACOS { get; set; }
        public decimal Sales { get; set; }
    }

}
