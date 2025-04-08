using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class SummedSearchTerms
    {
        public string SearchTerm { get; set; }
        public int ProductId { get; set; }
        public int Clicks { get; set; }
        public int Orders { get; set; }
        public int Pages { get; set; }
        public Guid ClientId { get; set; }
        public int Country { get; set; }
    }
}
