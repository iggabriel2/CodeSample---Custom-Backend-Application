using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class KeywordResponseByAdGroup
    {
        public string KeywordType = "";

        public List<KeywordsWithDataByAdGroup> keywords { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public KeywordResponseByAdGroup()
        {
            APIAuthorization = new APIAuthorization();
            keywords = new List<KeywordsWithDataByAdGroup>();
        }

        public class KeywordsWithDataByAdGroup
        {
            public string KeywordText { get; set; }
            public string MatchType { get; set; }
            public int CountryId { get; set; }
            public List<ExpressionByAdGroup> expression { get; set; }
            public string expressionType { get; set; }
            public decimal Cost { get; set; }
            public int Clicks { get; set; }
            public int Impressions { get; set; }
            public long PageReads { get; set; }
            public int Purchases14d { get; set; }
            public decimal ConversionRate { get; set; }
            public decimal CPC { get; set; }
            public string KeywordId { get; set; }
            public string AdGroupId { get; set; }
            public string CampaignId { get; set; }
            public decimal Bid { get; set; }
            public decimal Sales { get; set; } = 0;
            public decimal CTR { get; set; } = 0;
            public decimal ACOS { get; set; } = 0;

            //enabled, paused, archived
            public string State { get; set; }

            //producttarget or keyword
            public string KeywordType { get; set; }
            public KeywordsWithDataByAdGroup()
            {
                expression = new List<ExpressionByAdGroup>();
            }

        }

        public class ExpressionByAdGroup
        {
            public string type { get; set; }
            public string value { get; set; }
        }
    }
}
