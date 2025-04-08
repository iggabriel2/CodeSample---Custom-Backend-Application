using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class SaveActionRequired
    {
        public string? AzCampaignId { get; set; }
        public int? ActionId { get; set; }
        public string? Description { get; set; }
        public bool Resolved { get; set; } = false;
        public Guid? ClientId { get; set; }
        public int CountryId { get; set; }
    }
}
