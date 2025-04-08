using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.ProductManagement
{
    public class GetProductResponseAPI
    {
        public APIAuthorization APIAuthorization { get; set; }
        public List<AzSpProductSummaryData> ProductSummaryData { get; set; }
        public GetProductResponseAPI()
        {
            APIAuthorization = new APIAuthorization();
            ProductSummaryData = new List<AzSpProductSummaryData>();
        }
    }
}
