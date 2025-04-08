using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.General
{
    public class SimpleResponse
    {
        public List<CountrySuccess> CountrySuccess { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public SimpleResponse()
        {
            APIAuthorization = new APIAuthorization();
            CountrySuccess = new List<CountrySuccess>();
        }
    }

    public class CountrySuccess
    {
        public bool Success { get; set; }
        public int CountryId { get; set; }
    }

}
