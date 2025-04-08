using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class AzSpSnapshot
    {
        public decimal SpendOverage { get; set; }
        public int ActiveCampaigns { get; set; }

        public decimal CTR { get; set; }
        public decimal ConversionRate { get; set; }
        public int ActionsRequired { get; set; }
        public int KeywordsPromoted { get; set; }

    }
}
