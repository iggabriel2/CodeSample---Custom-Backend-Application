using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class ReconcileHistory
    {
        public string AzCampaignId { get; set; }
        public int CountryId { get; set; }
        public bool Reconcile { get; set; }
        public Guid ClientId { get; set; }
    }
}
