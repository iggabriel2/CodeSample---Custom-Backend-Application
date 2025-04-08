using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ClientAuthorization
{
    public class CountryAuthorizationUpdateResponse
    {
        public APIAuthorization APIAuthorization { get; set; }
        public List<ClientProfileCodes> ClientProfileCodes { get; set; }

        public CountryAuthorizationUpdateResponse()
        {
            APIAuthorization = new APIAuthorization();
            ClientProfileCodes = new List<ClientProfileCodes>();
        }
    }
}
