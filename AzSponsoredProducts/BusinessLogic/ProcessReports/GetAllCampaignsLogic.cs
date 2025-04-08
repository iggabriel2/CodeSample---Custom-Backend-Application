using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Save;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.Logging;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class GetAllCampaignsLogic
    {
        bool callSuccess = false;

        public async Task<bool> GetAllCampaigns(APIAuthorizationRequest aPIAuthorizationRequest, ClientProfileCodes? profileCode)
        {
            List<CampaignSaveBatch> campaigns = new List<CampaignSaveBatch>();

            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/vnd.spCampaign.v3+json";
            string endPoint = "sp/campaigns/list";

            //get token if I need a new one
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(aPIAuthorizationRequest);

            try
            {
                //make object
                string? nextToken = null;
                int totalResults = 0;
                int resultsRetrieved = 0;

                GetCampaignResponse myResponse = new GetCampaignResponse();

                do
                {
                    string serlializedJson = await MakeObjectToSend(nextToken);

                    //call api here
                    myResponse = await CallApiAndHandleResponse(mediaType, endPoint, auth, profileCode, serlializedJson);

                    if (myResponse != null)
                    {
                        nextToken = myResponse.nextToken;
                        resultsRetrieved = resultsRetrieved + myResponse.campaigns.Count;

                        if (totalResults == 0)
                        {
                            totalResults = myResponse.totalResults;
                        }

                        foreach (var campaign in myResponse.campaigns)
                        {
                            CampaignSaveBatch campaignSave = new CampaignSaveBatch();

                            //set bidding type
                            if (campaign.dynamicBidding.strategy == "AUTO_FOR_SALES")
                            {
                                campaignSave.DynamicBiddingStrategy = 2;
                            }
                            else if (campaign.dynamicBidding.strategy == "LEGACY_FOR_SALES")
                            {
                                campaignSave.DynamicBiddingStrategy = 1;
                            }
                            else if (campaign.dynamicBidding.strategy == "MANUAL")
                            {
                                campaignSave.DynamicBiddingStrategy = 3;
                            }

                            campaignSave.AZCampaignId = campaign.campaignId;
                            campaignSave.ProductId = 0;
                            campaignSave.CountryId = profileCode.CountryId;
                            campaignSave.AzSpCampaignUsageType = 1;
                            campaignSave.AzSpPrimaryInUsageType = false;
                            campaignSave.AzPortfolioId = campaign.portfolioId;
                            campaignSave.GeneratedByUs = false;
                            campaignSave.AzClientId = aPIAuthorizationRequest.ClientId;
                            campaignSave.State = campaign.state;
                            campaignSave.TargetingType = campaign.targetingType;

                            try
                            {
                                campaignSave.Budget = campaign.budget.budget;
                            }
                            catch (Exception ex)
                            {
                                Logging logging = new Logging();
                                LogError logError = new LogError();
                                logError.ErrorMessage = "Failed to convert budget on GetAllCampaigns";
                                logError.FailureMethod = "GetAllCampaigns";
                                logError.ClientId = aPIAuthorizationRequest.ClientId;
                                logError.Parameters = JsonSerializer.Serialize(aPIAuthorizationRequest) + JsonSerializer.Serialize(profileCode);
                                await logging.WriteToLog(logError);
                            }

                            if (campaign.state.ToLower() == "enabled")
                            {
                                campaignSave.Active = true;
                            }
                            else
                            {
                                campaignSave.Active = false;
                            }

                            if (campaign.name.Contains("#"))
                            {
                                campaignSave.CampaignName = campaign.name.Substring(0, campaign.name.IndexOf("#"));
                            }
                            else
                            {
                                campaignSave.CampaignName = campaign.name;
                            }

                            campaigns.Add(campaignSave);
                        }
                    }
                }
                while (myResponse != null && resultsRetrieved < totalResults && nextToken != null);
                
                //update the db with any successes
                SaveData saveData = new SaveData();
                var saveResponse = await saveData.SaveCampaignsBatch(campaigns, aPIAuthorizationRequest.ClientId, profileCode.CountryId);
                
                //mark the failure and return if it fails
                if (myResponse == null)
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = "Failed to get campaign info";
                    logError.FailureMethod = "GetAllCampaigns";
                    logError.ClientId = aPIAuthorizationRequest.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(aPIAuthorizationRequest) + JsonSerializer.Serialize(profileCode);
                    await logging.WriteToLog(logError);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = "Failed to get campaign info";
                logError.FailureMethod = "GetAllCampaigns";
                logError.ClientId = aPIAuthorizationRequest.ClientId;
                logError.Parameters = JsonSerializer.Serialize(aPIAuthorizationRequest) + JsonSerializer.Serialize(profileCode);
                await logging.WriteToLog(logError);
            }

            return true;
        }

        //CUSTOMIZE OBJECT
        private async Task<string> MakeObjectToSend(string? nextToken)
        {
            //make object to send
            AllCampaignsRequest getCampaignRequest = new AllCampaignsRequest(); //this holds your post parameters

            getCampaignRequest.nextToken = nextToken;

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(getCampaignRequest);

            return serlializedJson;
        }

        //call api
        private async Task<GetCampaignResponse> CallApiAndHandleResponse(string mediaType, string endPoint, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson)
        {
            GetCampaignResponse getValues = new GetCampaignResponse();

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

            //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
            if (responseMessage.IsSuccessStatusCode)
            {
                callSuccess = true;

                getValues = await JsonSerializer.DeserializeAsync<GetCampaignResponse>(responseMessage.Content.ReadAsStream());

            }

            return getValues;
        }
    }
}