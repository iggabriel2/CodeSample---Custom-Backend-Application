using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.NegativeKeyword.Get;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CampaignExtra
{
    public class SimpleAdNegativeKeywords
    {
        public async Task<string> AddTheseNegativeKeywords(NegativeQueryRoot negativeQueryRoot, int CountryId, List<ClientProfileCodes> clientProfileCodes, string EndPoint, string MediaType, APIAuthorization Auth, List<NewAdGroupIds> InvlaidKeywords)
        {
            string responseValue = "1";

            ClientProfileCodes profileCode = clientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            AzAPIUtils azAPIUtils = new AzAPIUtils();

            string serlializedJsonCheck = "";

            try
            {
                //first see if it already exists
                serlializedJsonCheck = await MakeValidationObjectToSend(negativeQueryRoot);

                //call api here
                string mediaType2 = "application/vnd.spNegativeKeyword.v3+json";
                string endPoint2 = "sp/negativeKeywords/list";
                HttpResponseMessage responseMessageValidation = await azAPIUtils.CallAmazonPostApi(endPoint2, mediaType2, Auth, profileCode, serlializedJsonCheck);

                if (responseMessageValidation.IsSuccessStatusCode)
                {
                    GetNegativeKeywordResponse getValues = new GetNegativeKeywordResponse();

                    try
                    {
                        getValues = await System.Text.Json.JsonSerializer.DeserializeAsync<GetNegativeKeywordResponse>(responseMessageValidation.Content.ReadAsStream());
                    }
                    catch (Exception ex)
                    {
                        //do nothing
                    }


                    if (getValues != null && getValues.totalResults > 0)
                    {
                        foreach(var value in getValues.negativeKeywords)
                        {
                            negativeQueryRoot.negativeKeywords.RemoveAll(x => x.keywordText == value.keywordText && x.campaignId == value.campaignId && x.adGroupId == value.adGroupId);
                        }
                    }
                }
                else
                {
                    //we didn't get anything back, so log the error and add the negative
                    await ErrorLogging.LogError("failed to get negative keywords on AddTheseNegativeKeywords in SimpleAdNegativeKeywords", "AddTheseNegativeKeywords", serlializedJsonCheck, Auth.ClientId);
                }
            }
            catch (Exception ex)
            {
                //log the error and keep going
                await ErrorLogging.LogError(ex.ToString(), "AddTheseNegativeKeywords in SimpleAdNegativeKeywords", serlializedJsonCheck, Auth.ClientId);
            }


            //let's process the ones we didn't remove

            //make object
            string serlializedJson = JsonSerializer.Serialize(negativeQueryRoot);

            //call api here
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                NegativeResponseRoot keywordResponse = new NegativeResponseRoot();
                keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<NegativeResponseRoot>(responseMessage.Content.ReadAsStream());

                if (keywordResponse.negativeKeywords.error != null && keywordResponse.negativeKeywords.error.Count > 0)
                {
                    foreach (var invalidKeywordId in keywordResponse.negativeKeywords.error)
                    {
                        var rejectedKeyword = negativeQueryRoot.negativeKeywords.ElementAt(invalidKeywordId.index);
                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                        invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                        invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                        invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                        InvlaidKeywords.Add(invalidKeyword);
                    }
                }
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                if (responseMessage.IsSuccessStatusCode)
                {
                    NegativeResponseRoot keywordResponse = new NegativeResponseRoot();
                    keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<NegativeResponseRoot>(responseMessage.Content.ReadAsStream());

                    if (keywordResponse.negativeKeywords.error != null && keywordResponse.negativeKeywords.error.Count > 0)
                    {
                        foreach (var invalidKeywordId in keywordResponse.negativeKeywords.error)
                        {
                            var rejectedKeyword = negativeQueryRoot.negativeKeywords.ElementAt(invalidKeywordId.index);
                            NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                            invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                            invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                            invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                            InvlaidKeywords.Add(invalidKeyword);
                        }
                    }
                }
                else
                {
                    foreach (var invalidKeywordId in negativeQueryRoot.negativeKeywords)
                    {
                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                        invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                        invalidKeyword.OldAdGroupId = invalidKeywordId.adGroupId;
                        invalidKeyword.KeywordText = invalidKeywordId.keywordText;
                        InvlaidKeywords.Add(invalidKeyword);
                    }

                    return "0";
                }
            }

            return responseValue;
        }

        private async Task<string> MakeValidationObjectToSend(NegativeQueryRoot negativeQueryRoot)
        {
            //make object to send
            GetNegativeKeywordRequest getNegativeKeywords = new GetNegativeKeywordRequest(); //this holds your post parameters

            foreach(var keyword in negativeQueryRoot.negativeKeywords)
            {
                if (!getNegativeKeywords.adGroupIdFilter.include.Contains(keyword.adGroupId))
                {
                    getNegativeKeywords.adGroupIdFilter.include.Add(keyword.adGroupId);
                }
            }
     
            //serialize object to send
            string serlializedJson = System.Text.Json.JsonSerializer.Serialize(getNegativeKeywords);

            return serlializedJson;
        }
    }
}
