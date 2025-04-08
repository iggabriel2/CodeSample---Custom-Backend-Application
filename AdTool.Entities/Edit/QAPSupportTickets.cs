using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class QAPSupportTickets
    {
        public int Id { get; set; }

        public string SubjectLine { get; set; }

        public string RequestDescription { get; set; }

        public string AdditionalEmailAddresses { get; set; }

        public int UserId { get; set; }
    }
}
