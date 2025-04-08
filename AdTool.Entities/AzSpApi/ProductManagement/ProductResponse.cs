using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ProductManagement
{
    public class ProductResponse
    {
        public APIAuthorization APIAuthorization { get; set; }
        public string ProductName { get; set; }
        public string Asin { get; set; }
        public string Author { get; set; }
        public string ImageURL { get; set; }
        public List<int> ValidCountries { get; set; }
        public ProductResponse()
        {
            ValidCountries = new List<int>();
            APIAuthorization = new APIAuthorization();
        }
    }
}
