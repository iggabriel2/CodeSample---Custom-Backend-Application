using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class ReportUser
    {
        public Guid ClientId { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime TokenExpirationTime { get; set; }
        public int PaymentPlan { get;set; }
        public APIAuthorizationRequest aPIAuthorizationRequest { get; set; }
        public ReportUser()
        {
            aPIAuthorizationRequest = new APIAuthorizationRequest();
        }
    }
}
