using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class ActionRequiredView
    {
        public int Id { get; set; }
        public string AzCampaignId { get; set; }
        public int ActionId { get; set; }
        public string ActionRequiredDescription { get; set; }
        public bool Resolved { get; set; }
        public Guid ClientId { get; set; }
        public int CountryId { get; set; }
        public DateTime DateProcessed { get; set; }
        public string Country { get; set; }
        public string CampaignName { get; set; }
        public int ProductId { get; set; }
    }
}
