using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.AsinError;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create
{
    //request


    public class ProductTargetRequestRoot
    {
        public List<TargetingClause> targetingClauses { get; set; }
    }

    public class TargetingClause
    {
        public List<Expression> expression { get; set; }
        public string campaignId { get; set; }
        public string expressionType { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
        public string adGroupId { get; set; }
    }


    //response


    public class ResolvedExpression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class ProductTargetResponseRoot
    {
        public TargetingClauses targetingClauses { get; set; }
    }

    public class ProductTargetSuccess
    {
        public int index { get; set; }
        public string targetId { get; set; }
        public ResponseTargetingClause targetingClause { get; set; }
    }

    public class ResponseTargetingClause
    {
        public string adGroupId { get; set; }
        public decimal bid { get; set; }
        public string campaignId { get; set; }
        public List<Expression> expression { get; set; }
        public string expressionType { get; set; }
        public List<ResolvedExpression> resolvedExpression { get; set; }
        public string state { get; set; }
        public string targetId { get; set; }
    }

    public class TargetingClauses
    {
        public List<AsinErrorRoot> error { get; set; }
        public List<ProductTargetSuccess> success { get; set; }
        public TargetingClauses()
        {
            error = new List<AsinErrorRoot>(); 
        }
    }

    //shared
    public class Expression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

}
