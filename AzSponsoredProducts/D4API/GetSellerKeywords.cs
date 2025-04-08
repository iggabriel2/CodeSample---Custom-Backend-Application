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
using Microsoft.Azure.Cosmos;

namespace AdTool.AzSponsoredProducts.D4API
{
    public class GetSellerKeywords
    {
        public async Task<bool> GetD4AzKeywords(KeywordRequest d4KeywordRequest)
        {
            D4AzRelatedKeywordResponse getValues = new D4AzRelatedKeywordResponse();

            //this holds our response
            KeywordResponse myResponse = new KeywordResponse();
            myResponse.APIAuthorization.ClientId = d4KeywordRequest.Authorization.ClientId;
            myResponse.APIAuthorization.AccessToken = d4KeywordRequest.Authorization.AccessToken;
            myResponse.APIAuthorization.TokenExpirationTime = d4KeywordRequest.Authorization.TokenExpirationTime;
            myResponse.SearchTerm = d4KeywordRequest.SearchTerm;

            string endPoint = "amazon/related_keywords/live";

            try
            {
                //make object
                string serlializedJson = await MakeObjectToSend(d4KeywordRequest);

                //call Amazon api here
                D4SeoAPIUtils d4SeoAPIUtils = new D4SeoAPIUtils();
                var responseMessage = await d4SeoAPIUtils.CallD4PostApi(endPoint, serlializedJson);
                if (responseMessage != null && responseMessage.IsSuccessStatusCode)
                {
                    getValues = await JsonSerializer.DeserializeAsync<D4AzRelatedKeywordResponse>(responseMessage.Content.ReadAsStream());
                }

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                string SearchTerm = regex.Replace(d4KeywordRequest.SearchTerm, " ");

                List<KeywordUpdate> updateKeywords = new List<KeywordUpdate>();

                //check language for amazon keywords
                List<string> aZKeywordsToUseRaw = new List<string>();
                List<string> aZKeywordsToUse = new List<string>();

                //combine all of the keywords into one list
                if (getValues != null && getValues.tasks[0].result[0].total_count > 0)
                {
                    foreach (var item in getValues.tasks[0].result[0].items)
                    {
                        string cleanTitle = Regex.Replace(item.keyword_data.keyword, @"\s+", " ");

                        //make sure we don't already have it first
                        var itemExists = aZKeywordsToUseRaw.Where(x => x == cleanTitle);

                        if (!itemExists.Any())
                        {
                            aZKeywordsToUseRaw.Add(cleanTitle);
                        }

                        //if (item.related_keywords != null)
                        //{
                        //    foreach (var related in item.related_keywords)
                        //    {
                        //        string cleanRelated = Regex.Replace(related, @"\s+", " ");

                        //        //make sure we don't already have it first
                        //        var itemExists2 = aZKeywordsToUseRaw.Where(x => x == cleanRelated);

                        //        if (!itemExists2.Any())
                        //        {
                        //            aZKeywordsToUseRaw.Add(cleanRelated);
                        //        }
                        //    }
                        //}
                    }
                }

                foreach (var item in aZKeywordsToUseRaw)
                {
                    D4Keyword keyword = new D4Keyword();
                    keyword.Keyword = item;
                    keyword.TypeId = 1;
                    myResponse.Keywords.Add(keyword);

                    KeywordUpdate updateKeyword = new KeywordUpdate();
                    updateKeyword.SearchTerm = SearchTerm;
                    updateKeyword.CompressedSearchTerm = d4KeywordRequest.CompressedSearchTerm;
                    updateKeyword.SourceId = 1;
                    updateKeyword.Keyword = keyword.Keyword;
                    updateKeyword.TypeId = 1;
                    updateKeywords.Add(updateKeyword);
                }

                SearchTermRefresh searchTermCombined = new SearchTermRefresh();
                searchTermCombined.SearchTerm = d4KeywordRequest.CompressedSearchTerm;
                searchTermCombined.FriendlyName = SearchTerm;
                SaveData saveData = new SaveData();
                var confirmSaved = await saveData.SaveKeywords(updateKeywords, d4KeywordRequest.Authorization.ClientId, searchTermCombined);

                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetD4AzKeywords on GetSellerKeywords";
                logError.ClientId = d4KeywordRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(d4KeywordRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed on GetD4AzKeywords";
                return false;
            }
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
    }
}
