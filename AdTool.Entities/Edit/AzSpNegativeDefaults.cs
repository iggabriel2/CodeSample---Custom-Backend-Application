using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class AzSpNegativeDefaults
    {
        public int Id { get; set; }

        public string NegativeKeyword { get; set; }

        public bool? Phrase { get; set; }

        public bool? Exact { get; set; }

        public int AzSpCountryCampaignConfigId { get; set; }

    }

}
