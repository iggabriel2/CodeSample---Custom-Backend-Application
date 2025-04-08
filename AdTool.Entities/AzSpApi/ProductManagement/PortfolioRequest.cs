using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ProductManagement
{
    public class PortfolioRequest
    {
        public APIAuthorizationRequest Authorization { get; set; }
        public PortfolioRequest()
        {
            Authorization = new APIAuthorizationRequest();
        }
    }
}
