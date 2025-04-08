using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.ProductManagement;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AdTool.Entities.D4Api;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.AsinError;
using Newtonsoft.Json;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create
{
    public class AddAsins
    {
        public async Task<string> AddTheseAsins(int CountryId, string CampaignID, CampaignRequest request, CountrySpecificRules CountryToCreate, string AdGroupId, string EndPoint, string MediaType, APIAuthorization Auth, List<string> InvalidAsins)
        {
            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            //make object
            string serlializedJson = await MakeObjectToSend(request, CampaignID, CountryToCreate, AdGroupId);

            ProductTargetResponseRoot myResponse = new ProductTargetResponseRoot();

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                myResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetResponseRoot>(responseMessage.Content.ReadAsStream());

                if (myResponse.targetingClauses.error.Count > 0)
                {
                    foreach (AsinErrorRoot invalidAsin in myResponse.targetingClauses.error)
                    {
                        var rejectedAsin = request.Asins.ElementAt(invalidAsin.index);
                        var invalidAsinPresent = InvalidAsins.Where(x => x == rejectedAsin).FirstOrDefault();

                        if (string.IsNullOrEmpty(invalidAsinPresent))
                        {
                            InvalidAsins.Add(rejectedAsin);
                        }
                    }
                }

                return "1";
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                if (responseMessage.IsSuccessStatusCode)
                {
                    myResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetResponseRoot>(responseMessage.Content.ReadAsStream());

                    if (myResponse.targetingClauses.error.Count > 0)
                    {
                        foreach (AsinErrorRoot invalidAsin in myResponse.targetingClauses.error)
                        {
                            var rejectedAsin = request.Asins.ElementAt(invalidAsin.index);
                            var invalidAsinPresent = InvalidAsins.Where(x => x == rejectedAsin).FirstOrDefault();

                            if (string.IsNullOrEmpty(invalidAsinPresent))
                            {
                                InvalidAsins.Add(rejectedAsin);
                            }

                            //old - keeping for reference
                            //if (invalidAsin.errors[0].errorType.ToLower() == "targetingclausesetuperror")
                            //{
                            //    string triggerError = invalidAsin.errors[0].errorValue.targetingClauseSetupError.cause.trigger;

                            //    string asinPt1 = triggerError.Replace("\\", "");
                            //    List<TriggerItem> triggerItem = JsonConvert.DeserializeObject<List<TriggerItem>>(asinPt1);

                            //    InvalidAsins.Add(triggerItem[0].value);
                            //}
                        }
                    }

                    return "1";
                }
                else
                {
                    return "0";
                }
            }

        }

        public async Task<string> MakeObjectToSend(CampaignRequest request, string CampaignId, CountrySpecificRules CountryToCreate, string AdGroupId)
        {
            ProductTargetRequestRoot productTargetRequestRoot = new ProductTargetRequestRoot(); //this holds your post parameters
            List<TargetingClause> targetingList = new List<TargetingClause>();

            foreach (var asin in request.Asins)
            {
                List<Expression> expressionList = new List<Expression>();
                Expression expressionValue = new Expression();
                expressionValue.type = "ASIN_SAME_AS";
                expressionValue.value = asin;
                expressionList.Add(expressionValue);


                TargetingClause targeting = new TargetingClause();
                targeting.expression = expressionList;
                targeting.campaignId = CampaignId;
                targeting.expressionType = "MANUAL";
                targeting.state = "ENABLED";
                targeting.bid = CountryToCreate.Bid;
                targeting.adGroupId = AdGroupId;
                targetingList.Add(targeting);
            }

            productTargetRequestRoot.targetingClauses = targetingList;

            string serlializedJson = System.Text.Json.JsonSerializer.Serialize(productTargetRequestRoot);

            return serlializedJson;
        }
    }
}
