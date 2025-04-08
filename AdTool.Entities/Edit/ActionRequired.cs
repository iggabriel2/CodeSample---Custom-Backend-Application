using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class ActionRequired
    {
        public int Id { get; set; }

        public string AzCampaignId { get; set; }

        public int? ActionId { get; set; }

        public string Description { get; set; }

        public bool? Resolved { get; set; }

        public Guid? ClientId { get; set; }

        public int CountryId { get; set; }

        public DateTime DateProcessed { get; set; }

        public bool Reconcile { get; set; }

    }
}
