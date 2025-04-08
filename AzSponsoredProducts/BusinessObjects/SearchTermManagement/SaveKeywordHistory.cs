using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class SaveKeywordHistory
    {
        public DateTime? DateProcessed { get; set; } = DateTime.Now.Date;

        public int? CountryId { get; set; }

        public string SearchTerm { get; set; }

        public int? ProductId { get; set; }

        public int? Action { get; set; }

        public string Reason { get; set; }

        public Guid? ClientId { get; set; }

    }
}
