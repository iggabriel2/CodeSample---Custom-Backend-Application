using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword.ProductTargets.Get
{
    public class AdGroupIdFilter
    {
        public List<string> include { get; set; }
        public AdGroupIdFilter()
        {
            include = new List<string>();
        }
    }

    public class GetNegativeProductTargetsRequest
    {
        public AdGroupIdFilter adGroupIdFilter { get; set; }
        public GetNegativeProductTargetsRequest()
        {
            adGroupIdFilter = new AdGroupIdFilter();
        }
    }

    //response
    public class Expression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class NegativeTargetingClause2
    {
        public string adGroupId { get; set; }
        public string campaignId { get; set; }
        public List<Expression> expression { get; set; }
        public List<ResolvedExpression> resolvedExpression { get; set; }
        public string state { get; set; }
        public string targetId { get; set; }
        public NegativeTargetingClause2()
        {
            expression = new List<Expression>();
            resolvedExpression = new List<ResolvedExpression>();
        }
    }

    public class ResolvedExpression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class GetNegativeProductTargetsResponse
    {
        public List<NegativeTargetingClause2> negativeTargetingClauses { get; set; }
        public int totalResults { get; set; }
        public GetNegativeProductTargetsResponse()
        {
            negativeTargetingClauses = new List<NegativeTargetingClause2>();
        }
    }

}