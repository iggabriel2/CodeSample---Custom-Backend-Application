using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class ProductTargetSnapshot
    {

        //ClientId-CountryId
        public string partitionKey { get; set; }

        //ClientId-CountryId-CampaignId-ProductTargetId
        public string id { get; set; }
        public string ClientId { get; set; }
        public int CountryId { get; set; }
        public bool HasData { get; set; } = false;

        public long targetId { get; set; }
        public long adGroupId { get; set; }
        public long campaignId { get; set; }
        public string expressionType { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
        public List<Expression> expression { get; set; }
        public List<ResolvedExpression> resolvedExpression { get; set; }
        public ProductTargetSnapshot()
        {
            expression = new List<Expression>();
            resolvedExpression = new List<ResolvedExpression>();
        }

    }

    public class Expression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class ResolvedExpression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

}
