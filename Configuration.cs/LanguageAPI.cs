using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class LanguageAPI
    {
        public static string LanguageAPIKey = AppSettings.LanguageAPIKey();
    }
}
