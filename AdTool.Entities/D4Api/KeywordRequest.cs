using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.D4Api
{
    public class KeywordRequest
    {
        public string SearchTerm { get; set; }
        public int AccountType { get; set; }

        //do not send from frontend. this is for backend use only
        public string? CompressedSearchTerm { get; set; }
        public APIAuthorizationRequest Authorization { get; set; }
        public KeywordRequest()
        {
            Authorization = new APIAuthorizationRequest();

        }
    }
}
