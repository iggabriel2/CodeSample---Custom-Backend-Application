using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Keywords
{
    public class UserDefinedKeywordsObj
    {
        public List<string> DeletedKeywords { get; set; }
        public List<string> DeletedAsins { get; set; }
        public List<string> DeletedPrint { get; set; }
        public Guid ClientId { get; set; }
        public int SavedSearchId { get; set; }
        public string partitionKey { get; set; }
        public string id { get; set; }

        public UserDefinedKeywordsObj() 
        {
            DeletedKeywords = new List<string>();
            DeletedAsins = new List<string>();
            DeletedPrint = new List<string>();
        }
    }
}
