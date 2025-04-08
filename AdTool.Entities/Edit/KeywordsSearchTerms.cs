using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class KeywordsSearchTerms
    {
        public int Id { get; set; }

        public string SearchTerm { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int Confidence { get; set; }

        public string FriendlyName { get; set; }

        public DateTime? DateBooksUpdated { get; set; }

        public bool? HasKeywords { get; set; }

        public bool? HasAsins { get; set; }
    }
}
