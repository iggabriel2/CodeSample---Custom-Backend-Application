using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignsAdGroups
{
    public class AdGroupSnapshotResponse
    {
        public long adGroupId { get; set; }
        public string name { get; set; }
        public long campaignId { get; set; }
        public decimal defaultBid { get; set; }
        public string state { get; set; }
        public string ClientId { get; set; }
        public int CountryId { get; set; }

        //ClientId-CountryId
        public string partitionKey { get; set; }

        //ClientId-CountryId-CampaignId-adGroupId
        public string id { get; set; }
        public KeywordPerformanceByMonthAdGroup PerformanceData { get; set; }
        public AdGroupSnapshotResponse()
        {
            PerformanceData = new KeywordPerformanceByMonthAdGroup();
        }
    }

    public class KeywordPerformanceByMonthAdGroup
    {
        public decimal Cost { get; set; }
        public decimal CPC { get; set; }
        public decimal ConversionRate { get; set; }
        public int Clicks { get; set; }
        public int Impressions { get; set; }
        public int KindleEditionNormalizedPagesRead14d { get; set; }
        public int purchases14d { get; set; }
        public int Country { get; set; }
        public string AdGroupId { get; set; }
        public decimal CTR { get; set; } = 0;
        public decimal ACOS { get; set; } = 0;
        public decimal Sales { get; set; } = 0;


    }
}
