using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ClientAuthorization
{
    public class APIAuthorization
    {
        //cient id in our system
        public Guid ClientId { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? TokenExpirationTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}
