using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Keywords
{
    public class KeywordSearchHistory
    {
        public int Id { get; set; }
        public Guid ClientId { get; set; }

        public int SearchTermId { get; set; }

        public DateTime Date { get; set; }

        public bool IsSavedSearch { get; set; }

        public string SavedSearchName { get; set; }

    }
}
