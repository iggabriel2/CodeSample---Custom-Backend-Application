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
    public class GetBooksAndAsins
    {
        RetrieveData rd = new RetrieveData();

        public async Task<bool> GetAsinKeywords(KeywordRequest AsinKeywordRequest)
        {
            KeywordResponse myResponse = new KeywordResponse();
            AsinApiUtils asinApiUtils = new AsinApiUtils();
            List<TitlesExcluded> titlesExcluded = new List<TitlesExcluded>();

            int languageConfidence = await rd.GetLanguageConfidence(AsinKeywordRequest.CompressedSearchTerm);
            titlesExcluded = await rd.GetKnownTitlesToExclude(AsinKeywordRequest.CompressedSearchTerm);

            try
            {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                string searchTerm = regex.Replace(AsinKeywordRequest.SearchTerm, " ");
                searchTerm = searchTerm.Replace(" ", "+").Replace(",", "").Replace(".", "").Replace("/", "").Replace(":", "");

                string endpointRaw = "?api_key=APIKEY&type=search&amazon_domain=amazon.com&search_term=SEARCHTERM&category_id=CATEOGRYID&exclude_sponsored=true&include_html=false&max_page=5";
                
                //first call - ebook
                string endpoint1 = endpointRaw.Replace("APIKEY", AsinApiInfo.ApiKey).Replace("SEARCHTERM", searchTerm).Replace("CATEOGRYID", "154606011");
                HttpResponseMessage httpResponseMessage1 = new HttpResponseMessage();
                httpResponseMessage1 = await asinApiUtils.CallAsinSearchApi(endpoint1);


                //second call - paperback
                //string endpoint2 = endpointRaw.Replace("APIKEY", AsinApiInfo.ApiKey).Replace("SEARCHTERM", searchTerm).Replace("CATEOGRYID", "283155");
                //HttpResponseMessage httpResponseMessage2 = new HttpResponseMessage();
                //httpResponseMessage2 = await asinApiUtils.CallAsinSearchApi(endpoint2);

                //if (httpResponseMessage1.IsSuccessStatusCode && httpResponseMessage2.IsSuccessStatusCode
                if (httpResponseMessage1.IsSuccessStatusCode)
                {
                    AsinApiResponse apiResponse1 = new AsinApiResponse();
               
                    try
                    {
                        apiResponse1 = await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage1.Content.ReadAsStream());
                    }
                    catch(Newtonsoft.Json.JsonReaderException ex)
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
                    catch(Exception ex)
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

                    AsinApiResponse apiResponse2 = new AsinApiResponse();
                    //= await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage2.Content.ReadAsStream());

                    var resultBool = await HandleResponse(apiResponse1, apiResponse2, myResponse, AsinKeywordRequest.SearchTerm, AsinKeywordRequest.CompressedSearchTerm, AsinKeywordRequest.Authorization.ClientId, languageConfidence, titlesExcluded, AsinKeywordRequest.AccountType);

                }
                else
                {
                    //try to call again

                    //first call - ebook
                    string endpoint1a = endpointRaw.Replace("APIKEY", AsinApiInfo.ApiKey).Replace("SEARCHTERM", searchTerm).Replace("CATEOGRYID", "154606011");
                    HttpResponseMessage httpResponseMessage1a = new HttpResponseMessage();
                    httpResponseMessage1a = await asinApiUtils.CallAsinSearchApi(endpoint1a);


                    //second call - paperback
                    //string endpoint2a = endpointRaw.Replace("APIKEY", AsinApiInfo.ApiKey).Replace("SEARCHTERM", searchTerm).Replace("CATEOGRYID", "283155");
                    //HttpResponseMessage httpResponseMessage2a = new HttpResponseMessage();
                    //httpResponseMessage2a = await asinApiUtils.CallAsinSearchApi(endpoint2a);

                    //if (httpResponseMessage1.IsSuccessStatusCode && httpResponseMessage2.IsSuccessStatusCode)
                    if (httpResponseMessage1a.IsSuccessStatusCode)
                    {
                        AsinApiResponse apiResponse1 = new AsinApiResponse();

                        try
                        {
                            apiResponse1 = await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage1a.Content.ReadAsStream());
                        }
                        catch (Newtonsoft.Json.JsonReaderException ex)
                        {
                            try
                            {
                                HttpResponseMessage httpResponseMessage3 = await asinApiUtils.CallAsinSearchApi(endpoint1a);
                                apiResponse1 = await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage3.Content.ReadAsStream());
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
                                    apiResponse1 = await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage1.Content.ReadAsStream());
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

                        AsinApiResponse apiResponse2 = new AsinApiResponse();
                        //= await JsonSerializer.DeserializeAsync<AsinApiResponse>(httpResponseMessage2.Content.ReadAsStream());

                        await HandleResponse(apiResponse1, apiResponse2, myResponse, AsinKeywordRequest.SearchTerm, AsinKeywordRequest.CompressedSearchTerm, AsinKeywordRequest.Authorization.ClientId, languageConfidence, titlesExcluded, AsinKeywordRequest.AccountType);

                    }
                    else
                    {
                        Logging logging = new Logging();
                        LogError logError = new LogError();
                        logError.ErrorMessage = "Failed on GetBooksAndAsins - no success code on API call.";
                        logError.FailureMethod = "GetAsinKeywords";
                        logError.ClientId = AsinKeywordRequest.Authorization.ClientId;
                        logError.Parameters = JsonSerializer.Serialize(AsinKeywordRequest);
                        await logging.WriteToLog(logError);

                        myResponse.APIAuthorization.ErrorMessage = "Failed on GetAsinKeywords";
                        //return myResponse;
                    }
                }
            }
            catch(Exception ex)
            {
                myResponse.APIAuthorization.ErrorMessage += "Unable to locate any ASIN information";
            }

            //return myResponse;

            return true;

        }

        public async Task<bool> HandleResponse(AsinApiResponse response1, AsinApiResponse response2, KeywordResponse myResponse, string SearchTermRaw, string CompressedSearchTerm, Guid ClientId, int languageConfidence, List<TitlesExcluded> titlesExcluded, int accountType)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            string SearchTerm = regex.Replace(SearchTermRaw, " ");
           
            List<KeywordUpdate> updateKeywords = new List<KeywordUpdate>();

            //split search term into parts
            string[] names = SearchTerm.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            List<string> nameStrings = names.ToList();

            //check language for amazon keywords
            List<string> books1KeywordsToUseRaw = new List<string>();
            List<string> books2KeywordsToUseRaw = new List<string>();

            List<BookHolder> books1HolderRaw = new List<BookHolder>();
            List<BookHolder> books2HolderRaw = new List<BookHolder>();
            List<BookHolder> booksHolder = new List<BookHolder>();

            //combine all of the keywords into one list
            if (response1 != null && response1.search_results.Count > 0)
            {
                foreach (var item in response1.search_results)
                {
                    bool keepProcessing = false;

                    //check for author name
                    foreach (var author in item.authors)
                    {
                        bool nameFound = false;

                        if (!author.name.Replace(".","").Replace(",", "").Replace(" ","").ToLower().Contains(SearchTerm.Replace(".", "").Replace(",", "").Replace(" ", "").ToLower()))
                        {
                            nameFound = false;
                        }
                        else
                        {
                            nameFound = true;
                            keepProcessing = true;
                            break;
                        }

                        //foreach (var nameString in nameStrings)
                        //{
                        //    if (nameString.Length > 2)
                        //    {
                        //        if (!author.name.ToLower().Contains(nameString.ToLower()))
                        //        {
                        //            nameFound = false;
                        //            break;
                        //        }
                        //        else
                        //        {
                        //            nameFound = true;
                        //        }
                        //    }
                        //}

                        ////name found, move on
                        //if (nameFound)
                        //{
                        //    keepProcessing = true;
                        //    break;
                        //}
                    }

                    //make sure it's not in another language with an English title
                    if (item.title.Contains("Spanish") || item.title.Contains("French") || item.title.Contains("Italian") || item.title.Contains("Portuguese") || item.title.Contains("Russian") || item.title.Contains("German"))
                    {
                        keepProcessing = false;
                    }

                    //name was found. keep going.
                    if (keepProcessing)
                    {
                        string bookTitle = "";
                        string bookTitle2 = "";
                        string bookTitle3 = "";

                        //clean up book title
                        if (item.title.Contains("/"))
                            bookTitle = item.title.Substring(0, item.title.IndexOf("/"));
                        else
                            bookTitle = item.title;

                        if (bookTitle.Contains("("))
                            bookTitle2 = bookTitle.Substring(0, bookTitle.IndexOf("("));
                        else
                            bookTitle2 = bookTitle;

                        if (bookTitle2.Contains(":"))
                            bookTitle3 = bookTitle2.Substring(0, bookTitle2.IndexOf(":"));
                        else
                            bookTitle3 = bookTitle2;

                        string finalBookTitle = bookTitle3.Replace("#", " ").Replace(".", "").Replace(",", "").Replace("-", " ").Replace("&", " ").Replace(":", "").Replace("/", "");
                        finalBookTitle = regex.Replace(finalBookTitle, " ");

                        //make sure we don't already have it first and that we want to include it
                        var itemExists = books1HolderRaw.Where(x => x.Title.Trim().ToLower() == finalBookTitle.Trim().ToLower());
                        var negativeTitleExists = titlesExcluded.Where(x => x.Title.Trim().ToLower() == finalBookTitle.Trim().ToLower());

                        if (!itemExists.Any() && !negativeTitleExists.Any())
                        {
                            //add book title
                            books1KeywordsToUseRaw.Add(finalBookTitle.Trim());
                            BookHolder bookHolder = new BookHolder();
                            bookHolder.Title = finalBookTitle.Trim();
                            bookHolder.Asin = item.asin;
                            books1HolderRaw.Add(bookHolder);
                        }
                    }
                }

                //get any titles we already have in db and include in our list
                List<string> titlesInDb = await rd.GetTitlesFromDb(CompressedSearchTerm);

                if (titlesInDb != null && titlesInDb.Count > 0)
                {
                    foreach (var titleInDb in titlesInDb)
                    {
                        var itemExists = books1HolderRaw.Where(x => x.Title.Trim().ToLower() == titleInDb.Trim().ToLower());
                        var negativeTitleExists = titlesExcluded.Where(x => x.Title.Trim().ToLower() == titleInDb.Trim().ToLower());

                        if (!itemExists.Any() && !negativeTitleExists.Any())
                        {
                            //add book title
                            books1KeywordsToUseRaw.Add(titleInDb.Trim());
                            BookHolder bookHolder = new BookHolder();
                            bookHolder.Title = titleInDb.Trim();
                            bookHolder.Asin = "";
                            books1HolderRaw.Add(bookHolder);
                        }
                    }
                }

                //validate language
                string[] book1Array = books1KeywordsToUseRaw.ToArray();

                DetectLanguageClient client = new DetectLanguageClient(LanguageAPI.LanguageAPIKey);
                DetectResult[][] results = await client.BatchDetectAsync(book1Array);

                //make sure I have results
                if (results.Length != book1Array.Length)
                {
                    booksHolder = books1HolderRaw;
                }
                else
                {
                    int resultItem = 0;
                    foreach (var result in results)
                    {
                        foreach (var resultItemValues in result)
                        {
                            if (resultItemValues.language == "en" && resultItemValues.reliable == true && resultItemValues.confidence >= languageConfidence)
                            {
                                string bookTitleRaw = books1KeywordsToUseRaw.ElementAt(resultItem);

                                BookHolder bookHolder = books1HolderRaw.Where(x => x.Title ==  bookTitleRaw).FirstOrDefault();
                                booksHolder.Add(bookHolder);
                                break;
                            }
                        }

                        resultItem++;
                    }
                }

                //get any ains we already have in db and include in our list
                List<string> asinsInDb = await rd.GetAsinsFromDb(CompressedSearchTerm);

                foreach (var asinInDb in asinsInDb)
                {
                    var itemExistsInRaw = books1HolderRaw.Where(x => x.Asin.Trim().ToLower() == asinInDb.Trim().ToLower());

                    if (!itemExistsInRaw.Any())
                    {
                        //add book asin
                        BookHolder bookHolder = new BookHolder();
                        bookHolder.Title = "";
                        bookHolder.Asin = asinInDb;
                        booksHolder.Add(bookHolder);
                    }
                }


            }





            //CLEAN THIS UP IF I ADD IT BACK

            ////combine all of the keywords into one list
            //if (response2 != null && response2.search_results.Count > 0)
            //{
            //    foreach (var item in response2.search_results)
            //    {
            //        bool keepProcessing = false;

            //        foreach (var author in item.authors)
            //        {
            //            bool nameFound = false;

            //            foreach (var nameString in nameStrings)
            //            {
            //                if (nameString.Length > 2)
            //                {
            //                    if (!author.name.Contains(nameString))
            //                    {
            //                        nameFound = false;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        nameFound = true;
            //                    }
            //                }

            //                //name found, move on
            //                if (nameFound)
            //                {
            //                    keepProcessing = true;
            //                    break;
            //                }
            //            }
            //        }

            //        //name was found. keep going.
            //        if (keepProcessing)
            //        {
            //            books2KeywordsToUseRaw.Add(item.title);
            //            BookHolder bookHolder = new BookHolder();
            //            bookHolder.Title = item.title;
            //            bookHolder.Asin = item.asin;
            //            books2HolderRaw.Add(bookHolder);
            //        }
            //    }

            //    //validate language
            //    string[] book2Array = books2KeywordsToUseRaw.ToArray();

            //    DetectLanguageClient client = new DetectLanguageClient(LanguageAPI.LanguageAPIKey);
            //    DetectResult[][] results = await client.BatchDetectAsync(book2Array);

            //    //make sure I have results
            //    if (results.Length != book2Array.Length)
            //    {
            //        booksHolder = books2HolderRaw;
            //    }
            //    else
            //    {
            //        int resultItem = 0;
            //        foreach (var result in results)
            //        {
            //            foreach (var resultItemValues in result)
            //            {
            //                if (resultItemValues.language == "en" && resultItemValues.reliable == true)
            //                {
            //                    string bookTitleRaw = books2KeywordsToUseRaw.ElementAt(resultItem);

            //                    BookHolder bookHolder = books2HolderRaw.Where(x => x.Title == bookTitleRaw).FirstOrDefault();
            //                    booksHolder.Add(bookHolder);
            //                    break;
            //                }
            //            }

            //            resultItem++;
            //        }
            //    }


            //}

            //amazon
            foreach (var item in booksHolder)
            {
                if (!string.IsNullOrEmpty(item.Title))
                {
                    D4Keyword keyword = new D4Keyword();
                    keyword.Keyword = item.Title;
                    keyword.TypeId = 3;
                    myResponse.Keywords.Add(keyword);

                    KeywordUpdate updateKeyword = new KeywordUpdate();
                    updateKeyword.CompressedSearchTerm = CompressedSearchTerm;
                    updateKeyword.SearchTerm = SearchTerm;
                    updateKeyword.SourceId = 1;
                    updateKeyword.Keyword = keyword.Keyword;
                    updateKeyword.TypeId = 3;
                    updateKeywords.Add(updateKeyword);
                }

                if (!string.IsNullOrEmpty(item.Asin))
                {
                    D4Keyword keyword2 = new D4Keyword();
                    keyword2.Keyword = item.Asin;
                    keyword2.TypeId = 2;
                    myResponse.Keywords.Add(keyword2);

                    KeywordUpdate updateKeyword2 = new KeywordUpdate();
                    updateKeyword2.CompressedSearchTerm = CompressedSearchTerm;
                    updateKeyword2.SearchTerm = SearchTerm;
                    updateKeyword2.SourceId = 1;
                    updateKeyword2.Keyword = keyword2.Keyword;
                    updateKeyword2.TypeId = 2;
                    updateKeywords.Add(updateKeyword2);
                }
            }

            List<KeywordUpdate> expandedKeywords = new List<KeywordUpdate>();

            //for authors only
            if (accountType != 1)
            {
                expandedKeywords = await MoreKeywordsFromTitles(SearchTerm, CompressedSearchTerm, updateKeywords);
            }
        

            if (expandedKeywords != null && expandedKeywords.Count > 0)
            {
                updateKeywords = updateKeywords.Union(expandedKeywords).ToList();
            }

            SearchTermRefresh searchTermCombined = new SearchTermRefresh();
            searchTermCombined.SearchTerm = CompressedSearchTerm;
            searchTermCombined.FriendlyName = SearchTerm;
            SaveData saveData = new SaveData();
            var confirmSaved = await saveData.SaveKeywords(updateKeywords, ClientId, searchTermCombined, true);

            return true;
        }

        public async Task<List<KeywordUpdate>> MoreKeywordsFromTitles(string SearchTerm, string CompressedSearchTerm, List<KeywordUpdate> updateKeywordsHere)
        {
            try
            {
                List<KeywordUpdate> expandedKeywords = new List<KeywordUpdate>();

                List<KeywordUpdate> keywordsToExpand = updateKeywordsHere.Where(x => x.TypeId == 3).ToList();

                foreach (var keywordToExpand in keywordsToExpand)
                {
                    try
                    {
                        char[] delimiters = new char[] { ' ' };
                        int totalWordsInName = SearchTerm.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                        bool middleNameOneCharacter = false;

                        if (totalWordsInName == 3)
                        {
                            string middleNameStart = SearchTerm.Substring(SearchTerm.IndexOf(" ")).Trim();
                            string middleNameToCalculate = middleNameStart.Substring(1, 1);
                            if (middleNameToCalculate == " " || middleNameToCalculate == ".")
                            {
                                middleNameOneCharacter = true;
                            }
                        }

                        if (totalWordsInName == 2 || middleNameOneCharacter)
                        {
                            List<string> allCombos = new List<string>();

                            List<string> newCombos = new List<string>();
                            newCombos = await AuthorCombos(keywordToExpand.Keyword, SearchTerm);
                            allCombos = allCombos.Union(newCombos).ToList();

                            //check a and the
                            int totalWordsInKeyword = keywordToExpand.Keyword.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

                            if (totalWordsInKeyword > 1)
                            {
                                string firstWord = keywordToExpand.Keyword.Substring(0, keywordToExpand.Keyword.IndexOf(" ")).Trim();

                                if (firstWord.ToLower() == "a" || firstWord.ToLower() == "the")
                                {
                                    string simpleKeyword = keywordToExpand.Keyword.Substring(keywordToExpand.Keyword.IndexOf(" "), keywordToExpand.Keyword.Length - keywordToExpand.Keyword.IndexOf(" ")).Trim();

                                    List<string> newCombos2 = new List<string>();
                                    newCombos2 = await AuthorCombos(simpleKeyword, SearchTerm);
                                    allCombos = allCombos.Union(newCombos2).ToList();
                                }

                                string startingKeyword = "";

                                if (firstWord.ToLower() == "a" || firstWord.ToLower() == "the")
                                {
                                    startingKeyword = keywordToExpand.Keyword.Substring(keywordToExpand.Keyword.IndexOf(" "), keywordToExpand.Keyword.Length - keywordToExpand.Keyword.IndexOf(" ")).Trim();
                                }
                                else
                                {
                                    startingKeyword = keywordToExpand.Keyword;
                                }

                                string prepFreeKeyword = startingKeyword.Replace(" A ", " ").Replace(" The ", " ").Replace(" In ", " ").Replace(" At ", " ").Replace(" By ", " ").Replace(" Of ", " ").Replace(" To ", " ")
                                    .Replace(" a ", " ").Replace(" the ", " ").Replace(" in ", " ").Replace(" at ", " ").Replace(" by ", " ").Replace(" of ", " ").Replace(" to ", " ");

                                if (prepFreeKeyword.Length < startingKeyword.Length)
                                {
                                    List<string> newCombos3 = new List<string>();
                                    newCombos3 = await AuthorCombos(prepFreeKeyword, SearchTerm);
                                    allCombos = allCombos.Union(newCombos3).ToList();
                                }
                            }

                            foreach (var newKeyword in allCombos)
                            {
                                //after I get all the keywords I want, add them here
                                KeywordUpdate updateKeyword = new KeywordUpdate();
                                updateKeyword.CompressedSearchTerm = CompressedSearchTerm;
                                updateKeyword.SearchTerm = SearchTerm;
                                updateKeyword.SourceId = 1;
                                updateKeyword.Keyword = newKeyword.ToLower();
                                updateKeyword.TypeId = 1;
                                expandedKeywords.Add(updateKeyword);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        await ErrorLogging.LogError(ex.ToString(), "MoreKeywordsFromTitles", "Faile for search term " +  SearchTerm + " and keyword " + keywordToExpand.Keyword);
                    }
                }

                return expandedKeywords;
            }
            catch(Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "MoreKeywordsFromTitles", SearchTerm);
                return null;
            }
        }

        public async Task<List<string>> AuthorCombos(string keyword, string SearchTerm)
        {
            try
            {
                List<string> newCombos = new List<string>();

                string partialName = SearchTerm.Trim().Substring(SearchTerm.LastIndexOf(" "));
                string lastName = partialName.Trim();

                //title and author in both orders
                string combo1 = SearchTerm.Trim() + " " + keyword.Trim();
                newCombos.Add(combo1);

                string combo2 = keyword.Trim() + " " + SearchTerm.Trim();
                newCombos.Add(combo2);

                string combo3 = lastName.Trim() + " " + keyword.Trim();
                newCombos.Add(combo3);

                string combo4 = keyword.Trim() + " " + lastName.Trim();
                newCombos.Add(combo4);

                return newCombos;
            }
            catch(Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "AuthorCombos", SearchTerm + " " + keyword);
                return null;
            }
        }
    }
}
