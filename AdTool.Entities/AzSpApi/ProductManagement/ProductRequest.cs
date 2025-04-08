using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ProductManagement
{
    public class ProductRequest
    {
        public string Asin { get; set; }
        public string AdType { get; set; } //always SP for now
        public APIAuthorizationRequest Authorization { get; set; }
        public ProductRequest() {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
