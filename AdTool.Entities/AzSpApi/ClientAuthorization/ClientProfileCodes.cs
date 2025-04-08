using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.AzSp.ClientAuthorization
{
    public class ClientProfileCodes
    {
        public string ProfileCode { get; set; }
        public int CountryId { get; set; }
        public string? TimeZone { get; set; }
    }

}
