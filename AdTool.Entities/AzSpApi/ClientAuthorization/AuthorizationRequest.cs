using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ClientAuthorization
{
    public class APIAuthorizationRequest
    {
        //cient id in our system
        public Guid ClientId { get; set; }
        public int AppUserId { get; set; }
        public string? RefreshToken { get; set; }
        public List<ClientProfileCodes>? ClientProfileCodes { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? TokenExpirationTime { get; set; }
        public APIAuthorizationRequest()
        {
            ClientProfileCodes = new List<ClientProfileCodes>();
        }
    }
}
