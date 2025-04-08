using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Update;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.BusinessLogic.CampaignsAdGroups
{
    public class UpdateCampaignLogic
    {
        public async Task<SimpleResponse> UpdateCampaign(CampaignUpdateRequest request)
        {
            SimpleResponse response = new SimpleResponse();

            try
            {
                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request.Authorization);


                //get profile codes
                RetrieveReportData rrdCodes = new RetrieveReportData();
                request.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(request.Authorization.ClientId);

                //handle if token fails
                if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
                {
                    response.APIAuthorization.AccessToken = "";
                    response.APIAuthorization.ErrorMessage = "Token Failed";
                    return response;
                }
                else
                {
                    response.APIAuthorization = auth;
                }

                request.Authorization.AccessToken = auth.AccessToken;
                request.Authorization.TokenExpirationTime = auth.TokenExpirationTime;

                string adGroupRequestEndpoint = "/sp/campaigns";
                string adGroupRequestMediaType = "application/vnd.spCampaign.v3+json";

                UpdateCampaign updateCampaign = new UpdateCampaign();
                var updateSuccess = await updateCampaign.Update(request.CountryId, request, adGroupRequestEndpoint, adGroupRequestMediaType, auth);

                if (updateSuccess == "1")
                {



                    //update database
                    CampaignUpdateDbObject campaignUpdateDbObject = new CampaignUpdateDbObject();
                    campaignUpdateDbObject.CampaignId = request.CampaignId;
                    campaignUpdateDbObject.CampaignName = request.CampaignName;
                    campaignUpdateDbObject.CountryId = request.CountryId;
                    campaignUpdateDbObject.ClientId = request.Authorization.ClientId;
                    campaignUpdateDbObject.State = request.state;
                    campaignUpdateDbObject.Budget = request.Budget;
                    
                    if (request.state == "ENABLED")
                    {
                        campaignUpdateDbObject.Active = true;
                    }
                    else
                    {
                        campaignUpdateDbObject.Active = false;
                    }

                    campaignUpdateDbObject.DynamicBiddingStrategy = Convert.ToInt32(request.DynamicBiddingStrategy);

                    try
                    {
                        Data.SaveData saveData = new Data.SaveData();
                        var dataSaved = await saveData.UpdateCampaign(campaignUpdateDbObject);
                    }
                    catch(Exception ex)
                    {
                        response.APIAuthorization.ErrorMessage = "Campaign Updated in Amazon, but failed to save to db. Will be updated tonight.";
                    }


                    CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = request.CountryId;
                    countrySucces.Success = true;

                    response.CountrySuccess.Add(countrySucces);
                    return response;
                }
                else
                {
                    CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = request.CountryId;
                    countrySucces.Success = false;

                    response.CountrySuccess.Add(countrySucces);
                    return response;
                }

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateCampaignLogic - UpdateCampaign", System.Text.Json.JsonSerializer.Serialize(request), request.Authorization.ClientId);
                response.APIAuthorization.ErrorMessage = "UpdateCampaignLogic failed";
            }

            return response;
        }
    }
}
