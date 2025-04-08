using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Config
{
    public class ConfigInfo
    {
        public string ConnectionString { get; set; }
        public string BackendApi { get; set; }
        public string keyvaultUrl { get; set; }
        public string AsinApiKey { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AmazonRegistrationURL { get; set; }
        public string D4Login { get; set; }
        public string LanguageAPIKey { get; set; }
    }
}
