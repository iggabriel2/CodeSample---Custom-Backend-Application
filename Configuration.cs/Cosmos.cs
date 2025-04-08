using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class Cosmos
    {
        public static string CosmosUri = "https://adtoolcosmos.documents.azure.com:443/";
        public static string CosmosKey = AppSettings.CosmosKey();
        public static string CosmosAzDb = AppSettings.CosmosAmazonDb();
        public static string CosmosKeywords = AppSettings.CosmosKeywordsContainer();
        public static string CosmosAdGroups = AppSettings.CosmosAdGroupsContainer();
        public static string CosmosProductTargets = AppSettings.CosmosProductTargetsContainer();
        public static string CosmosKeywordDataContainer = AppSettings.CosmosKeywordDataContainer();
        public static string CosmosSearchTermsDataContainer = AppSettings.CosmosSearchTermsDataContainer();
        public static string CosmosCampaignDataContainer = AppSettings.CosmosCampaignDataContainer();
        public static string CosmosBidTrackingContainer = AppSettings.CosmosKeywordBidTrackingContainer();
        public static string CosmosUserDefinedKeywordsContainer = AppSettings.CosmosUserDefinedKeywordsContainer();
        public static CosmosClient cosmosInstance = AppSettings.CosmosInstance();
    }
}
