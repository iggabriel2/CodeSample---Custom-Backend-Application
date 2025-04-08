using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class ProductTargetExpression
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class ProdutTargetUpdateRequest
    {
        public List<TargetingClauseUpdate> targetingClauses { get; set; }
        public ProdutTargetUpdateRequest()
        {
            targetingClauses = new List<TargetingClauseUpdate>();
        }
    }

    public class TargetingClauseUpdate
    {
        public List<ProductTargetExpression> expression { get; set; }
        public string targetId { get; set; }
        public string expressionType { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
        public TargetingClauseUpdate()
        {
            expression = new List<ProductTargetExpression>();
        }
    }


}
