using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using Azure;
using Azure.Core;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Get
{
    public class GetCampaign
    {
        bool callSuccess = false;

        public async Task<SimpleResponse> GetCampaignName(CampaignNameRequest campaignNameRequest)
        {
            //this holds our response
            SimpleResponse myResponse = new SimpleResponse();

            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/vnd.spCampaign.v3+json";
            string endPoint = "sp/campaigns/list";
 

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(campaignNameRequest.Authorization);

            //get profile codes
            RetrieveReportData rrdCodes = new RetrieveReportData();
            campaignNameRequest.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(campaignNameRequest.Authorization.ClientId);

            //handle if token fails
            if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
            {
                SimpleResponse simpleResponse = new SimpleResponse();
                simpleResponse.APIAuthorization.ErrorMessage = "Token Failed";
                return simpleResponse;
            }


           
            //check each country
            foreach (int requestedCountry in campaignNameRequest.RequestedCountries)
            {
                //get clientProfileCode so I can make sure user is authorized
                ClientProfileCodes cpCode = new ClientProfileCodes();
                cpCode = campaignNameRequest.Authorization.ClientProfileCodes.Where(x => x.CountryId == requestedCountry).FirstOrDefault();

                if (cpCode != null && cpCode.CountryId != 0)
                {

                    try
                    {
                        //make object
                        string serlializedJson = await MakeObjectToSend(campaignNameRequest.CampaignName);

                        ClientProfileCodes profileCode = new ClientProfileCodes();
                        profileCode = campaignNameRequest.Authorization.ClientProfileCodes.Where(x => x.CountryId == requestedCountry).FirstOrDefault();

                        //call api here
                        myResponse = await CallApiAndHandleResponse(campaignNameRequest, requestedCountry, mediaType, endPoint, auth, profileCode, serlializedJson);

                        //call again if it fails
                        if (callSuccess == false)
                        {
                            //on save, we would check for the item here before remaking it. Since this is just a get, we can keep going.
                            myResponse = await CallApiAndHandleResponse(campaignNameRequest, requestedCountry, mediaType, endPoint, auth, profileCode, serlializedJson);
                        }

                        //mark the failure and return if it fails twice
                        if (callSuccess == false)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = "Failed to get country info for Campaign name";
                            logError.FailureMethod = "GetCampaignName";
                            logError.ClientId = campaignNameRequest.Authorization.ClientId;
                            logError.Parameters = JsonSerializer.Serialize(campaignNameRequest);
                            await logging.WriteToLog(logError);

                            CountrySuccess countrySuccess = new CountrySuccess();
                            countrySuccess.CountryId = requestedCountry;
                            countrySuccess.Success = false;

                            myResponse.APIAuthorization.ErrorMessage = "Failed on GetCampaignName for at least one country";
                            myResponse.CountrySuccess.Add(countrySuccess);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging logging = new Logging();
                        LogError logError = new LogError();
                        logError.ErrorMessage = ex.ToString();
                        logError.FailureMethod = "GetCampaignName";
                        logError.ClientId = campaignNameRequest.Authorization.ClientId;
                        logError.Parameters = JsonSerializer.Serialize(campaignNameRequest);
                        await logging.WriteToLog(logError);

                        CountrySuccess countrySuccess = new CountrySuccess();
                        countrySuccess.CountryId = requestedCountry;
                        countrySuccess.Success = false;

                        myResponse.APIAuthorization.ErrorMessage = "Failed on GetCampaignName for at least one country";
                        myResponse.CountrySuccess.Add(countrySuccess);
                    }
                }
               
            }

            //this will be inside the product loop
            //campaignRequest.CampaignName = workingCampaignName;

            return myResponse;
        }

        //CUSTOMIZE OBJECT
        private async Task<string> MakeObjectToSend(string CampaignNameRequested)
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

        //call api
        private async Task<SimpleResponse> CallApiAndHandleResponse(CampaignNameRequest campaignNameRequest, int requestedCountry, string mediaType, string endPoint, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson)
        {
            SimpleResponse myResponse = new SimpleResponse();


            //assign authorization to response
            myResponse.APIAuthorization = auth;


            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

            //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
            if (responseMessage.IsSuccessStatusCode)
            {
                callSuccess = true;

                GetCampaignResponse getValues = await JsonSerializer.DeserializeAsync<GetCampaignResponse>(responseMessage.Content.ReadAsStream());

                if (getValues.totalResults > 0)
                {
                    Campaign campaignPresent = new Campaign();
                    campaignPresent = getValues.campaigns.Where(x => x.name == campaignNameRequest.CampaignName).FirstOrDefault();

                    if (!string.IsNullOrEmpty(campaignPresent.name))
                    {
                        CountrySuccess countrySuccess = new CountrySuccess();
                        countrySuccess.CountryId = requestedCountry;
                        countrySuccess.Success = false;

                        myResponse.CountrySuccess.Add(countrySuccess);
                    }
                    else
                    {
                        CountrySuccess countrySuccess = new CountrySuccess();
                        countrySuccess.CountryId = requestedCountry;
                        countrySuccess.Success = true;

                        myResponse.CountrySuccess.Add(countrySuccess);
                    }
                }
                else
                {
                    CountrySuccess countrySuccess = new CountrySuccess();
                    countrySuccess.CountryId = requestedCountry;
                    countrySuccess.Success = true;

                    myResponse.CountrySuccess.Add(countrySuccess);
                }
            }
            else
            {
                callSuccess = false;
            }
            return myResponse;
        }
    }
}
