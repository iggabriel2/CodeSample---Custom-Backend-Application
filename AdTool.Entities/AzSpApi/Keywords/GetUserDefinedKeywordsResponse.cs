using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.Keywords
{
    public class GetUserDefinedKeywordsResponse
    {
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public List<string> DeletedKeywords { get; set; }

        public List<string> DeletedAsins { get; set; }
        public List<string> DeletedPrint { get; set; }
        public GetUserDefinedKeywordsResponse()
        {
            DeletedKeywords = new List<string>();
            DeletedAsins = new List<string>();
            DeletedPrint = new List<string>();
        }
    }
}
