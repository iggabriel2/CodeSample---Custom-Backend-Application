using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class AzPortfolio
    {
        public int Id { get; set; }

        public string AZPortfolioId { get; set; }

        public string PortfolioName { get; set; }

        public int? CountryId { get; set; }

        public Guid ClientId { get; set; }

        public bool? Active { get; set; }

    }
}
