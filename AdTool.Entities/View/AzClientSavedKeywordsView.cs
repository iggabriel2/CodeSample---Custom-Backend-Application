using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class AzClientSavedKeywordsView
    {
        public int SavedSearchId { get; set; }

        public int SearchTermId { get; set; }
        public string SavedSearchName { get; set; }

        public string FriendlyName { get; set; }

        public Guid ClientId { get; set; }

        public bool HasAsins { get; set; }
        public bool HasKeywords { get; set; }

    }
}
