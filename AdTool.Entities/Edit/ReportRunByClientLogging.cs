using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class ReportRunByClientLogging
    {
        public int Id { get; set; }

        public Guid? AzClientId { get; set; }

        public int? CountryId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? LastRunDate { get; set; }
    }
}
