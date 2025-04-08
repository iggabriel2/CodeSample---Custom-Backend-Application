using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Configuration
{
    public class GetSetting
    {
        //not in use. will use if we go back to secrets
        public string GetSettingValue(string settingName)
        {
#if DEBUG
                return settingName;
#elif QA
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
              .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.QA.json"), optional: false)
              .Build();


            string keyvaultUrl = configuration.GetSection("AppSettings").GetSection("keyvaultUrl").Value;

            var client = new SecretClient(vaultUri: new Uri(keyvaultUrl), credential: new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret(settingName);
            string secretValue = secret.Value;
            return secretValue;
#else
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Release.json"), optional: false)
                .Build();


            string keyvaultUrl = configuration.GetSection("AppSettings").GetSection("keyvaultUrl").Value;

            var client = new SecretClient(vaultUri: new Uri(keyvaultUrl), credential: new DefaultAzureCredential());
                KeyVaultSecret secret = client.GetSecret(settingName);
                string secretValue = secret.Value;
                return secretValue;
#endif
        }

   
    }
}
