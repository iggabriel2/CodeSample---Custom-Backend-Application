using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement
{
    public class CampaignProductRelationships
    {
        public string AzAdGroupId { get; set; }
        public string azspcampaignid { get; set; }
        public int azadgroupusagetype { get; set; }
        public int ProductId { get; set; }
        public int CountryId { get; set; }
        public Guid ClientId { get; set; }
        public int CampaignUsageType { get; set; }
        public bool PrimaryInUsageType { get; set; }
        public bool PrimaryAdGroup { get; set; }
    }
}
