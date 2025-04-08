using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword.Google;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.D4Api;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using Azure.Core;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DetectLanguage;
using Configuration;
using System.Text.RegularExpressions;
using AdTool.BusinessLogic.Utilities;
using System.Net.Http;

namespace AdTool.AzSponsoredProducts.D4API
{
    public class GetKeywords
    {
        public async Task<KeywordResponse> GetD4AzKeywords(KeywordRequest d4KeywordRequest)
        {
            //make appropriate updates for Google



            //this holds our response
            KeywordResponse myResponse = new KeywordResponse();
            myResponse.APIAuthorization.ClientId = d4KeywordRequest.Authorization.ClientId;
            myResponse.APIAuthorization.AccessToken = d4KeywordRequest.Authorization.AccessToken;
            myResponse.APIAuthorization.TokenExpirationTime = d4KeywordRequest.Authorization.TokenExpirationTime;
            myResponse.SearchTerm = d4KeywordRequest.SearchTerm;

            string endPoint = "amazon/related_keywords/live";
            string gEndPoint = "google/keyword_suggestions/live";

            //for now, we are only getting the US and applying that everywhere since it is likely applicable to English-speaking countries

            try
            {
                //make object
                string serlializedJson = await MakeObjectToSend(d4KeywordRequest);

                //call Amazon api here
                D4SeoAPIUtils d4SeoAPIUtils = new D4SeoAPIUtils();
                var responseMessageFirst = d4SeoAPIUtils.CallD4PostApi(endPoint, serlializedJson);

                //call Google api here
                //var gResponseMessageFirst = d4SeoAPIUtils.CallD4PostApi(gEndPoint, serlializedJson);
                await System.Threading.Tasks.Task.WhenAll(responseMessageFirst);

                HttpResponseMessage responseMessage = await responseMessageFirst;
                //HttpResponseMessage gResponseMessage = await gResponseMessageFirst;

                //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                if (responseMessage != null && responseMessage.IsSuccessStatusCode)
                {

                    D4AzRelatedKeywordResponse getValues = new D4AzRelatedKeywordResponse();

                    try
                    {

                        getValues = await JsonSerializer.DeserializeAsync<D4AzRelatedKeywordResponse>(responseMessage.Content.ReadAsStream());
                    }
                    catch (Newtonsoft.Json.JsonReaderException ex)
                    {
                        try
                        {
                            var responseMessageFirst2 = d4SeoAPIUtils.CallD4PostApi(endPoint, serlializedJson);

                            await System.Threading.Tasks.Task.WhenAll(responseMessageFirst2);

                            HttpResponseMessage responseMessage2 = await responseMessageFirst2;

                            getValues = await JsonSerializer.DeserializeAsync<D4AzRelatedKeywordResponse>(responseMessage2.Content.ReadAsStream());
                        }
                        catch (Exception e)
                        {
                            await ErrorLogging.LogError(e.ToString(), "GetD4AzKeywords - handle success on first call",serlializedJson, null);
                            myResponse.APIAuthorization.ErrorMessage += "Unable to locate any keyword information";
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("Newtonsoft.Json.JsonReaderException"))
                        {
                            try
                            {
                                var responseMessageFirst3 = d4SeoAPIUtils.CallD4PostApi(endPoint, serlializedJson);

                                await System.Threading.Tasks.Task.WhenAll(responseMessageFirst3);

                                HttpResponseMessage responseMessage3 = await responseMessageFirst3;

                                getValues = await JsonSerializer.DeserializeAsync<D4AzRelatedKeywordResponse>(responseMessage3.Content.ReadAsStream());
                            }
                            catch (Exception e)
                            {
                                await ErrorLogging.LogError(e.ToString(), "GetD4AzKeywords - handle success on first call", serlializedJson, null);
                                myResponse.APIAuthorization.ErrorMessage += "Unable to locate any keyword information";
                            }
                        }
                        else
                        {
                            await ErrorLogging.LogError(ex.ToString(), "GetD4AzKeywords - handle success on first call", serlializedJson, null);
                            myResponse.APIAuthorization.ErrorMessage += "Unable to locate any keyword information";
                        }
                    }

                    if (getValues != null && getValues.tasks[0].result[0].total_count > 0)
                    {
                        await HandleResponse(getValues, myResponse, d4KeywordRequest.SearchTerm, d4KeywordRequest.CompressedSearchTerm, d4KeywordRequest.Authorization.ClientId, d4KeywordRequest.AccountType);
                    }
                    else
                    {
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex("[ ]{2,}", options);
                        string SearchTerm = regex.Replace(d4KeywordRequest.SearchTerm, " ");

                        List<KeywordUpdate> keywordUpdates = new List<KeywordUpdate>();

                        SearchTermRefresh searchTermCombined = new SearchTermRefresh();
                        searchTermCombined.SearchTerm = d4KeywordRequest.CompressedSearchTerm;
                        searchTermCombined.FriendlyName = SearchTerm;
                        SaveData saveData = new SaveData();
                        var confirmSaved = await saveData.SaveKeywords(keywordUpdates, d4KeywordRequest.Authorization.ClientId, searchTermCombined);

                        myResponse.APIAuthorization.ErrorMessage = "No Keywords Returned";
                    }
                }
                else
                {
                    //call api here
                    var responseMessageFirst2 = d4SeoAPIUtils.CallD4PostApi(endPoint, serlializedJson);

                    await System.Threading.Tasks.Task.WhenAll(responseMessageFirst2);

                    HttpResponseMessage responseMessage2 = await responseMessageFirst2;

                    //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                    if (responseMessage2 != null  && responseMessage2.IsSuccessStatusCode)
                    {
                        D4AzRelatedKeywordResponse getValues = new D4AzRelatedKeywordResponse();

                        try
                        {

                            getValues = await JsonSerializer.DeserializeAsync<D4AzRelatedKeywordResponse>(responseMessage2.Content.ReadAsStream());
                        }
                        catch (Newtonsoft.Json.JsonReaderException ex)
                        {
                            try
                            {
                                var responseMessageFirst4 = d4SeoAPIUtils.CallD4PostApi(endPoint, serlializedJson);

                                await System.Threading.Tasks.Task.WhenAll(responseMessageFirst4);

                                HttpResponseMessage responseMessage4 = await responseMessageFirst4;

                                getValues = await JsonSerializer.DeserializeAsync<D4AzRelatedKeywordResponse>(responseMessage4.Content.ReadAsStream());
                            }
                            catch (Exception e)
                            {
                                await ErrorLogging.LogError(e.ToString(), "GetD4AzKeywords - handle success on first call", serlializedJson, null);
                                myResponse.APIAuthorization.ErrorMessage += "Unable to locate any keyword information";
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.ToString().Contains("Newtonsoft.Json.JsonReaderException"))
                            {
                                try
                                {
                                    var responseMessageFirst3 = d4SeoAPIUtils.CallD4PostApi(endPoint, serlializedJson);

                                    await System.Threading.Tasks.Task.WhenAll(responseMessageFirst3);

                                    HttpResponseMessage responseMessage3 = await responseMessageFirst3;

                                    getValues = await JsonSerializer.DeserializeAsync<D4AzRelatedKeywordResponse>(responseMessage3.Content.ReadAsStream());
                                }
                                catch (Exception e)
                                {
                                    await ErrorLogging.LogError(e.ToString(), "GetD4AzKeywords - handle success on first call", serlializedJson, null);
                                    myResponse.APIAuthorization.ErrorMessage += "Unable to locate any keyword information";
                                }
                            }
                            else
                            {
                                await ErrorLogging.LogError(ex.ToString(), "GetD4AzKeywords - handle success on first call", serlializedJson, null);
                                myResponse.APIAuthorization.ErrorMessage += "Unable to locate any keyword information";
                            }
                        }

                        if (getValues != null && getValues.tasks[0].result[0].total_count > 0)
                        {
                            await HandleResponse(getValues, myResponse, d4KeywordRequest.SearchTerm, d4KeywordRequest.CompressedSearchTerm, d4KeywordRequest.Authorization.ClientId, d4KeywordRequest.AccountType);
                        }
                        else
                        {
                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex("[ ]{2,}", options);
                            string SearchTerm = regex.Replace(d4KeywordRequest.SearchTerm, " ");

                            List<KeywordUpdate> keywordUpdates = new List<KeywordUpdate>();

                            SearchTermRefresh searchTermCombined = new SearchTermRefresh();
                            searchTermCombined.SearchTerm = d4KeywordRequest.CompressedSearchTerm;
                            searchTermCombined.FriendlyName = SearchTerm;
                            SaveData saveData = new SaveData();
                            var confirmSaved = await saveData.SaveKeywords(keywordUpdates, d4KeywordRequest.Authorization.ClientId, searchTermCombined);

                            myResponse.APIAuthorization.ErrorMessage = "No Keywords Returned";
                        }
                    }
                    else
                    {
                        Logging logging = new Logging();
                        LogError logError = new LogError();
                        logError.ErrorMessage = "Failed on GetD4AzKeywords - no success code on API call.";
                        logError.FailureMethod = "GetD4AzKeywords";
                        logError.ClientId = d4KeywordRequest.Authorization.ClientId;
                        logError.Parameters = JsonSerializer.Serialize(d4KeywordRequest);
                        await logging.WriteToLog(logError);

                        myResponse.APIAuthorization.ErrorMessage = "Failed on GetD4AzKeywords";
                        return myResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetD4AzKeywords";
                logError.ClientId = d4KeywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(d4KeywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed on GetD4AzKeywords";
                return myResponse;
            }

            return myResponse;
        }

        //CUSTOMIZE OBJECT
        public async Task<string> MakeObjectToSend(KeywordRequest d4KeywordRequest)
        {
            //make object to send
            List<D4AzRelatedKeywordsRequest> d4AzRelatedKeywordsRequestList = new List<D4AzRelatedKeywordsRequest>();

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            string searchTerm = regex.Replace(d4KeywordRequest.SearchTerm, " ");

            D4AzRelatedKeywordsRequest d4AzRelatedKeywordsRequest = new D4AzRelatedKeywordsRequest();
            d4AzRelatedKeywordsRequest.keyword = searchTerm.Replace(",", "").Replace(".", "").Replace("/", "").Replace(":", "");
            d4AzRelatedKeywordsRequest.language_name = "English";
            d4AzRelatedKeywordsRequest.location_code = 2840;
            d4AzRelatedKeywordsRequest.limit = 1000;
            d4AzRelatedKeywordsRequest.depth = 4;
            d4AzRelatedKeywordsRequestList.Add(d4AzRelatedKeywordsRequest);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(d4AzRelatedKeywordsRequestList);

            return serlializedJson;
        }

        public async Task<bool> HandleResponse(D4AzRelatedKeywordResponse getValues, KeywordResponse myResponse, string SearchTermRaw, string CompressedSearchTerm, Guid ClientId, int accountType)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            string SearchTerm = regex.Replace(SearchTermRaw, " ");
       
            List<KeywordUpdate> updateKeywords = new List<KeywordUpdate>();
            KeywordsToExclueList keywordsToExclueList = await ReturnExcludedKeywords();

            //check language for amazon keywords
            List<string> aZKeywordsToUseRaw = new List<string>();
            List<string> aZKeywordsToUse = new List<string>();

            //combine all of the keywords into one list
            if (getValues != null && getValues.tasks[0].result[0].total_count > 0)
            {
                foreach (var item in getValues.tasks[0].result[0].items)
                {
                    string cleanTitle = Regex.Replace(item.keyword_data.keyword, @"\s+", " ");

                    if (cleanTitle.Replace(".", "").ToLower().Contains(SearchTerm.Replace(".", "").ToLower()))
                    {
                        if (!keywordsToExclueList.keywordToExclude.Any(s => cleanTitle.ToLower().Contains(s)))
                        {
                            //make sure we don't already have it first
                            var itemExists = aZKeywordsToUseRaw.Where(x => x == cleanTitle);

                            if (!itemExists.Any())
                            {
                                aZKeywordsToUseRaw.Add(cleanTitle);
                            }

                            if (item.related_keywords != null)
                            {
                                foreach (var related in item.related_keywords)
                                {
                                    string cleanRelated = Regex.Replace(related, @"\s+", " ");

                                    if (cleanRelated.Replace(".", "").ToLower().Contains(SearchTerm.Replace(".", "").ToLower()))
                                    {
                                        if (!keywordsToExclueList.keywordToExclude.Any(s => cleanRelated.ToLower().Contains(s)))
                                        {
                                            //make sure we don't already have it first
                                            var itemExists2 = aZKeywordsToUseRaw.Where(x => x == cleanRelated);

                                            if (!itemExists2.Any())
                                            {
                                                aZKeywordsToUseRaw.Add(cleanRelated);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //validate language
                string[] azKeywordArray = aZKeywordsToUseRaw.ToArray();

                DetectLanguageClient client = new DetectLanguageClient(LanguageAPI.LanguageAPIKey);
                DetectResult[][] results = await client.BatchDetectAsync(azKeywordArray);

                //make sure I have results
                if (results.Length != azKeywordArray.Length)
                {
                    aZKeywordsToUse = aZKeywordsToUseRaw;
                }
                else
                {
                    int resultItem = 0;
                    foreach (var result in results)
                    {
                        foreach (var resultItemValues in result)
                        {
                            if (resultItemValues.language == "en" && resultItemValues.reliable == true)
                            {
                                aZKeywordsToUse.Add(aZKeywordsToUseRaw.ElementAt(resultItem));
                                break;
                            }
                        }

                        resultItem++;
                    }
                }
               

            }

            //special add for authors
            if (accountType != 1)
            {
                aZKeywordsToUse.Add(SearchTerm + " books");
                aZKeywordsToUse.Add("books by " + SearchTerm);
            }
       
            aZKeywordsToUse.Add(SearchTerm);

            //amazon
            foreach (var item in aZKeywordsToUse)
            {
                D4Keyword keyword = new D4Keyword();
                keyword.Keyword = item;
                keyword.TypeId = 1;
                myResponse.Keywords.Add(keyword);

                KeywordUpdate updateKeyword = new KeywordUpdate();
                updateKeyword.SearchTerm = SearchTerm;
                updateKeyword.CompressedSearchTerm = CompressedSearchTerm;
                updateKeyword.SourceId = 1;
                updateKeyword.Keyword = keyword.Keyword;
                updateKeyword.TypeId = 1;
                updateKeywords.Add(updateKeyword);
            }


            //final keyword cleanup
            foreach(var item in updateKeywords)
            {
                string keywordToClean = item.Keyword.Replace("#", " ");
                item.Keyword = keywordToClean;
            }

            SearchTermRefresh searchTermCombined = new SearchTermRefresh();
            searchTermCombined.SearchTerm = CompressedSearchTerm;
            searchTermCombined.FriendlyName = SearchTerm;
            SaveData saveData = new SaveData();
            var confirmSaved = await saveData.SaveKeywords(updateKeywords, ClientId, searchTermCombined);

            return true;
        }

        public async Task<KeywordsToExclueList> ReturnExcludedKeywords()
        {
            KeywordsToExclueList keywordsToExclueList = new KeywordsToExclueList();

            RetrieveData rd = new RetrieveData();
            var excludedKeywords = await rd.GetKeywordsToExclude();

            if (excludedKeywords != null)
            {
                foreach (var keyword in excludedKeywords)
                {
                    keywordsToExclueList.keywordToExclude.Add(keyword);
                }
            }

            return keywordsToExclueList;
        }
    }

    public class KeywordsToExclueList
    {
        public List<string> keywordToExclude { get; set; }
        public KeywordsToExclueList()
        {
            keywordToExclude = new List<string>();
        }
    }


}
