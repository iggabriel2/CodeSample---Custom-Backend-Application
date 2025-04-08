using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword.ProductTargets.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.NegativeKeyword.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdTool.AzSponsoredProducts.BusinessObjects.NegativeKeyword.Get;
using AdTool.Entities.Logging;
using AdTool.BusinessLogic.Utilities;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ExtraKeywordPromoManagement
{
    public class AddNegativeProd
    {
        public async Task<string> SetNegativeProduct(int CountryId, CampaignRequest request, string EndPoint, string MediaType, APIAuthorization Auth, SaveSummaryReportAction negativesToApply)
        {
            AzAPIUtils azAPIUtils = new AzAPIUtils();

            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();


            string serlializedJsonCheck = "";

            try
            {
                //first see if it already exists
                serlializedJsonCheck = await MakeValidationObjectToSend(negativesToApply.AdGroup);

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
                        string negativeValue = negativesToApply.SearchTerm;
                        NegativeTargetingClause2 negativeResponse = new NegativeTargetingClause2();
                        negativeResponse = getValues.negativeTargetingClauses.Where(x => x.expression[0].value.ToLower().Trim() == negativeValue.ToLower().Trim() && x.expression[0].type == "ASIN_SAME_AS").FirstOrDefault();

                        if (negativeResponse != null && !string.IsNullOrEmpty(negativeResponse.expression[0].value))
                        {
                            return "1";
                        }
                    }
                }
                else
                {
                    //we didn't get anything back, so log the error and add the negative
                    await ErrorLogging.LogError("failed to get ngative product targets on SetNegativeProduct", "SetNegativeProduct", serlializedJsonCheck, Auth.ClientId);
                }
            }
            catch (Exception ex)
            {
                //log the error and keep going
                await ErrorLogging.LogError(ex.ToString(), "SetNegativeProduct", serlializedJsonCheck, Auth.ClientId);
            }


            //we're still here, so we didn't find a negative

            //make object
            string serlializedJson = await MakeObjectToSend(negativesToApply);

            //call api here
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                NegativeProductResponseRoot keywordResponse = new NegativeProductResponseRoot();
                keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<NegativeProductResponseRoot>(responseMessage.Content.ReadAsStream());

                if (keywordResponse.negativeTargetingClauses.error != null && keywordResponse.negativeTargetingClauses.error.Count > 0)
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
                    NegativeProductResponseRoot keywordResponse = new NegativeProductResponseRoot();
                    keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<NegativeProductResponseRoot>(responseMessage.Content.ReadAsStream());

                    if (keywordResponse.negativeTargetingClauses.error != null && keywordResponse.negativeTargetingClauses.error.Count > 0)
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

        public async Task<string> MakeObjectToSend(SaveSummaryReportAction negativesToApply)
        {
            NegativeProduct negativeQueryRoot = new NegativeProduct();

            NegativeTargetingClause negativeQueryItem = new NegativeTargetingClause();
            negativeQueryItem.campaignId = negativesToApply.AzCampaignId;
            negativeQueryItem.state = "ENABLED";
            negativeQueryItem.adGroupId = negativesToApply.AdGroup;


            BusinessObjects.SearchTermManagement.Expression expression = new BusinessObjects.SearchTermManagement.Expression();
            expression.type = "ASIN_SAME_AS";
            expression.value = negativesToApply.SearchTerm;
            negativeQueryItem.expression.Add(expression);

            negativeQueryRoot.negativeTargetingClauses.Add(negativeQueryItem);

            string serlializedJson = JsonConvert.SerializeObject(negativeQueryRoot);

            return serlializedJson;
        }

        private async Task<string> MakeValidationObjectToSend(string AdGroupId)
        {
            //make object to send
            GetNegativeProductTargetsRequest getNegativeProductTargets = new GetNegativeProductTargetsRequest(); //this holds your post parameters

            getNegativeProductTargets.adGroupIdFilter.include.Add(AdGroupId);

            //serialize object to send
            string serlializedJson = System.Text.Json.JsonSerializer.Serialize(getNegativeProductTargets);

            return serlializedJson;
        }
    }
}
