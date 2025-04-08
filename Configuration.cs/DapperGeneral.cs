using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Configuration
{
    public static class DapperConnection
    {
        public static string ConnectionString = AppSettings.ConnectionString();

    }

    public static class General
    {
        public static string BackendApi = AppSettings.BackendApi();
    }

}
