using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using System.Security.Cryptography;
using AdTool.Entities.AzSpApi.CampaignCreations;
using System.Text.RegularExpressions;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create
{
    public class AddKeywords
    {
        public async Task<string> AddTheseKeywords(int CountryId, string CampaignID, List<string> broadAdGroupIds, List<string> phraseAdGroupIds, List<string> exactAdGroupIds, CampaignRequest request, CountrySpecificRules CountryToCreate, string EndPoint, string MediaType, APIAuthorization Auth, List<string> InvlaidKeywords, List<string> DuplicateKeywords)
        {

            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            //Holds entire list of keywords
            KeywordRequestRoot keywordRequestMaster = new KeywordRequestRoot(); //this holds your post parameters
            KeywordResponseRoot keywordResponseMaster = new KeywordResponseRoot(); //this holds response
            string responseValue = "1";

            //count the number of keywords we have added
            int totalBroadKeywordCount = 0;
            int totalPhraseKeywordCount = 0;
            int totalExactKeywordCount = 0;
            int broadAdGroupIdToUse = 0;
            int phraseAdGroupIdToUse = 0;
            int ExactAdGroupIdToUse = 0;
            int broadNextIncrement = 1000;
            int phraseNextIncrement = 1000;
            int exactNextIncrement = 1000;
            int calculateIncriment = 1000;

            List<string> keywordsProcessed = new List<string>();
            List<string> distinctKeywords = request.Keywords.Distinct().ToList();

            foreach (var rawKeywordStart in distinctKeywords)
            {

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                string rawKeywordNoExtaSpace = regex.Replace(rawKeywordStart, " ");

                var cleanKeyword1 = rawKeywordNoExtaSpace.Replace("#", " ").Replace(".", "").Replace(",", "").Replace("-", " ").Replace("&", " ").Replace(":", "").Replace("/", "").Replace("\"", "");
                var cleanKeyword =  regex.Replace(cleanKeyword1, " ");


                var keywordFound = keywordsProcessed.Where(x => x == cleanKeyword).FirstOrDefault();
                if (!string.IsNullOrEmpty(keywordFound))
                {
                    DuplicateKeywords.Add(keywordFound);
                }
                else 
                {
                    keywordsProcessed.Add(cleanKeyword);

                    foreach (var kType in request.KeywordTypes)
                    {
                        APIKeyword apiKeyword = new APIKeyword();
                        apiKeyword.campaignId = CampaignID;
                        apiKeyword.bid = CountryToCreate.Bid;
                        apiKeyword.keywordText = cleanKeyword;
                        apiKeyword.matchType = kType.ToUpper();
                        apiKeyword.state = "ENABLED";

                        if (kType.ToLower() == "broad")
                        {
                            totalBroadKeywordCount++;

                            if (totalBroadKeywordCount > broadNextIncrement)
                            {
                                broadNextIncrement = broadNextIncrement + calculateIncriment;
                                broadAdGroupIdToUse++;
                            }

                            apiKeyword.adGroupId = broadAdGroupIds.ElementAt(broadAdGroupIdToUse);
                        }
                        else if (kType.ToLower() == "phrase")
                        {
                            totalPhraseKeywordCount++;

                            if (totalPhraseKeywordCount > phraseNextIncrement)
                            {
                                phraseNextIncrement = phraseNextIncrement + calculateIncriment;
                                phraseAdGroupIdToUse++;
                            }

                            apiKeyword.adGroupId = phraseAdGroupIds.ElementAt(phraseAdGroupIdToUse);
                        }
                        else if (kType.ToLower() == "exact")
                        {
                            totalExactKeywordCount++;

                            if (totalExactKeywordCount > exactNextIncrement)
                            {
                                exactNextIncrement = exactNextIncrement + calculateIncriment;
                                ExactAdGroupIdToUse++;
                            }

                            apiKeyword.adGroupId = exactAdGroupIds.ElementAt(ExactAdGroupIdToUse);
                        }

                        keywordRequestMaster.keywords.Add(apiKeyword);
                    }
                }


            }

            //seperate list and run request
            int maxTotalKeywordCountPerGroup = 1000;
            double totalAdGroups = Math.Ceiling((double)keywordRequestMaster.keywords.Count() / maxTotalKeywordCountPerGroup);
            for (var x = 0; x < totalAdGroups; x++)
            {
                int recordsToSkip = x * maxTotalKeywordCountPerGroup;
                KeywordRequestRoot keywordRequest = new KeywordRequestRoot();
                List<APIKeyword> keywordsToAdd = keywordRequestMaster.keywords.Skip(recordsToSkip).Take(maxTotalKeywordCountPerGroup).ToList();
                keywordRequest.keywords = keywordsToAdd;

                string serlializedJson = JsonSerializer.Serialize(keywordRequest);
                var data = serlializedJson;

                //call api here
                AzAPIUtils azAPIUtils = new AzAPIUtils();
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                KeywordResponseRoot keywordResponse = new KeywordResponseRoot();
                if (responseMessage.IsSuccessStatusCode)
                {
                    keywordResponse = await JsonSerializer.DeserializeAsync<KeywordResponseRoot>(responseMessage.Content.ReadAsStream());

                    foreach (var invalidKeywordId in keywordResponse.keywords.error)
                    {
                        var rejectedKeyword = keywordRequest.keywords.ElementAt(invalidKeywordId.index);
                        var invalidKeywordPresent = InvlaidKeywords.Where(x => x == rejectedKeyword.keywordText).FirstOrDefault();

                        if (string.IsNullOrEmpty(invalidKeywordPresent))
                        {
                            InvlaidKeywords.Add(rejectedKeyword.keywordText);
                        }
                    }
                }
                else
                {
                    responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        keywordResponse = await JsonSerializer.DeserializeAsync<KeywordResponseRoot>(responseMessage.Content.ReadAsStream());

                        foreach (var invalidKeywordId in keywordResponse.keywords.error)
                        {
                            var rejectedKeyword = keywordRequest.keywords.ElementAt(invalidKeywordId.index);
                            var invalidKeywordPresent = InvlaidKeywords.Where(x => x == rejectedKeyword.keywordText).FirstOrDefault();

                            if (string.IsNullOrEmpty(invalidKeywordPresent))
                            {
                                InvlaidKeywords.Add(rejectedKeyword.keywordText);
                            }
                        }
                    }
                    else
                    {
                        return "0";
                    }
                }
            }

            return responseValue;
        }

    }
}
