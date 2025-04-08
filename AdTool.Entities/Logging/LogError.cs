using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Logging
{
    public class LogError
    {
        public int id { get; set; }

        public Guid ClientId { get; set; }

        public string ErrorMessage { get; set; }

        public string FailureMethod { get; set; }

        public DateTime? RecordDate { get; set; } = DateTime.Now;

        public string Parameters { get; set; }

    }
}
