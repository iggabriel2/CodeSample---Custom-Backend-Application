using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.D4Api
{
    public class ASINMatchRequest
    {
        public List<string> ASINs { get; set; }

        //do not send from frontend. this is for backend use only
        public APIAuthorizationRequest Authorization { get; set; }
        public ASINMatchRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
