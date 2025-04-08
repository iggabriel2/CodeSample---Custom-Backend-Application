using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class Countries
    {
        public int Id { get; set; }

        public string Country { get; set; }

        public string ShortName { get; set; }

        public int? AzApi { get; set; }

    }
}
