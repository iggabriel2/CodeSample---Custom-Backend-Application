using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.D4Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using AdTool.AzSponsoredProducts.AmazonAPI.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Data;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class GetMatchinASINs
    {
        public async Task<SimpleResponse> GetmatchingAsins(ASINMatchRequest myRequest)
        {
            SimpleResponse response = new SimpleResponse();

            try
            {
                //prep call
                //List<string> asins = myRequest.ASINs.Split(',').ToList<string>();
                List<string> asins = myRequest.ASINs;
                HttpClient client = new HttpClient();
                List<KeywordUpdate> updateKeywords = new List<KeywordUpdate>();
                SearchTermsForAsins searchTermsForAsins = new SearchTermsForAsins();

                //get the compressed search term and search term
                RetrieveData rd = new RetrieveData();
                searchTermsForAsins = await rd.GetSearchTermsForAsins(myRequest.Authorization.ClientId, asins[0]);

                //read the asins
                foreach (var asin in asins)
                {
                    string page = await client.GetStringAsync("https://www.amazon.com/dp/" + asin.Trim());
                    await System.Threading.Tasks.Task.Delay(1000);
                    string pageClean = page.Replace(" ", "");

                    string paperbackAsin = "";
                    string hardbackAsin = "";

                    //get paperback if available
                    try
                    {
                        string Substring1 = "";
                        try
                        {
                            Substring1 = pageClean.Substring(pageClean.IndexOf("<spanaria-label=\"PaperbackFormat:\">Paperback</span>") - 320, 320);
                        }
                        catch(Exception ex)
                        {
                            //second try in case page doesn't work right
                            await System.Threading.Tasks.Task.Delay(5000);
                            page = await client.GetStringAsync("https://www.amazon.com/dp/" + asin.Trim());
                            pageClean = page.Replace(" ", "");
                            Substring1 = pageClean.Substring(pageClean.IndexOf("<spanaria-label=\"PaperbackFormat:\">Paperback</span>") - 320, 320);

                        }
                        
                        if (!string.IsNullOrEmpty(Substring1))
                        {
                            string Substring2 = Substring1.Substring(Substring1.IndexOf("ahref="));
                            List<string> Substring3 = SplitAtOccurence(Substring2, '/', 4);
                            string Substring4 = Substring3[0].Substring(Substring3[0].LastIndexOf("/") + 1);
                            paperbackAsin = Substring4;
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        //no paperback
                    }

                    //get hardback if available
                    try
                    {
                        string Substring1 = pageClean.Substring(pageClean.IndexOf("<spanaria-label=\"HardcoverFormat:\">Hardcover</span>") - 320, 320);

                        if (!string.IsNullOrEmpty(Substring1))
                        {
                            string Substring2 = Substring1.Substring(Substring1.IndexOf("ahref="));
                            List<string> Substring3 = SplitAtOccurence(Substring2, '/', 4);
                            string Substring4 = Substring3[0].Substring(Substring3[0].LastIndexOf("/") + 1);
                            hardbackAsin = Substring4;
                        }
                    }
                    catch (Exception ex)
                    {
                        //no hardback
                    }

                    if (!string.IsNullOrEmpty(paperbackAsin))
                    {
                        KeywordUpdate updateKeyword2 = new KeywordUpdate();
                        updateKeyword2.CompressedSearchTerm = searchTermsForAsins.Compressed;
                        updateKeyword2.SearchTerm = searchTermsForAsins.Regular;
                        updateKeyword2.SourceId = 1;
                        updateKeyword2.Keyword = paperbackAsin;
                        updateKeyword2.TypeId = 6;
                        updateKeywords.Add(updateKeyword2);
                    }


                    if (!string.IsNullOrEmpty(hardbackAsin))
                    {
                        KeywordUpdate updateKeyword2 = new KeywordUpdate();
                        updateKeyword2.CompressedSearchTerm = searchTermsForAsins.Compressed;
                        updateKeyword2.SearchTerm = searchTermsForAsins.Regular;
                        updateKeyword2.SourceId = 1;
                        updateKeyword2.Keyword = hardbackAsin;
                        updateKeyword2.TypeId = 7;
                        updateKeywords.Add(updateKeyword2);
                    }

                    await System.Threading.Tasks.Task.Delay(1000);
                }

                //save to the db
                SearchTermRefresh searchTermCombined = new SearchTermRefresh();
                searchTermCombined.SearchTerm = searchTermsForAsins.Compressed;
                searchTermCombined.FriendlyName = searchTermsForAsins.Regular;
                SaveData saveData = new SaveData();
                var confirmSaved = await saveData.SaveKeywords(updateKeywords, myRequest.Authorization.ClientId, searchTermCombined, true);

                CountrySuccess cs = new CountrySuccess();
                cs.Success = true;
                cs.CountryId = 1;

                response.CountrySuccess.Add(cs);

                response.APIAuthorization.ClientId = myRequest.Authorization.ClientId;
                response.APIAuthorization.AccessToken = myRequest.Authorization.AccessToken;
                response.APIAuthorization.TokenExpirationTime = myRequest.Authorization.TokenExpirationTime;

            }
            catch (Exception ex)
            {
                CountrySuccess cs = new CountrySuccess();
                cs.Success = false;
                cs.CountryId = 1;

                response.CountrySuccess.Add(cs);

                await ErrorLogging.LogError(ex.ToString(), "GetmatchingAsins", System.Text.Json.JsonSerializer.Serialize(myRequest), myRequest.Authorization.ClientId);
                response.APIAuthorization.ErrorMessage = "Failed to get ASIN data";
            }

            return response;
        }

        public static List<string> SplitAtOccurence(string input, char separator, int occurence)
        {
            var parts = input.Split(separator);
            var partlist = new List<string>();
            var result = new List<string>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (partlist.Count == occurence)
                {
                    result.Add(string.Join(separator.ToString(), partlist));
                    partlist.Clear();
                }
                partlist.Add(parts[i]);
                if (i == parts.Length - 1) result.Add(string.Join(separator.ToString(), partlist)); // if no more parts, add the rest
            }
            return result;
        }

    }
}
