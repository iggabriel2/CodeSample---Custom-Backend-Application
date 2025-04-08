using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.ProductManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.D4Api
{
    public class KeywordResponse
    {
        public string SearchTerm { get; set; }
        public int? SearchTermId { get; set; }
        public List<D4Keyword> Keywords { get; set; }
        public APIAuthorization APIAuthorization { get; set; }
        public KeywordResponse()
        {
            Keywords = new List<D4Keyword>();
            APIAuthorization = new APIAuthorization();
        }
    }

    public class D4Keyword
    {
        public string Keyword { get; set; }
        public int TypeId { get; set; }
    }

}
