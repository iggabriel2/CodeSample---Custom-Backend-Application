using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.CampaignCreations;
using Azure;
using Azure.Core;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create
{
    public class CampaignCreation
    {
        //make object to send, check that it exists, remake if it doesn't


        public async Task<string> CreateThisCampaign(int CountryId, CampaignRequest CampaignInfo, CountrySpecificRules CountryToCreate, string EndPoint, string MediaType, APIAuthorization Auth )
        {

            ClientProfileCodes profileCode = CampaignInfo.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();
            
            //make object
            string serlializedJson = await MakeObjectToSend(CampaignInfo, CountryToCreate);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            CampaignResponseRoot campaignResponse = new CampaignResponseRoot();
            if (responseMessage.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                campaignResponse = await JsonSerializer.DeserializeAsync<CampaignResponseRoot>(responseMessage.Content.ReadAsStream(), options);
            }

            if (responseMessage.IsSuccessStatusCode && campaignResponse.campaigns.error.Count == 0)
            {
                return campaignResponse.campaigns.success[0].campaignId;
            }
            else
            {
                //recheck and try again
                string serlializedJsonCheck = await MakeValidationObjectToSend(CampaignInfo.ProductAsinsAndCampaignNames[0].CampaignName);

                //call api here
                string mediaType2 = "application/vnd.spCampaign.v3+json";
                string endPoint2 = "sp/campaigns/list";
                HttpResponseMessage responseMessageValidation = await azAPIUtils.CallAmazonPostApi(endPoint2, mediaType2, Auth, profileCode, serlializedJsonCheck);

                if (responseMessageValidation.IsSuccessStatusCode)
                {
                    GetCampaignResponse getValues = new GetCampaignResponse();

                    try
                    {
                        getValues = await JsonSerializer.DeserializeAsync<GetCampaignResponse>(responseMessageValidation.Content.ReadAsStream());
                    }
                    catch(Exception ex)
                    {
                        //do nothing
                    }
  

                    //make sure it exists
                    if (getValues != null && getValues.totalResults > 0)
                    {
                        BusinessObjects.CreateCampaign.Get.Campaign campaignPresent = new BusinessObjects.CreateCampaign.Get.Campaign();
                        campaignPresent = getValues.campaigns.Where(x => x.name == CampaignInfo.ProductAsinsAndCampaignNames[0].CampaignName).FirstOrDefault();

                        if (campaignPresent != null && !string.IsNullOrEmpty(campaignPresent.name))
                        {
                            if (CampaignInfo.Resubmit == 0)
                            {
                                return "2";
                            }
                            else
                            {
                                return campaignPresent.campaignId;
                            }
                        }
                        else
                        {

                            //if it doesn't, call again
                            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                            if (responseMessage.IsSuccessStatusCode)
                            {
                                campaignResponse = await JsonSerializer.DeserializeAsync<CampaignResponseRoot>(responseMessage.Content.ReadAsStream());
                            }

                            if (responseMessage.IsSuccessStatusCode && campaignResponse.campaigns.error.Count == 0)
                            {
                                return campaignResponse.campaigns.success[0].campaignId;
                            }
                            else
                            {
                                return "0";
                            }
                        }
                    }
                    else
                    {
                        //if it doesn't, call again
                        responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            campaignResponse = await JsonSerializer.DeserializeAsync<CampaignResponseRoot>(responseMessage.Content.ReadAsStream());
                        }

                        if (responseMessage.IsSuccessStatusCode && campaignResponse.campaigns.error.Count == 0)
                        {
                            return campaignResponse.campaigns.success[0].campaignId;
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

        public async Task<string> MakeObjectToSend(CampaignRequest CampaignInfo, CountrySpecificRules CountryToCreate)
        {

            CampaignQueryRoot campaignQueryRoot = new CampaignQueryRoot();
            CampaignBudget cbudget = new CampaignBudget();
            cbudget.budgetType = "DAILY";
            cbudget.budget = CountryToCreate.Budget;

            BusinessObjects.CreateCampaign.Create.Campaign campaign = new BusinessObjects.CreateCampaign.Create.Campaign();
            campaign.portfolioId = CountryToCreate.AzPortfolioId;
            campaign.state = "ENABLED";
            campaign.budget = cbudget;
            campaign.name = CampaignInfo.ProductAsinsAndCampaignNames[0].CampaignName;


            //set targeting
            BusinessObjects.CreateCampaign.Create.DynamicBidding dynamicBidding = new BusinessObjects.CreateCampaign.Create.DynamicBidding();

            BusinessObjects.CreateCampaign.Create.PlacementBidding placementBidding = new BusinessObjects.CreateCampaign.Create.PlacementBidding();
            placementBidding.placement = "PLACEMENT_TOP";
            placementBidding.percentage = CountryToCreate.TopOfSearch;
            dynamicBidding.placementBidding.Add(placementBidding);

            BusinessObjects.CreateCampaign.Create.PlacementBidding placementBidding2 = new BusinessObjects.CreateCampaign.Create.PlacementBidding();
            placementBidding2.placement = "PLACEMENT_PRODUCT_PAGE";
            placementBidding2.percentage = CountryToCreate.ProductPages;
            dynamicBidding.placementBidding.Add(placementBidding2);

            if (CountryToCreate.BiddingStrategy.ToLower() == "down")
            {
                dynamicBidding.strategy = "LEGACY_FOR_SALES";
            }
            else if (CountryToCreate.BiddingStrategy.ToLower() == "updown")
            {
                dynamicBidding.strategy = "AUTO_FOR_SALES";
            }
            else
            {
                dynamicBidding.strategy = "MANUAL";
            }
        
            if (CampaignInfo.CampaignType == 1)
            {
                campaign.targetingType = "MANUAL";
            }
            else
            {
                campaign.targetingType = "AUTO";
            }


            campaign.dynamicBidding = dynamicBidding;

            campaignQueryRoot.campaigns.Add(campaign);


            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(campaignQueryRoot);

            return serlializedJson;
        }


        //CUSTOMIZE OBJECT
        private async Task<string> MakeValidationObjectToSend(string CampaignNameRequested)
        {
            //make object to send
            GetCampaignRequest getCampaignRequest = new GetCampaignRequest(); //this holds your post parameters

            NameFilter nameFilter = new NameFilter();
            nameFilter.queryTermMatchType = "EXACT_MATCH";
            nameFilter.include.Add(CampaignNameRequested);
            getCampaignRequest.nameFilter = nameFilter;

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(getCampaignRequest);

            return serlializedJson;
        }
    }
}
