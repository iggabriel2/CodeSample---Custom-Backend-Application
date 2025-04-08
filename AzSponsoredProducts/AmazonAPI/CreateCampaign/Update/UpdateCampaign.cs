using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Save;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Update
{
    public class UpdateCampaign
    {
        public async Task<string> Update(int CountryId, CampaignUpdateRequest request, string EndPoint, string MediaType, APIAuthorization Auth)
        {
            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            //make object
            string serlializedJson = await MakeObjectToSend(request);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPutApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                CampaignUpdateResponse updateCampaignResponse = new CampaignUpdateResponse();
                updateCampaignResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<CampaignUpdateResponse>(responseMessage.Content.ReadAsStream());

                if (updateCampaignResponse.campaigns.error != null && updateCampaignResponse.campaigns.error.Count > 0)
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
                responseMessage = await azAPIUtils.CallAmazonPutApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                if (responseMessage.IsSuccessStatusCode)
                {
                    CampaignUpdateResponse updateCampaignResponse = new CampaignUpdateResponse();
                    updateCampaignResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<CampaignUpdateResponse>(responseMessage.Content.ReadAsStream());

                    if (updateCampaignResponse.campaigns.error != null && updateCampaignResponse.campaigns.error.Count > 0)
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

        public async Task<string> MakeObjectToSend(Entities.AzSpApi.CampaignManagement.CampaignUpdateRequest request)
        {
            //make object to send
            CampaignUpdateRequestAzApi responseAz = new CampaignUpdateRequestAzApi();

            Campaign campaign = new Campaign();
            campaign.campaignId = request.CampaignId;
            campaign.name = request.CampaignName;
            //campaign.targetingType = request.TargetingType;
            campaign.state = request.state;

            if (request.DynamicBiddingStrategy.ToLower() == "1")
            {
                campaign.dynamicBidding.strategy = "LEGACY_FOR_SALES";
            }
            else if (request.DynamicBiddingStrategy.ToLower() == "2")
            {
                campaign.dynamicBidding.strategy = "AUTO_FOR_SALES";
            }
            else
            {
                campaign.dynamicBidding.strategy = "MANUAL";
            }

            campaign.budget.budgetType = "DAILY";
            campaign.budget.budget = request.Budget;


            responseAz.campaigns.Add(campaign);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(responseAz);

            return serlializedJson;
        }
    }
}
