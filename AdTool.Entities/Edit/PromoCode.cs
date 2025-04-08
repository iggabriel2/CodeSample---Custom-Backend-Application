using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class PromoCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsPercentage { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public string ApiId { get; set; }
        public string TestApiId { get; set; }
    }
}
