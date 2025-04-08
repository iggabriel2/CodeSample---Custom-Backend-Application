using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{
    public class AzSpClient
    {
        public Guid Id { get; set; }

        public string RefreshToken { get; set; }

        public string AccessToken { get; set; }

        public DateTime? TokenExpirationTime { get; set; }

        public int AppUserId { get; set; }

    }

}
