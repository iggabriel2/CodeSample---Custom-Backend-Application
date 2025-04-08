using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Payments
{
    public class SimplePaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
    }
}
