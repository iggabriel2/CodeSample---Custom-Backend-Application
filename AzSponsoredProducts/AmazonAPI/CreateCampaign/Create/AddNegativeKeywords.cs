using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.D4Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdTool.AzSponsoredProducts.BusinessObjects.NegativeKeyword.Get;
using AdTool.Entities.Logging;
using AdTool.BusinessLogic.Utilities;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create
{
    public class AddNegativeKeywords
    {
        public async Task<string> AddTheseNegativeKeywords(int CountryId, List<string> AdGroupIds, string CampaignID, CampaignRequest request, string EndPoint, string MediaType, APIAuthorization Auth)
        {
            AzAPIUtils azAPIUtils = new AzAPIUtils();

            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            string serlializedJsonCheck = "";
            string serlializedJson = "";

            try
            {
                //first see if it already exists
                serlializedJsonCheck = await MakeValidationObjectToSend(AdGroupIds);

                //make object
                NegativeQueryRoot negativeQueryRoot = await MakeRawObjectToSend(request, CampaignID, AdGroupIds);

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


                    //make sure it doesn't exist - we only support singles with this request unless it is a new campaign, so only check one
                    if (getValues != null && getValues.totalResults > 0)
                    {
                        foreach (var value in getValues.negativeKeywords)
                        {
                            negativeQueryRoot.negativeKeywords.RemoveAll(x => x.keywordText == value.keywordText && x.campaignId == value.campaignId && x.adGroupId == value.adGroupId && x.matchType.ToLower().Trim() == value.matchType.ToLower().Trim());
                        }
                    }

                }
                else
                {
                    //we didn't get anything back, so log the error and add the negative
                    await ErrorLogging.LogError("failed to get ngative keywords on AddTheseNegativeKeywords", "AddTheseNegativeKeywords", serlializedJsonCheck, Auth.ClientId);
                }

                serlializedJson = JsonConvert.SerializeObject(negativeQueryRoot);
            }
            catch(Exception ex)
            {
                //log the error and keep going
                await ErrorLogging.LogError(ex.ToString(), "AddTheseNegativeKeywords", serlializedJsonCheck, Auth.ClientId);
            }


            //we're still here, so we didn't find a negative

            //call api here
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                NegativeResponseRoot keywordResponse = new NegativeResponseRoot();
                keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<NegativeResponseRoot>(responseMessage.Content.ReadAsStream());

                if (keywordResponse.negativeKeywords.error != null && keywordResponse.negativeKeywords.error.Count > 0)
                {
                    return "0";
                }
                else
                {
                    return "1";
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
                        return "0";
                    }
                    else
                    {
                        return "1";
                    }
                }
                else
                {
                    return "0";
                }
            }
        }

        public async Task<NegativeQueryRoot> MakeRawObjectToSend(CampaignRequest request, string CampaignId, List<string> AdGroupIds)
        {
            NegativeQueryRoot negativeQueryRoot = new NegativeQueryRoot();

            foreach(var adgroundId in AdGroupIds)
            {
                foreach (var negative in request.NegativeKeywordsNewCampaigns)
                {
                    NegativeKeywords negativeQueryItem = new NegativeKeywords();
                    negativeQueryItem.campaignId = CampaignId;
                    negativeQueryItem.state = "ENABLED";
                    negativeQueryItem.keywordText = negative.NegativeKeyword;
                    negativeQueryItem.adGroupId = adgroundId;

                    if (negative.BlockType.ToLower() == "phrase")
                    {
                        negativeQueryItem.matchType = "NEGATIVE_PHRASE";
                    }
                    else
                    {
                        negativeQueryItem.matchType = "NEGATIVE_EXACT";
                    }

                    negativeQueryRoot.negativeKeywords.Add(negativeQueryItem);
                }
            }

            return negativeQueryRoot;

        }

        private async Task<string> MakeValidationObjectToSend(List<string> AdGroupIds)
        {
            //make object to send
            GetNegativeKeywordRequest getNegativeKeywords = new GetNegativeKeywordRequest(); //this holds your post parameters

            foreach(var adGroupId in AdGroupIds)
            {
                getNegativeKeywords.adGroupIdFilter.include.Add(adGroupId);
            }

            //serialize object to send
            string serlializedJson = System.Text.Json.JsonSerializer.Serialize(getNegativeKeywords);

            return serlializedJson;
        }
    }
}
