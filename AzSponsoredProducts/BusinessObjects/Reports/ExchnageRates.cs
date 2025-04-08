using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class Rates
    {
        public decimal GBP { get; set; }
        public decimal AUD { get; set; }
        public decimal CAD { get; set; }
    }

    public class ExchnageRates
    {
        public bool success { get; set; }
        public int timestamp { get; set; }
        public string @base { get; set; }
        public string date { get; set; }
        public Rates rates { get; set; }
        public ExchnageRates()
        {
            rates = new Rates();
        }
    }


}
