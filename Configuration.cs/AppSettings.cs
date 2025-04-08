using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Configuration
{
    public sealed class AppSettings
    {
        private AppSettings() {
            ConnectionString();
            BackendApi();
            AsinApiKey();
            ClientId();
            ClientSecret();
            AmazonRegistrationURL();
            D4Login();
            LanguageAPIKey();
            PaymentLogin();
            TransKey();
            Crypty();
            EmailPassword();
            EmailFrom();
            SmtpServer();
            EmailPort();
            EmailUsername();
            WebHookKey();
            CosmosKey();
            CosmosAmazonDb();
            CosmosKeywordsContainer();
            CosmosAdGroupsContainer();
            CosmosProductTargetsContainer();
            CosmosKeywordDataContainer();
            CosmosSearchTermsDataContainer();
            CosmosCampaignDataContainer();
            CosmosKeywordBidTrackingContainer();
            CosmosUserDefinedKeywordsContainer();
            ExchangeKey();
            FacebookAPI();
            StripeKey();
            StripePaymentUrl();
            StripePublicKey();
            WebHookSecret();
            CosmosInstance();
        }
        
        private static readonly object padlock = new object();

        static string _connectionString = string.Empty;
        static string _BackendApi = string.Empty;
        static string _keyvaultUrl = string.Empty;
        static string _AsinApiKey = string.Empty;
        static string _ClientId = string.Empty;
        static string _ClientSecret = string.Empty;
        static string _AmazonRegistrationURL = string.Empty;
        static string _D4Login = string.Empty;
        static string _LanguageAPIKey = string.Empty;
        static string _PaymentLogin = string.Empty;
        static string _TransKey = string.Empty;
        static string _Crypty = string.Empty;
        static string _EmailPassword = string.Empty;
        static string _EmailFrom = string.Empty;
        static string _SmtpServer = string.Empty;
        static int _EmailPort = 0;
        static string _EmailUsername = string.Empty;
        static string _WebHookKey = string.Empty;
        static string _CosmosKey = string.Empty;
        static string _CosmosAmazonDb = string.Empty;
        static string _CosmosKeywordsContainer = string.Empty;
        static string _CosmosAdGroupsContainer = string.Empty;
        static string _CosmosProductTargetsContainer = string.Empty;
        static string _CosmosKeywordDataContainer = string.Empty;
        static string _CosmosSearchTermsDataContainer = string.Empty;
        static string _CosmosCampaignDataContainer = string.Empty;
        static string _CosmosKeywordBidTrackingContainer = string.Empty;
        static string _CosmosUserDefinedKeywordsContainer = string.Empty;
        static string _ExchangeKey = string.Empty;
        static string _FacebookAPI = string.Empty;
        static string _StripeKey = string.Empty;
        static string _StripePaymentUrl = string.Empty;
        static string _StripePublicKey = string.Empty;
        static string _WebHookSecret = string.Empty;
        static CosmosClient? _cosmosInstance = null;


#if DEBUG
        static IConfigurationRoot configuration = new ConfigurationBuilder()
             .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
         .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Development.json"), optional: false)
             .Build();
#elif QA
        static IConfigurationRoot configuration = new ConfigurationBuilder()
             .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
         .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.QA.json"), optional: false)
             .Build();
#else
   static IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Release.json"), optional: false)
                .Build();
#endif

        private static readonly Lazy<AppSettings> lazy = new Lazy<AppSettings>(() => new AppSettings());
        public static AppSettings Instance
        {
            get
            {
                return lazy.Value;
            }
        }



        public static string ConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                GetSetting getSetting = new GetSetting();
                _connectionString = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("ConnectionString").Value);
                return _connectionString;
            }
            else
            {
                return _connectionString;
            }
        }

        public static string BackendApi()
        {
            if (string.IsNullOrEmpty(_BackendApi))
            {
                GetSetting getSetting = new GetSetting();
                _BackendApi = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("BackendApi").Value);
                return _BackendApi;
            }
            else
            {
                return _BackendApi;
            }
        }

        public static string AsinApiKey()
        {
            if (string.IsNullOrEmpty(_AsinApiKey))
            {
                GetSetting getSetting = new GetSetting();
                _AsinApiKey = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("AsinApiKey").Value);
                return _AsinApiKey;
            }
            else
            {
                return _AsinApiKey;
            }
        }

        public static string ClientId()
        {
            if (string.IsNullOrEmpty(_ClientId))
            {
                GetSetting getSetting = new GetSetting();
                _ClientId = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("ClientId").Value);
                return _ClientId;
            }
            else
            {
                return _ClientId;
            }
        }

        public static string ClientSecret()
        {
            if (string.IsNullOrEmpty(_ClientSecret))
            {
                GetSetting getSetting = new GetSetting();
                _ClientSecret = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("ClientSecret").Value);
                return _ClientSecret;
            }
            else
            {
                return _ClientSecret;
            }
        }


        public static string AmazonRegistrationURL()
        {
            if (string.IsNullOrEmpty(_AmazonRegistrationURL))
            {
                GetSetting getSetting = new GetSetting();
                _AmazonRegistrationURL = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("AmazonRegistrationURL").Value);
                return _AmazonRegistrationURL;
            }
            else
            {
                return _AmazonRegistrationURL;
            }
        }

        public static string D4Login()
        {
            if (string.IsNullOrEmpty(_D4Login))
            {
                GetSetting getSetting = new GetSetting();
                _D4Login = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("D4Login").Value);
                return _D4Login;
            }
            else
            {
                return _D4Login;
            }
        }

        public static  string LanguageAPIKey()
        {
            if (string.IsNullOrEmpty(_LanguageAPIKey))
            {
                GetSetting getSetting = new GetSetting();
                _LanguageAPIKey = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("LanguageAPIKey").Value);
                return _LanguageAPIKey;
            }
            else
            {
                return _LanguageAPIKey;
            }
        }

        public static string PaymentLogin()
        {
            if (string.IsNullOrEmpty(_PaymentLogin))
            {
                GetSetting getSetting = new GetSetting();
                _PaymentLogin = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("PaymentLogin").Value);
                return _PaymentLogin;
            }
            else
            {
                return _PaymentLogin;
            }
        }

        public static string TransKey()
        {
            if (string.IsNullOrEmpty(_TransKey))
            {
                GetSetting getSetting = new GetSetting();
                _TransKey = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("TransKey").Value);
                return _TransKey;
            }
            else
            {
                return _TransKey;
            }
        }

        public static string Crypty()
        {
            if (string.IsNullOrEmpty(_Crypty))
            {
                GetSetting getSetting = new GetSetting();
                _Crypty = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("Crypty").Value);
                return _Crypty;
            }
            else
            {
                return _Crypty;
            }
        }

        public static string CosmosKey()
        {
            if (string.IsNullOrEmpty(_CosmosKey))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosKey = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("CosmosKey").Value);
                return _CosmosKey;
            }
            else
            {
                return _CosmosKey;
            }
        }

        public static string WebHookKey()
        {
            if (string.IsNullOrEmpty(_WebHookKey))
            {
                GetSetting getSetting = new GetSetting();
                _WebHookKey = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("WebHookKey").Value);

                //the webhookkey is only used on the payment project, so it is okay if it is empty on other projects. Update appsettings to populate it on other projects.
                if ((string.IsNullOrEmpty(_WebHookKey)))
                    _WebHookKey = "";

                return _WebHookKey;
            }
            else
            {
                return _WebHookKey;
            }
        }






        //don't need getsettingvalue since we are only pulling from appsettings


        public static string EmailPassword()
        {
            if (string.IsNullOrEmpty(_EmailPassword))
            {
                GetSetting getSetting = new GetSetting();
                _EmailPassword = configuration.GetSection("AppSettings").GetSection("EmailPassword").Value;
                return _EmailPassword;
            }
            else
            {
                return _EmailPassword;
            }
        }

        public static string EmailFrom()
        {
            if (string.IsNullOrEmpty(_EmailFrom))
            {
                GetSetting getSetting = new GetSetting();
                _EmailFrom = configuration.GetSection("AppSettings").GetSection("EmailFrom").Value;
                return _EmailFrom;
            }
            else
            {
                return _EmailFrom;
            }
        }

        public static string SmtpServer()
        {
            if (string.IsNullOrEmpty(_SmtpServer))
            {
                GetSetting getSetting = new GetSetting();
                _SmtpServer = configuration.GetSection("AppSettings").GetSection("SmtpServer").Value;
                return _SmtpServer;
            }
            else
            {
                return _SmtpServer;
            }
        }

        public static int EmailPort()
        {
            if (_EmailPort == 0)
            {
                GetSetting getSetting = new GetSetting();
                _EmailPort = Convert.ToInt32(configuration.GetSection("AppSettings").GetSection("EmailPort").Value);
                return _EmailPort;
            }
            else
            {
                return _EmailPort;
            }
        }

        public static string EmailUsername()
        {
            if (string.IsNullOrEmpty(_EmailUsername))
            {
                GetSetting getSetting = new GetSetting();
                _EmailUsername = configuration.GetSection("AppSettings").GetSection("EmailUsername").Value;
                return _EmailUsername;
            }
            else
            {
                return _EmailUsername;
            }
        }

        public static string CosmosAmazonDb()
        {
            if (string.IsNullOrEmpty(_CosmosAmazonDb))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosAmazonDb = configuration.GetSection("AppSettings").GetSection("CosmosAmazonDb").Value;
                return _CosmosAmazonDb;
            }
            else
            {
                return _CosmosAmazonDb;
            }
        }

        public static string CosmosKeywordsContainer()
        {
            if (string.IsNullOrEmpty(_CosmosKeywordsContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosKeywordsContainer = configuration.GetSection("AppSettings").GetSection("CosmosKeywordsContainer").Value;
                return _CosmosKeywordsContainer;
            }
            else
            {
                return _CosmosKeywordsContainer;
            }
        }

        public static string CosmosKeywordDataContainer()
        {
            if (string.IsNullOrEmpty(_CosmosKeywordDataContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosKeywordDataContainer = configuration.GetSection("AppSettings").GetSection("CosmosKeywordDataContainer").Value;
                return _CosmosKeywordDataContainer;
            }
            else
            {
                return _CosmosKeywordDataContainer;
            }
        }

        public static string CosmosSearchTermsDataContainer()
        {
            if (string.IsNullOrEmpty(_CosmosSearchTermsDataContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosSearchTermsDataContainer = configuration.GetSection("AppSettings").GetSection("CosmosSearchTermsDataContainer").Value;
                return _CosmosSearchTermsDataContainer;
            }
            else
            {
                return _CosmosSearchTermsDataContainer;
            }
        }

        public static string CosmosAdGroupsContainer()
        {
            if (string.IsNullOrEmpty(_CosmosAdGroupsContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosAdGroupsContainer = configuration.GetSection("AppSettings").GetSection("CosmosAdGroupsContainer").Value;
                return _CosmosAdGroupsContainer;
            }
            else
            {
                return _CosmosAdGroupsContainer;
            }
        }

        public static string CosmosProductTargetsContainer()
        {
            if (string.IsNullOrEmpty(_CosmosProductTargetsContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosProductTargetsContainer = configuration.GetSection("AppSettings").GetSection("CosmosProductTargetsContainer").Value;
                return _CosmosProductTargetsContainer;
            }
            else
            {
                return _CosmosProductTargetsContainer;
            }
        }

        public static string CosmosCampaignDataContainer()
        {
            if (string.IsNullOrEmpty(_CosmosCampaignDataContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosCampaignDataContainer = configuration.GetSection("AppSettings").GetSection("CosmosCampaignDataContainer").Value;
                return _CosmosCampaignDataContainer;
            }
            else
            {
                return _CosmosCampaignDataContainer;
            }
        }

        public static string CosmosKeywordBidTrackingContainer()
        {
            if (string.IsNullOrEmpty(_CosmosKeywordBidTrackingContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosKeywordBidTrackingContainer = configuration.GetSection("AppSettings").GetSection("CosmosKeywordBidTrackingContainer").Value;
                return _CosmosKeywordBidTrackingContainer;
            }
            else
            {
                return _CosmosKeywordBidTrackingContainer;
            }
        }

        public static string CosmosUserDefinedKeywordsContainer()
        {
            if (string.IsNullOrEmpty(_CosmosUserDefinedKeywordsContainer))
            {
                GetSetting getSetting = new GetSetting();
                _CosmosUserDefinedKeywordsContainer = configuration.GetSection("AppSettings").GetSection("CosmosUserDefinedKeywordsContainer").Value;
                return _CosmosUserDefinedKeywordsContainer;
            }
            else
            {
                return _CosmosUserDefinedKeywordsContainer;
            }
        }
        public static string ExchangeKey()
        {
            if (string.IsNullOrEmpty(_ExchangeKey))
            {
                GetSetting getSetting = new GetSetting();
                _ExchangeKey = configuration.GetSection("AppSettings").GetSection("ExchangeKey").Value;
                return _ExchangeKey;
            }
            else
            {
                return _ExchangeKey;
            }
        }

        public static string FacebookAPI()
        {
            if (string.IsNullOrEmpty(_FacebookAPI))
            {
                GetSetting getSetting = new GetSetting();
                _FacebookAPI = configuration.GetSection("AppSettings").GetSection("FacebookAPI").Value;
                return _FacebookAPI;
            }
            else
            {
                return _FacebookAPI;
            }
        }

        public static string StripeKey()
        {
            if (string.IsNullOrEmpty(_StripeKey))
            {
                GetSetting getSetting = new GetSetting();
                _StripeKey = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("StripeKey").Value);
                return _StripeKey;
            }
            else
            {
                return _StripeKey;
            }
        }

        public static string StripePublicKey()
        {
            if (string.IsNullOrEmpty(_StripePublicKey))
            {
                GetSetting getSetting = new GetSetting();
                _StripePublicKey = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("StripePublicKey").Value);
                return _StripePublicKey;
            }
            else
            {
                return _StripePublicKey;
            }
        }

        public static string WebHookSecret()
        {
            if (string.IsNullOrEmpty(_WebHookSecret))
            {
                GetSetting getSetting = new GetSetting();
                _WebHookSecret = getSetting.GetSettingValue(configuration.GetSection("AppSettings").GetSection("WebHookSecret").Value);
                return _WebHookSecret;
            }
            else
            {
                return _WebHookSecret;
            }
        }

        public static string StripePaymentUrl()
        {
            if (string.IsNullOrEmpty(_StripePaymentUrl))
            {
                GetSetting getSetting = new GetSetting();
                _StripePaymentUrl = configuration.GetSection("AppSettings").GetSection("StripePaymentUrl").Value;
                return _StripePaymentUrl;
            }
            else
            {
                return _StripePaymentUrl;
            }
        }

        public static CosmosClient CosmosInstance()
        {
            if (_cosmosInstance == null)
            {
                _cosmosInstance = new CosmosClient(Cosmos.CosmosUri, Cosmos.CosmosKey, new CosmosClientOptions() { AllowBulkExecution = true });
                return _cosmosInstance;
            }
            else
            {
                return _cosmosInstance;
            }
        }
    }
}
