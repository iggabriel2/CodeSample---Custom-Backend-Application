using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.AdGroups
{
    public class AdGroup
    {
        public string name { get; set; }
        public string state { get; set; }
        public string adGroupId { get; set; }
        public decimal defaultBid { get; set; }
    }

    public class UpdateAdGroupRequestAz
    {
        public List<AdGroup> adGroups { get; set; }
        public UpdateAdGroupRequestAz() { 
            adGroups = new List<AdGroup>();
        }
    }
}
