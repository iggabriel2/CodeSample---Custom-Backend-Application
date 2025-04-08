using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class AsinApiInfo
    {
        public static string AsinApi = "https://api.asindataapi.com/request";
        public static string ApiKey = AppSettings.AsinApiKey();

    }
}
