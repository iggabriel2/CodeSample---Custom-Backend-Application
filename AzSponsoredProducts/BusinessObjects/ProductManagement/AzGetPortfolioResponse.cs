using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement
{
    
    public class Budget
    {
        public decimal amount { get; set; }
        public string currencyCode { get; set; }
        public string policy { get; set; }
    }

    public class AzGetPortfolioResponse
    {
        public long portfolioId { get; set; }
        public string name { get; set; }
        public Budget budget { get; set; }
        public bool inBudget { get; set; }
        public string state { get; set; }
        public AzGetPortfolioResponse() {
            budget = new Budget();
        }
    }

}
