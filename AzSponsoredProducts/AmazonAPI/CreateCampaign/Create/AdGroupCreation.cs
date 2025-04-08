using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create
{
    public class AdGroupCreation
    {
        public async Task<string> CreateThisAdGroup(int CountryId, string CampaignID, string AdGroupName, CampaignRequest request, CountrySpecificRules CountryToCreate, string EndPoint, string MediaType, APIAuthorization Auth)
        {

            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            //make object
            string serlializedJson = await MakeObjectToSend(request, CountryToCreate, CampaignID, AdGroupName);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            AdGroupResposeRoot adGroupResponse = new AdGroupResposeRoot();
            if (responseMessage.IsSuccessStatusCode)
            {
                adGroupResponse = await JsonSerializer.DeserializeAsync<AdGroupResposeRoot>(responseMessage.Content.ReadAsStream());
            }

            if (responseMessage.IsSuccessStatusCode && adGroupResponse.adGroups.error.Count == 0)
            {
                return adGroupResponse.adGroups.success[0].adGroupId;
            }
            else
            {
                //recheck and try again
                string serlializedJsonCheck = await MakeValidationObjectToSend(CampaignID);

                //call api here
                string mediaType2 = "application/vnd.spAdGroup.v3+json";
                string endPoint2 = "sp/adGroups/list";
                HttpResponseMessage responseMessageValidation = await azAPIUtils.CallAmazonPostApi(endPoint2, mediaType2, Auth, profileCode, serlializedJsonCheck);

                if (responseMessageValidation.IsSuccessStatusCode)
                {
                    GetAdGroupResponse getValues = new GetAdGroupResponse();

                    try
                    {
                        getValues = await JsonSerializer.DeserializeAsync<GetAdGroupResponse>(responseMessageValidation.Content.ReadAsStream());
                    }
                    catch (Exception ex)
                    {
                        //do nothing
                    }


                    //make sure it exists
                    if (getValues != null && getValues.totalResults > 0)
                    {
                        BusinessObjects.CreateCampaign.Get.AdGroup adGroupPresent = new BusinessObjects.CreateCampaign.Get.AdGroup();
                        adGroupPresent = getValues.adGroups.Where(x => x.name == AdGroupName).FirstOrDefault();

                        if (adGroupPresent != null && !string.IsNullOrEmpty(adGroupPresent.name))
                        {
                            return adGroupPresent.adGroupId;
                        }
                        else
                        {
                            //call api here
                            responseMessage = new HttpResponseMessage();
                            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                            adGroupResponse = new AdGroupResposeRoot();
                            if (responseMessage.IsSuccessStatusCode)
                            {
                                adGroupResponse = await JsonSerializer.DeserializeAsync<AdGroupResposeRoot>(responseMessage.Content.ReadAsStream());
                            }

                            if (responseMessage.IsSuccessStatusCode && adGroupResponse.adGroups.error.Count == 0)
                            {
                                return adGroupResponse.adGroups.success[0].adGroupId;
                            }
                            else
                            {
                                return "0";
                            }

                        }
                    }
                    else
                    {
                        //call api here
                        responseMessage = new HttpResponseMessage();
                        responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                        adGroupResponse = new AdGroupResposeRoot();
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            adGroupResponse = await JsonSerializer.DeserializeAsync<AdGroupResposeRoot>(responseMessage.Content.ReadAsStream());
                        }

                        if (responseMessage.IsSuccessStatusCode && adGroupResponse.adGroups.error.Count == 0)
                        {
                            return adGroupResponse.adGroups.success[0].adGroupId;
                        }
                        else
                        {
                            return "0";
                        }
                    }
                }
                else
                {
                    return "0";
                }
            }
        }

        public async Task<string> MakeObjectToSend(CampaignRequest request, CountrySpecificRules CountryToCreate, string CampaignID, string AdGroupName)
        {
            AdGroupRequestRoot adGroupRequestRoot = new AdGroupRequestRoot();
            List<APIAdGroupsRequest> adGroupsRequest = new List<APIAdGroupsRequest>();

            APIAdGroupsRequest adGroupsRequestItem = new APIAdGroupsRequest(); //this holds your post parameters
            adGroupsRequestItem.campaignId = CampaignID;
            adGroupsRequestItem.defaultBid = (float)CountryToCreate.Bid;
            adGroupsRequestItem.name = AdGroupName;
            adGroupsRequestItem.state = "ENABLED";
            adGroupsRequest.Add(adGroupsRequestItem);

            adGroupRequestRoot.adGroups = adGroupsRequest;

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(adGroupRequestRoot);

            return serlializedJson;
        }

        private async Task<string> MakeValidationObjectToSend(string CampaignID)
        {
            //make object to send
            GetAdGroups getAdGroups = new GetAdGroups(); //this holds your post parameters

            getAdGroups.campaignIdFilter.include.Add(CampaignID);
            getAdGroups.stateFilter.include.Add("ENABLED");

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(getAdGroups);

            return serlializedJson;
        }
    }
}
