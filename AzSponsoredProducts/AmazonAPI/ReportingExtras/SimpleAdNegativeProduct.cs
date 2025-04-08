using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword.ProductTargets.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
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
    public class SimpleAdNegativeProduct
    {
        public async Task<string> SetNegativeProduct(NegativeProduct negativeProductQueryRoot, int CountryId, List<ClientProfileCodes> clientProfileCodes, string EndPoint, string MediaType, APIAuthorization Auth, List<NewAdGroupIds> InvlaidKeywords)
        {
            string responseValue = "1";

            ClientProfileCodes profileCode = clientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            AzAPIUtils azAPIUtils = new AzAPIUtils();

            string serlializedJsonCheck = "";

            try
            {
                //first see if it already exists
                serlializedJsonCheck = await MakeValidationObjectToSend(negativeProductQueryRoot);

                //call api here
                string mediaType2 = "application/vnd.spNegativeTargetingClause.v3+json";
                string endPoint2 = "sp/negativeTargets/list";
                HttpResponseMessage responseMessageValidation = await azAPIUtils.CallAmazonPostApi(endPoint2, mediaType2, Auth, profileCode, serlializedJsonCheck);

                if (responseMessageValidation.IsSuccessStatusCode)
                {
                    GetNegativeProductTargetsResponse getValues = new GetNegativeProductTargetsResponse();

                    try
                    {
                        getValues = await System.Text.Json.JsonSerializer.DeserializeAsync<GetNegativeProductTargetsResponse>(responseMessageValidation.Content.ReadAsStream());
                    }
                    catch (Exception ex)
                    {
                        //do nothing
                    }


                    //make sure it doesn't exist - we only support singles with this request, so only check one
                    if (getValues != null && getValues.totalResults > 0)
                    {
                        foreach (var value in getValues.negativeTargetingClauses)
                        {
                            negativeProductQueryRoot.negativeTargetingClauses.RemoveAll(x => x.expression[0].value == value.expression[0].value && x.adGroupId == value.adGroupId);
                        }
                    }
                }
                else
                {
                    //we didn't get anything back, so log the error and add the negative
                    await ErrorLogging.LogError("failed to get ngative product targets on SetNegativeProduct on SimpleAdNegativeProduct", "SetNegativeProduct", serlializedJsonCheck, Auth.ClientId);
                }
            }
            catch (Exception ex)
            {
                //log the error and keep going
                await ErrorLogging.LogError(ex.ToString(), "SetNegativeProduct on SimpleAdNegativeProduct", serlializedJsonCheck, Auth.ClientId);
            }


            //let's process the ones we didn't remove








            //make object
            string serlializedJson = JsonSerializer.Serialize(negativeProductQueryRoot);

            //call api here
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                NegativeProductResponseRoot keywordResponse = new NegativeProductResponseRoot();
                keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<NegativeProductResponseRoot>(responseMessage.Content.ReadAsStream());

                if (keywordResponse.negativeTargetingClauses.error != null && keywordResponse.negativeTargetingClauses.error.Count > 0)
                {
                    foreach (var invalidKeywordId in keywordResponse.negativeTargetingClauses.error)
                    {
                        var rejectedKeyword = negativeProductQueryRoot.negativeTargetingClauses.ElementAt(invalidKeywordId.index);
                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                        invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                        invalidKeyword.KeywordText = rejectedKeyword.expression[0].value;
                        invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                        InvlaidKeywords.Add(invalidKeyword);
                    }
                }
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                if (responseMessage.IsSuccessStatusCode)
                {
                    NegativeProductResponseRoot keywordResponse = new NegativeProductResponseRoot();
                    keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<NegativeProductResponseRoot>(responseMessage.Content.ReadAsStream());

                    if (keywordResponse.negativeTargetingClauses.error != null && keywordResponse.negativeTargetingClauses.error.Count > 0)
                    {
                        foreach (var invalidKeywordId in keywordResponse.negativeTargetingClauses.error)
                        {
                            var rejectedKeyword = negativeProductQueryRoot.negativeTargetingClauses.ElementAt(invalidKeywordId.index);
                            NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                            invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                            invalidKeyword.KeywordText = rejectedKeyword.expression[0].value;
                            invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                            InvlaidKeywords.Add(invalidKeyword);
                        }
                    }
                }
                else
                {
                    foreach (var invalidKeywordId in negativeProductQueryRoot.negativeTargetingClauses)
                    {
                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                        invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                        invalidKeyword.KeywordText = invalidKeywordId.expression[0].value;
                        invalidKeyword.OldAdGroupId = invalidKeywordId.adGroupId;
                        InvlaidKeywords.Add(invalidKeyword);
                    }

                    return "0";
                }
            }

            return responseValue;
        }

        private async Task<string> MakeValidationObjectToSend(NegativeProduct negativeProductQueryRoot)
        {
            //make object to send
            GetNegativeProductTargetsRequest getNegativeProductTargets = new GetNegativeProductTargetsRequest(); //this holds your post parameters

            foreach (var product in negativeProductQueryRoot.negativeTargetingClauses)
            {
                if (!getNegativeProductTargets.adGroupIdFilter.include.Contains(product.adGroupId))
                {
                    getNegativeProductTargets.adGroupIdFilter.include.Add(product.adGroupId);
                }
            }

            //serialize object to send
            string serlializedJson = System.Text.Json.JsonSerializer.Serialize(getNegativeProductTargets);

            return serlializedJson;
        }

    }
}
