using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.ProductManagement
{
    public class PortfolioListResponse
    {
        public List<PortfolioList> Portfolios { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public PortfolioListResponse()
        {
            APIAuthorization = new APIAuthorization();
            Portfolios = new List<PortfolioList>();
        }
    }

    public class PortfolioList
    {
        public string PortfolioName { get; set; }
        public string AzPortfolioId { get; set; }
        public int CountryId { get; set; }
        public int QapId { get; set; }
    }
}
