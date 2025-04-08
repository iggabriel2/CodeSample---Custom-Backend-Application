using AdTool.AzSponsoredProducts.AmazonAPI.CampaignExtra;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword.Google;
using AdTool.AzSponsoredProducts.D4API;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.D4Api;
using AdTool.Entities.Logging;
using Configuration;
using DetectLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.ASINAPI
{
    public class GetProductAsins
    {
        RetrieveData rd = new RetrieveData();

        public async Task<bool> GetAsinKeywords(KeywordRequest AsinKeywordRequest)
        {
            KeywordResponse myResponse = new KeywordResponse();
            AsinApiUtils asinApiUtils = new AsinApiUtils();
            List<BookHolder> books1HolderRaw = new List<BookHolder>();

            try {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                string searchTerm = regex.Replace(AsinKeywordRequest.SearchTerm, " ");
                searchTerm = searchTerm.Replace(" ", "+").Replace(",", "").Replace(".", "").Replace("/", "").Replace(":", "");

                string endpointRaw = "?api_key=APIKEY&type=search&amazon_domain=amazon.com&search_term=SEARCHTERM&exclude_sponsored=true&include_html=false&max_page=1";

                //first call - ebook
                string endpoint1 = endpointRaw.Replace("APIKEY", AsinApiInfo.ApiKey).Replace("SEARCHTERM", searchTerm);
                HttpResponseMessage httpResponseMessage1 = new HttpResponseMessage();
                httpResponseMessage1 = await asinApiUtils.CallAsinSearchApi(endpoint1);

                if (httpResponseMessage1.IsSuccessStatusCode)
                {
                    AsinApiResponse apiResponse1 = new AsinApiResponse();

                    try
                    {
                        apiResponse1 = await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage1.Content.ReadAsStream());
                    }
                    catch (Newtonsoft.Json.JsonReaderException ex)
                    {
                        try
                        {
                            HttpResponseMessage httpResponseMessage2 = await asinApiUtils.CallAsinSearchApi(endpoint1);
                            apiResponse1 = await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage2.Content.ReadAsStream());
                        }
                        catch (Exception e)
                        {
                            await ErrorLogging.LogError(e.ToString(), "GetAsinKeywords - handle success on first call", JsonSerializer.Serialize<AsinApiResponse>(apiResponse1), null);
                            myResponse.APIAuthorization.ErrorMessage += "Unable to locate any ASIN information";
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("Newtonsoft.Json.JsonReaderException"))
                        {
                            try
                            {
                                HttpResponseMessage httpResponseMessage3 = await asinApiUtils.CallAsinSearchApi(endpoint1);
                                apiResponse1 = await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage3.Content.ReadAsStream());
                            }
                            catch (Exception e)
                            {
                                await ErrorLogging.LogError(e.ToString(), "GetAsinKeywords - handle success on first call", JsonSerializer.Serialize<AsinApiResponse>(apiResponse1), null);
                                myResponse.APIAuthorization.ErrorMessage += "Unable to locate any ASIN information";
                            }
                        }
                        else
                        {
                            await ErrorLogging.LogError(ex.ToString(), "GetAsinKeywords - handle success on first call", JsonSerializer.Serialize<AsinApiResponse>(apiResponse1), null);
                            myResponse.APIAuthorization.ErrorMessage += "Unable to locate any ASIN information";
                        }
                    }

                    RegexOptions options2 = RegexOptions.None;
                    Regex regex2 = new Regex("[ ]{2,}", options2);
                    string SearchTerm = regex2.Replace(AsinKeywordRequest.SearchTerm, " ");

                    List<KeywordUpdate> updateKeywords = new List<KeywordUpdate>();

                    if (apiResponse1 != null && apiResponse1.search_results.Count > 0)
                    {
                        foreach (var item in apiResponse1.search_results)
                        {
                            D4Keyword keyword2 = new D4Keyword();
                            keyword2.Keyword = item.asin;
                            keyword2.TypeId = 2;
                            myResponse.Keywords.Add(keyword2);

                            KeywordUpdate updateKeyword2 = new KeywordUpdate();
                            updateKeyword2.CompressedSearchTerm = AsinKeywordRequest.CompressedSearchTerm;
                            updateKeyword2.SearchTerm = SearchTerm;
                            updateKeyword2.SourceId = 1;
                            updateKeyword2.Keyword = keyword2.Keyword;
                            updateKeyword2.TypeId = 2;
                            updateKeywords.Add(updateKeyword2);
                        }
                    }

                    SearchTermRefresh searchTermCombined = new SearchTermRefresh();
                    searchTermCombined.SearchTerm = AsinKeywordRequest.CompressedSearchTerm;
                    searchTermCombined.FriendlyName = SearchTerm;
                    SaveData saveData = new SaveData();
                    var confirmSaved = await saveData.SaveKeywords(updateKeywords, AsinKeywordRequest.Authorization.ClientId, searchTermCombined, true);
                }
            }
            catch (Exception ex)
            {
                myResponse.APIAuthorization.ErrorMessage += "Unable to locate any ASIN information";
            }

            //return myResponse;

            return true;
        }
    }
}
