using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class KeywordSearchHistoryView
    {
        public int Id { get; set; }
        public Guid ClientId { get; set; }
        public int SearchTermId { get; set; }
        public string Date { get; set; }
        public bool IsSavedSearch { get; set; }
        public string SearchTerm { get; set; }
        public string FriendlyName { get; set; }
        public string SavedSearchName { get; set; }
    }
}
