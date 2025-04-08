using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ClientAuthorization
{
    public class OriginalApiAuthorizationRequest
    {
        public int AppUserId { get; set; }
        public string ClientCode { get; set; }
        public Guid ClientId { get; set; }
    }
}
