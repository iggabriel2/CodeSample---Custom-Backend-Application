using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class AzSpClientProfileCodes
    {
        public int Id { get; set; }

        public string ProfileCode { get; set; }

        public int? CountryId { get; set; }

        public Guid ClientId { get; set; }

    }
}
