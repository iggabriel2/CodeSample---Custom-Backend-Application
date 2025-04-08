using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword.Google;
using AdTool.AzSponsoredProducts.BusinessObjects.Special;
using AdTool.AzSponsoredProducts.BusinessObjects.Special.Compete;
using AdTool.AzSponsoredProducts.Data.Special;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.D4Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class CustomKeywords
    {
        public async Task<string> GetKeywords(KeywordRequest myRequest)
        {

            GetKeywordsLogic getKeywordsLogic = new GetKeywordsLogic();
            var result = await getKeywordsLogic.GetKeywords(myRequest, true);

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            string searchTerm = regex.Replace(myRequest.SearchTerm, " ");

            myRequest.CompressedSearchTerm = searchTerm.Trim().ToLower().Replace(".", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace(" ", "");

            //get the books from the table for this author
            SpecialDbCalls specialDbCalls = new SpecialDbCalls();
            List<string> asinsToProcess = await specialDbCalls.GetAllAsins(myRequest.CompressedSearchTerm);

            if (asinsToProcess.Count < 1)
            {
                return "Failed to locate any books with this keyword. Check spelling and try again or make sure db has records.";
            }

            int searchTermId = await specialDbCalls.GetSearchTermId(myRequest.CompressedSearchTerm);

            List<SpecialKeywords> specialKeywords = new List<SpecialKeywords>();

            //get ranked keywords for each book
            foreach(var asin in asinsToProcess)
            {
                string endPoint = "amazon/ranked_keywords/live";

                string objectToSend = await MakeObjectToSend(asin);

                D4SeoAPIUtils d4SeoAPIUtils = new D4SeoAPIUtils();
                HttpResponseMessage responseMessage = await d4SeoAPIUtils.CallD4PostApi(endPoint, objectToSend);

                if (responseMessage.IsSuccessStatusCode)
                {
                    RankedKeywords getValues = await JsonSerializer.DeserializeAsync<RankedKeywords>(responseMessage.Content.ReadAsStream());

                    if (getValues != null && getValues.tasks[0].result[0].total_count > 0)
                    {
                        foreach(var searchTermHolder in getValues.tasks[0].result[0].items)
                        {
                            SpecialKeywords specialCheck = specialKeywords.FirstOrDefault(x => x.Keyword == searchTermHolder.keyword_data.keyword && x.KeywordSearchTermId == searchTermId);

                            if (specialCheck == null || string.IsNullOrEmpty(specialCheck.Keyword))
                            {
                                SpecialKeywords special = new SpecialKeywords();
                                special.Keyword = searchTermHolder.keyword_data.keyword;
                                special.KeywordSearchTermId = searchTermId;
                                special.TypeId = 4;
                                special.SourceId = 1;
                                specialKeywords.Add(special);
                            }
                        }
                    }
                }
                else
                {
                    return "Failed to call ranked keywords. Check credits first.";
                }
            }

            //get competing titles for each book
            //foreach (var asin in asinsToProcess)
            //{
            //    string endPoint = "amazon/product_competitors/live";

            //    string objectToSend = await MakeObjectToSend(asin);

            //    D4SeoAPIUtils d4SeoAPIUtils = new D4SeoAPIUtils();
            //    HttpResponseMessage responseMessage = await d4SeoAPIUtils.CallD4PostApi(endPoint, objectToSend);

            //    if (responseMessage.IsSuccessStatusCode)
            //    {
            //        CompetingAsins getValues = await JsonSerializer.DeserializeAsync<CompetingAsins>(responseMessage.Content.ReadAsStream());

            //        if (getValues != null && getValues.tasks[0].result[0].total_count > 0)
            //        {
            //            foreach (var searchTermHolder in getValues.tasks[0].result[0].items)
            //            {
            //                SpecialKeywords specialCheck = specialKeywords.Where(x => x.Keyword == searchTermHolder.asin && x.KeywordSearchTermId == searchTermId).FirstOrDefault();

            //                if (specialCheck == null)
            //                {
            //                    SpecialKeywords special = new SpecialKeywords();
            //                    special.Keyword = searchTermHolder.asin;
            //                    special.KeywordSearchTermId = searchTermId;
            //                    special.TypeId = 5;
            //                    special.SourceId = 1;
            //                    specialKeywords.Add(special);
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return "Failed to call competing products. Check credits first.";
            //    }
            //}




            //save results to db
            if (searchTermId > 0)
            {
                var saveResponse = await specialDbCalls.SaveNewKeywords(specialKeywords);

                if (!saveResponse)
                {
                    return "Failed to save new keywords";
                }
            }
            else
            {
                return "Failed to get search term id";
            }

            string processFinished = "process finished";
            return processFinished;
        }



        public async Task<string> MakeObjectToSend(string asin)
        {
            //make object to send
            List<D4Special> d4AzRelatedKeywordsRequestList = new List<D4Special>();

            D4Special d4AzRelatedKeywordsRequest = new D4Special();
            d4AzRelatedKeywordsRequest.asin = asin;
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
