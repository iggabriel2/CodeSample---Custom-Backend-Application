using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSpApi.Keywords
{
    public class GetUserDefinedKeywordsRequest
    {
        public Guid ClientId { get; set; }

        public int SavedSearchId { get; set; }
    }
}
