using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class KeywordUpdate
    {
        public int KeywordSearchTermId { get; set; }
        public string CompressedSearchTerm { get; set; }
        public string SearchTerm { get; set; }
        public int TypeId { get; set; }
        public int SourceId { get; set; }
        public string Keyword { get; set; }
        public Guid SearchKey { get; set; }

    }

}
