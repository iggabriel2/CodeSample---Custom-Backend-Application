using AdTool.AzSponsoredProducts.BusinessObjects.General;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Configuration;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.D4Api;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using Google.Ads.GoogleAds.V11.Resources;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.Logging;
using System.Text.Json;
using AdTool.Entities.View;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;

namespace AdTool.AzSponsoredProducts.Data
{
    public class RetrieveData
    {
        public async Task<AllAccessTokens> GetAccessToken(Guid ClientId)
        {
            AllAccessTokens accessTokenFromDB = new AllAccessTokens();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    accessTokenFromDB = (await connection.QueryAsync<AllAccessTokens>("GetAccessToken", new { @ClientId = ClientId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return accessTokenFromDB;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAccessToken";
                logError.ClientId = ClientId;
                logError.Parameters = DapperConnection.ConnectionString + " Query response: " + accessTokenFromDB;
                await logging.WriteToLog(logError);

                //we should log here, but it won't stop us from proccessing
                accessTokenFromDB.AccessToken = "";
                return accessTokenFromDB;
            }
        }

        public async Task<string> GetRefreshToken(Guid ClientId)
        {
            string RefreshToken = "";
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    RefreshToken = (await connection.QueryAsync<string>("GetRefreshToken", new { @ClientId = ClientId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return RefreshToken;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetRefreshToken";
                logError.ClientId = ClientId;
                logError.Parameters = DapperConnection.ConnectionString + " Query response: " + RefreshToken;
                await logging.WriteToLog(logError);

                return "";
            }
        }

        public async Task<List<AzApiCountries>> GetCountries()
        {
            List<AzApiCountries> allCountries = new List<AzApiCountries>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    allCountries = (await connection.QueryAsync<AzApiCountries>("GetCountries", commandType: CommandType.StoredProcedure)).ToList();
                }
                return allCountries;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCountries";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString;
                await logging.WriteToLog(logError);

                allCountries.Add(new AzApiCountries{ Id = 1, Country = "United States"});
                return allCountries;
            }
        }

        public List<AzApiCountries> GetCountriesSync()
        {
            List<AzApiCountries> allCountries = new List<AzApiCountries>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    allCountries = connection.Query<AzApiCountries>("GetCountries", commandType: CommandType.StoredProcedure).ToList();
                }
                return allCountries;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCountries";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString;
                logging.WriteToLog(logError);

                allCountries.Add(new AzApiCountries { Id = 1, Country = "United States" });
                return allCountries;
            }
        }

        public async Task<List<string>> GetKeywordsToExclude()
        {
            List<string> excludedKeywords = new List<string>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    excludedKeywords = (await connection.QueryAsync<string>("GetKeywordsToExclude", commandType: CommandType.StoredProcedure)).ToList();
                }
                return excludedKeywords;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordsToExclude";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<SearchTermRefresh>> GetExpiredSearchTerms()
        {
            List<SearchTermRefresh> expiredSearchTerms = new List<SearchTermRefresh>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    expiredSearchTerms = (await connection.QueryAsync<SearchTermRefresh>("GetExpiredSearchTerms", new { @dateToUpdate = DateTime.Now.Date.AddMonths(-3) }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return expiredSearchTerms;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetExpiredSearchTerms";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<D4Keyword>> GetKeywordsFromDb(string SearchTerm)
        {
            List<D4Keyword> includedKeywords = new List<D4Keyword>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    includedKeywords = (await connection.QueryAsync<D4Keyword>("GetKeywordsFromDb", new { @searchTerm = SearchTerm }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return includedKeywords;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordsFromDb";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " Search term: " + SearchTerm;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<int> GetSearchTermId(string SearchTerm)
        {
            int searchTermId = 0;
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    searchTermId = (await connection.QueryAsync<int>("GetSearchTermIdFromDb", new { @searchTerm = SearchTerm }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return searchTermId;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetSearchTermId";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " Search term: " + SearchTerm;
                await logging.WriteToLog(logError);

                return 0;
            }
        }

        public async Task<LastProcessedSearchTerms> GetKeywordProcessedDate(string SearchTerm)
        {
            LastProcessedSearchTerms processedDate = new LastProcessedSearchTerms();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    processedDate = (await connection.QueryAsync<LastProcessedSearchTerms>("GetKeywordProcessedDate", new { @searchTerm = SearchTerm }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return processedDate;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordProcessedDate";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " Search term: " + SearchTerm;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<int> GetLanguageConfidence(string SearchTerm)
        {
            int confidence = new int();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    confidence = (await connection.QueryAsync<int>("GetLanguageConfidence", new { @searchTerm = SearchTerm }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                if (confidence == 0)
                {
                    confidence = 3;
                }

                return confidence;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetLanguageConfidence";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " Search term: " + SearchTerm;
                await logging.WriteToLog(logError);

                return 3;
            }
        }

        public async Task<List<TitlesExcluded>> GetKnownTitlesToExclude(string SearchTerm)
        {
            List<TitlesExcluded> titlesExcluded = new List<TitlesExcluded>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    titlesExcluded = (await connection.QueryAsync<TitlesExcluded>("GetKnownTitlesToExclude", new { @searchTerm = SearchTerm }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return titlesExcluded;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKnownTitlesToExclude";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " Search term: " + SearchTerm;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<string>> GetTitlesFromDb(string SearchTerm)
        {
            List<string> includeTitles = new List<string>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    includeTitles = (await connection.QueryAsync<string>("GetTitlesFromDb", new { @searchTerm = SearchTerm }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return includeTitles;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetTitlesFromDb";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " Search term: " + SearchTerm;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<string>> GetAsinsFromDb(string SearchTerm)
        {
            List<string> includeAsins = new List<string>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    includeAsins = (await connection.QueryAsync<string>("GetAsinsFromDb", new { @searchTerm = SearchTerm }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return includeAsins;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAsinsFromDb";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " Search term: " + SearchTerm;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<string> GetAdGroupName(string AzSpCampaignId, int CountryId, Guid ClientId, string AzAdGroupId)
        {
            string adGroupName = "";
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    adGroupName = (await connection.QueryAsync<string>("GetAdGroupName", new { @AzAdGroupId = AzAdGroupId, @AzSpCampaignId = AzSpCampaignId, @ClientId = ClientId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return adGroupName;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAdGroupName";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString;
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<string> GetProductAsin(int QAPProductId)
        {
            string asin = "";
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    asin = (await connection.QueryAsync<string>("GetProductAsin", new { @QAPProductId = QAPProductId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return asin;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetProductAsin";
                logError.ClientId = Guid.Empty;
                logError.Parameters = DapperConnection.ConnectionString + " product id: " + QAPProductId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<AllCampaigns>> GetAllCampaigns(Guid? ClientId)
        {
            List<AllCampaigns> campaignList = new List<AllCampaigns>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    campaignList = (await connection.QueryAsync<AllCampaigns>("GetAllCampaigns", new { @ClientId = ClientId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return campaignList;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAllCampaigns";
                logError.ClientId = (Guid)ClientId;
                logError.Parameters = "client id: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<AllCampaigns>> GetAllCampaignsByCountry(Guid? ClientId, int CountryId)
        {
            List<AllCampaigns> campaignList = new List<AllCampaigns>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    campaignList = (await connection.QueryAsync<AllCampaigns>("GetAllCampaignsByCountry", new { @ClientId = ClientId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return campaignList;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAllCampaignsByCountry";
                logError.ClientId = (Guid)ClientId;
                logError.Parameters = "client id: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<AllCampaigns>> GetAllCampaignsByCountryGeneralView(Guid? ClientId, int CountryId)
        {
            List<AllCampaigns> campaignList = new List<AllCampaigns>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    campaignList = (await connection.QueryAsync<AllCampaigns>("GetAllCampaignsByCountryGeneralView", new { @ClientId = ClientId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return campaignList;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAllCampaignsByCountryGeneralView";
                logError.ClientId = (Guid)ClientId;
                logError.Parameters = "client id: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<List<AllProducts>> GetAllProductsByCountry(Guid? ClientId, int? CountryId)
        {
            List<AllProducts> productList = new List<AllProducts>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    productList = (await connection.QueryAsync<AllProducts>("GetAzSpProductSummaryGridList", new { @ClientId = ClientId, @CountryId = CountryId }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return productList;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetAllProductsByCountry";
                logError.ClientId = (Guid)ClientId;
                logError.Parameters = "client id: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }

        public async Task<SearchTermsForAsins> GetSearchTermsForAsins(Guid? ClientId, string asin)
        {
            SearchTermsForAsins searchTermsForAsins = new SearchTermsForAsins();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    searchTermsForAsins = (await connection.QueryAsync<SearchTermsForAsins>("select searchterm Compressed, friendlyname Regular from KeywordsSearchTerms join KeywordsLocated on KeywordsSearchTerms.Id = KeywordsLocated.KeywordSearchTermId where keyword = @asin", new { @asin = asin }, commandType: CommandType.Text)).FirstOrDefault();
                }
                return searchTermsForAsins;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetSearchTermsForAsins";
                logError.ClientId = (Guid)ClientId;
                logError.Parameters = "client id: " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return null;
            }
        }
    }
}