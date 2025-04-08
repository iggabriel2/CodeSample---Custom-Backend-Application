using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class AzApiInfo
    {

        public static string API1 = "https://advertising-api.amazon.com/";
        public static string API2 = "https://advertising-api-eu.amazon.com/";
        public static string API3 = "https://advertising-api-fe.amazon.com/";
        public static string AuthorizeAPI = "https://api.amazon.com/auth/o2/";


        public static string ClientId = AppSettings.ClientId();
        public static string ClientSecret = AppSettings.ClientSecret();
        public static string AmazonRegistrationURL = AppSettings.AmazonRegistrationURL();
    }
}
