using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class QAPQuestions
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public bool IsActive { get; set; }

    }
}
