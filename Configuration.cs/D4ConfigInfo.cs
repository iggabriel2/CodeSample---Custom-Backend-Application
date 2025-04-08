using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class D4ConfigInfo
    {
        public static string D4Api = "https://api.dataforseo.com/v3/dataforseo_labs/";
        public static string D4Login = AppSettings.D4Login();
    }
}
