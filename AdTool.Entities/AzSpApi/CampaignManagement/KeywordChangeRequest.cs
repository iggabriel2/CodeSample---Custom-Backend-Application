using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.CampaignManagement
{
    public class KeywordChangeRequest
    {
        public string keywordId { get; set; }

        //options: "ENABLED" "PAUSED"
        public string state { get; set; }
        public decimal bid { get; set; }
        public bool BidUpdated { get; set; }
        public int CountryId { get; set; }
        public string? CampaignId { get; set; }
        public string? expressionType { get; set; }
        public List<ProductTargetExpression>? expression { get; set; }

        //options are "KEYWORD" or "TARGET"
        public string KeywordType { get; set; }
        public APIAuthorizationRequest Authorization { get; set; }
        public KeywordChangeRequest()
        {
            Authorization = new APIAuthorizationRequest();
            expression = new List<ProductTargetExpression>();

        }
    }

    public class ProductTargetExpression
    {
        public string? type { get; set; }
        public string? value { get; set; }
    }

}
