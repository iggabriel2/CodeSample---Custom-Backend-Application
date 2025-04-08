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
    public class UpdateMultipleCampaignLogic
    {
        public async Task<SimpleResponse> UpdateCampaign(CampaignUpdateMultipleRequest request)
        {


            //request currently requires the campaigns to already be idnetified and sent as part of request object. If we move forward with this, we will want to add
            //an additonal layer before this to get everything we want and break it appropriate chunks for Amazon to handle















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

                UpdateMultipleCampaigns updateCampaign = new UpdateMultipleCampaigns();
                var updateSuccess = await updateCampaign.Update(request.CountryId, request, adGroupRequestEndpoint, adGroupRequestMediaType, auth);

                if (updateSuccess == "1")
                {
                    List<CampaignUpdateDbObject> campaignUpdateDbObjects = new List<CampaignUpdateDbObject>();

                    foreach (var c in request.CampaignsToUpdate)
                    {
                        CampaignUpdateDbObject campaignUpdateDbObject = new CampaignUpdateDbObject();
                        campaignUpdateDbObject.CampaignId = c.CampaignId;
                        campaignUpdateDbObject.CountryId = request.CountryId;
                        campaignUpdateDbObject.ClientId = request.Authorization.ClientId;
                        campaignUpdateDbObject.DynamicBiddingStrategy = Convert.ToInt32(c.DynamicBiddingStrategy);
                        campaignUpdateDbObjects.Add(campaignUpdateDbObject);
                    }


                    Data.SaveData saveData = new Data.SaveData();
                    var dataSaved = await saveData.UpdateMultipleCampaigns(campaignUpdateDbObjects);


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
                await ErrorLogging.LogError(ex.ToString(), "UpdateMultipleCampaignLogic - UpdateCampaign", System.Text.Json.JsonSerializer.Serialize(request), request.Authorization.ClientId);
                response.APIAuthorization.ErrorMessage = "UpdateMultipleCampaignLogic failed";
            }

            return response;
        }
    }
}
