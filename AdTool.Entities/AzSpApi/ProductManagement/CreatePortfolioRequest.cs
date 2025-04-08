using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ProductManagement
{
    public class CreatePortfolioRequest
    {
        public APIAuthorizationRequest Authorization { get; set; }
        public string PortfolioName { get;set; }
        public List<int> CountriesToCreate { get; set; }
        public CreatePortfolioRequest()
        {
            Authorization = new APIAuthorizationRequest();
        }
    }
}
