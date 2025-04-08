using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class AzSpProduct
    {
        public int QAPProductId { get; set; }

        public string Asin { get; set; }

        public string ProductName { get; set; }

        public bool Active { get; set; }

        public Guid ClientId { get; set; }

        public string AzProductName { get; set; }

        public string AzImageURL { get; set; }
        public string Author { get; set; }

    }

}
