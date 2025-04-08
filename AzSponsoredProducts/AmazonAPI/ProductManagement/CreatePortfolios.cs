using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement
{
    public class CreatePortfolios
    {
        public async Task<PortfolioResponse> CreatePortfolio(CreatePortfolioRequest request)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/json";
            string endPoint = "v2/portfolios";

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request.Authorization);

            //get profile codes
            RetrieveReportData rrdCodes = new RetrieveReportData();
            request.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(request.Authorization.ClientId);

            //this holds our response - CUSTOMIZE OBJECT
            PortfolioResponse myResponse = new PortfolioResponse();

            //handle if token fails
            if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
            {
                myResponse.APIAuthorization.ErrorMessage = "Token Failed";
                return myResponse;
            }

            myResponse.APIAuthorization = auth;


            //make portfolio in all countries
            foreach (int thisCountryId in request.CountriesToCreate)
            {
                try
                {
                    //get this profile code
                    ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == thisCountryId).FirstOrDefault();

                    //make object
                    string serlializedJson = await MakeObjectToSend(request);

                    //call api here
                    AzAPIUtils azAPIUtils = new AzAPIUtils();
                    HttpResponseMessage responseMessage = new HttpResponseMessage();
                    responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

                    //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        List<AzPortfolioResponse>? getValues = await JsonSerializer.DeserializeAsync<List<AzPortfolioResponse>>(responseMessage.Content.ReadAsStream());

                        if (getValues[0].code.ToLower() == "success")
                        {
                            PortfolioSuccess portfolioSuccess = new PortfolioSuccess();
                            portfolioSuccess.Success = true;
                            portfolioSuccess.CountryId = profileCode.CountryId;
                            portfolioSuccess.PortfolioId = getValues[0].portfolioId.ToString();
                        
                            var saveToDb = await AddPortfolioToDb(getValues[0], request, profileCode);
                            portfolioSuccess.QapId = saveToDb;

                            myResponse.PortfolioSuccessByCountry.Add(portfolioSuccess);
                        }
                        else
                        {
                            AzPortfolioResponse secondAttempt = await TryToCreateAgain(request, profileCode, serlializedJson, auth);

                            if (secondAttempt.code.ToLower() == "success")
                            {
                                PortfolioSuccess portfolioSuccess = new PortfolioSuccess();
                                portfolioSuccess.Success = true;
                                portfolioSuccess.CountryId = profileCode.CountryId;
                                portfolioSuccess.PortfolioId = secondAttempt.portfolioId.ToString();

                                var saveToDb = await AddPortfolioToDb(secondAttempt, request, profileCode);
                                portfolioSuccess.QapId = saveToDb;

                                myResponse.PortfolioSuccessByCountry.Add(portfolioSuccess);
                            }
                            else
                            {
                                PortfolioSuccess portfolioSuccess = new PortfolioSuccess();
                                portfolioSuccess.Success = false;
                                portfolioSuccess.CountryId = profileCode.CountryId;
                                portfolioSuccess.PortfolioId = secondAttempt.portfolioId.ToString();
                                myResponse.PortfolioSuccessByCountry.Add(portfolioSuccess);
                            }
                        }
                    }
                    else
                    {
                        AzPortfolioResponse secondAttempt = await TryToCreateAgain(request, profileCode, serlializedJson, auth);


                        if (secondAttempt.code.ToLower() == "success")
                        {
                            PortfolioSuccess portfolioSuccess = new PortfolioSuccess();
                            portfolioSuccess.Success = true;
                            portfolioSuccess.CountryId = profileCode.CountryId;
                            portfolioSuccess.PortfolioId = secondAttempt.portfolioId.ToString();

                            var saveToDb = await AddPortfolioToDb(secondAttempt, request, profileCode);
                            portfolioSuccess.QapId = saveToDb;

                            myResponse.PortfolioSuccessByCountry.Add(portfolioSuccess);
                        }
                        else
                        {
                            PortfolioSuccess portfolioSuccess = new PortfolioSuccess();
                            portfolioSuccess.Success = false;
                            portfolioSuccess.CountryId = profileCode.CountryId;
                            portfolioSuccess.PortfolioId = secondAttempt.portfolioId.ToString();
                            myResponse.PortfolioSuccessByCountry.Add(portfolioSuccess);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = ex.ToString();
                    logError.FailureMethod = "CreatePortfolio";
                    logError.ClientId = request.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(request);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed on CreatePortfolios";
                    return myResponse;
                }
            }

            return myResponse;
        }

        public async Task<AzPortfolioResponse> TryToCreateAgain(CreatePortfolioRequest request, ClientProfileCodes profileCode, string requestJson, APIAuthorization auth)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/json";
            string endPoint = "v2/portfolios";

            //see if it exists and try again
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessageForGet = new HttpResponseMessage();
            responseMessageForGet = await azAPIUtils.CallAmazonGetApi(endPoint, mediaType, auth, profileCode);

            //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
            if (responseMessageForGet.StatusCode.ToString() == "OK")
            {
                List<AzGetPortfolioResponse> getValues = await JsonSerializer.DeserializeAsync<List<AzGetPortfolioResponse>>(responseMessageForGet.Content.ReadAsStream());

                List<AzGetPortfolioResponse> azPortfolioResponse = getValues.Where(x => x.name.Equals(request.PortfolioName)).ToList();

                if (azPortfolioResponse.Count > 0)
                {
                    AzPortfolioResponse myAzSearchResponse = new AzPortfolioResponse();
                    myAzSearchResponse.portfolioId = azPortfolioResponse[0].portfolioId;
                    myAzSearchResponse.code = "success";
                    return myAzSearchResponse;
                }
                else
                {
                    //make portfolio again
                    HttpResponseMessage responseMessage = new HttpResponseMessage();
                    responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, requestJson);

                    List<AzPortfolioResponse> getValuesCreate = new List<AzPortfolioResponse>();

                    //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        getValuesCreate = await JsonSerializer.DeserializeAsync<List<AzPortfolioResponse>>(responseMessage.Content.ReadAsStream());

                        return getValuesCreate[0];
                    }
                    else
                    {
                        getValuesCreate[0].code = "error";
                        return getValuesCreate[0];
                    }
                }
            }
            else 
            {
                AzPortfolioResponse getValuesCreate = new AzPortfolioResponse();
                getValuesCreate.code = "error";
                return getValuesCreate;
            }


        }

        //CUSTOMIZE OBJECT
        public async Task<string> MakeObjectToSend(CreatePortfolioRequest portfolioRequestValues)
        {
            //make object to send
            List<AzPortfolioRequest> azPortfolioRequests = new List<AzPortfolioRequest>();

            AzPortfolioRequest portfolioRequest = new AzPortfolioRequest(); //this holds your post parameters
            portfolioRequest.name = portfolioRequestValues.PortfolioName;
            portfolioRequest.state = "enabled";
            azPortfolioRequests.Add(portfolioRequest);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(azPortfolioRequests);

            return serlializedJson;
        }

        public async Task<int> AddPortfolioToDb(AzPortfolioResponse azPortfolioResponse, CreatePortfolioRequest request, ClientProfileCodes profileCode)
        {
            AzPortfolio portfolio = new AzPortfolio();
            portfolio.AZPortfolioId = azPortfolioResponse.portfolioId.ToString();
            portfolio.ClientId = request.Authorization.ClientId;
            portfolio.PortfolioName = request.PortfolioName;
            portfolio.CountryId = profileCode.CountryId;
            portfolio.Active = true;

            SaveData sd = new SaveData();
            int id = await sd.SavePortfolios(portfolio);

            return id;

        }
    }
}
