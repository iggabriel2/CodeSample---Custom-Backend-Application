using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.EmailSending
{
    public class EmailTemplates
    {
        public int Id { get; set; }
        public string TemplateBodyFileName { get; set; }

        public string TemplateSubject { get; set; }

        public string TemplateName { get; set; }

    }
}
