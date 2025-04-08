using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ProductManagement
{
    
    public class PortfolioResponse
    {
        public List<PortfolioSuccess> PortfolioSuccessByCountry { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public PortfolioResponse()
        {
            APIAuthorization = new APIAuthorization();
            PortfolioSuccessByCountry = new List<PortfolioSuccess>();
        }
    }

    public class PortfolioSuccess
    {
        public bool Success { get; set; }
        public int CountryId { get; set; }
        public string PortfolioId { get; set; }
        public int QapId { get; set; }
    }
}
