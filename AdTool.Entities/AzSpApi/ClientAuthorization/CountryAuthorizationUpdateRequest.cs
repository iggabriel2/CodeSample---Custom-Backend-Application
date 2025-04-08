using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ClientAuthorization
{
    public class CountryAuthorizationUpdateRequest
    {
        public APIAuthorizationRequest Authorization { get; set; }
        public CountryAuthorizationUpdateRequest()
        {
            Authorization = new APIAuthorizationRequest();
        }
    }
}
