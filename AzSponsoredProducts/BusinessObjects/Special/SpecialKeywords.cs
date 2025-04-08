using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Special
{
    public class SpecialKeywords
    {
        public int Id { get; set; }
        public int KeywordSearchTermId { get; set; }
        public string Keyword { get; set; }
        public int TypeId { get; set; }
        public int SourceId { get; set; }
    }
}
