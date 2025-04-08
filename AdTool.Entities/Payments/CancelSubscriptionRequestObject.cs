using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Payments
{
    public class CancelSubscriptionRequestObject
    {
        public string AppUserId { get; set; }
        public string processDate { get; set; }
    }
}
