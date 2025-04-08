using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class KeywordHistoryView
    {
        public Guid ClientId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public string SearchTerm { get; set; }
        public int Action { get; set; }
        public string Reason { get; set; }
        public string ActionDescription { get; set; }
        public DateTime DateProcessed { get; set; }
        public string DateProcessedFormatted { get; set; }
    }
}
