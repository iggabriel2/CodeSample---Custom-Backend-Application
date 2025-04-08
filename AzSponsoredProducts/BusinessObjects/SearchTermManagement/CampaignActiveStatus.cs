using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class CampaignActiveStatus
    {
        public string AzCampaignId { get; set; }
        public bool Active { get; set; }
        public Guid ClientId { get; set; }
    }
}
