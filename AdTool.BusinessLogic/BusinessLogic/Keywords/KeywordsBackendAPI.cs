using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.CampaignManagement;
using AdTool.Entities.AzSpApi.Keywords;
using AdTool.Entities.D4Api;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.BusinessLogic.Keywords
{
    public class KeywordsBackendAPI
    {

        public async Task<KeywordResponse> GetRelatedKeywords(KeywordRequest keywordRequest)
        {
            KeywordResponse? myResponse = new KeywordResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/GetRelatedKeywords", keywordRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<KeywordResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetRelatedKeywords - Api Call";
                    logError.ClientId = keywordRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetRelatedKeywords";
                logError.ClientId = keywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords.";

            }
            return myResponse;
        }

        public async Task<KeywordPerformanceResponse> RetrievePerfromanceKeywords(KeywordPerformanceRequest keywordRequest)
        {
            KeywordPerformanceResponse? myResponse = new KeywordPerformanceResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/RetrievePerfromanceKeywords", keywordRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<KeywordPerformanceResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "RetrievePerfromanceKeywords - Api Call";
                    logError.ClientId = keywordRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords information.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "RetrievePerfromanceKeywords";
                logError.ClientId = keywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords information.";

            }
            return myResponse;
        }

        public async Task<SearchTermPerformanceResponse> RetrievePerfromanceSearchTerms(SearchTermPerformanceRequest keywordRequest)
        {
            SearchTermPerformanceResponse? myResponse = new SearchTermPerformanceResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/RetrievePerfromanceSearchTerms", keywordRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SearchTermPerformanceResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "RetrievePerfromanceSearchTerms - Api Call";
                    logError.ClientId = keywordRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords information.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "RetrievePerfromanceSearchTerms";
                logError.ClientId = keywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords information.";

            }
            return myResponse;
        }

        public async Task<SimpleResponse> UpdateKeyword(KeywordChangeRequest keywordRequest)
        {
            SimpleResponse? myResponse = new SimpleResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/UpdateKeyword", keywordRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimpleResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "UpdateKeyword - Api Call";
                    logError.ClientId = keywordRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to update keywords.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateKeyword";
                logError.ClientId = keywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to update keywords.";

            }
            return myResponse;
        }

        public async Task<SimpleResponse> ApplyNegativeKeyword(NegativeOneOffKeyword keywordRequest)
        {
            SimpleResponse? myResponse = new SimpleResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/ApplyNegativeKeyword", keywordRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimpleResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "ApplyNegativeKeyword - Api Call";
                    logError.ClientId = keywordRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to negative keywords.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ApplyNegativeKeyword";
                logError.ClientId = keywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to negative keywords.";

            }
            return myResponse;
        }

        public async Task<SimpleResponse> ApplyReviewed(NegativeOneOffKeyword keywordRequest)
        {
            SimpleResponse? myResponse = new SimpleResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/ApplySearchTermReviewed", keywordRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimpleResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "ApplyReviewed - Api Call";
                    logError.ClientId = keywordRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to apply reviewed.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ApplyReviewed";
                logError.ClientId = keywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to apply reviewed.";

            }
            return myResponse;
        }

        public async Task<KeywordResponseByAdGroup> RetrieveKeywordsByAdGroup(KeywordRequestByAdGroup keywordRequest)
        {
            KeywordResponseByAdGroup? myResponse = new KeywordResponseByAdGroup();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/RetrieveKeywordsByAdGroup", keywordRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<KeywordResponseByAdGroup>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "RetrieveKeywordsByAdGroup - Api Call";
                    logError.ClientId = keywordRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "RetrieveKeywordsByAdGroup";
                logError.ClientId = keywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get keywords.";

            }
            return myResponse;
        }

        public async Task<GetUserDefinedKeywordsResponse> GetUserDefinedKeywords(GetUserDefinedKeywordsRequest keywordRequest)
        {
            GetUserDefinedKeywordsResponse? myResponse = new GetUserDefinedKeywordsResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/GetUserDefinedKeywords", keywordRequest.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<GetUserDefinedKeywordsResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetUserDefinedKeywords - Api Call";
                    logError.ClientId = keywordRequest.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.ErrorMessage = "Failed to get deleted keywords.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetUserDefinedKeywords";
                logError.ClientId = keywordRequest.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.ErrorMessage = "Failed to get deleted keywords.";

            }
            return myResponse;
        }

        public async Task<UpdateUserDefinedKeywordsResponse> UpdateUserDefinedKeywords(UpdateUserDefinedKeywordsRequest keywordRequest)
        {
            UpdateUserDefinedKeywordsResponse? myResponse = new UpdateUserDefinedKeywordsResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Keyword/UpdateUserDefinedKeywords", keywordRequest.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<UpdateUserDefinedKeywordsResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "UpdateUserDefinedKeywords - Api Call";
                    logError.ClientId = keywordRequest.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                    await logging.WriteToLog(logError);

                    myResponse.ErrorMessage = "Failed to update keywords.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateUserDefinedKeywords";
                logError.ClientId = keywordRequest.ClientId;
                logError.Parameters = JsonSerializer.Serialize(keywordRequest);
                await logging.WriteToLog(logError);

                myResponse.ErrorMessage = "Failed to update keywords.";

            }
            return myResponse;
        }
    }
}
