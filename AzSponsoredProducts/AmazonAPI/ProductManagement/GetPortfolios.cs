using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.ProductManagement;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement
{
    public class GetPortfolios
    {
        public async Task<PortfolioListResponse> GetPortfolioInfo(PortfolioRequest portfolioRequestValues)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/json";
            string endPoint = "v2/portfolios";

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(portfolioRequestValues.Authorization);

            //get profile codes
            RetrieveReportData rrdCodes = new RetrieveReportData();
            portfolioRequestValues.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(portfolioRequestValues.Authorization.ClientId);

            //handle if token fails
            if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
            {
                PortfolioListResponse thisResponse = new PortfolioListResponse();
                thisResponse.APIAuthorization.ErrorMessage = "Token Failed";
                return thisResponse;
            }


            //this holds our response
            PortfolioListResponse portfolioListResponse = new PortfolioListResponse();
            portfolioListResponse.APIAuthorization = auth;

            //check each country
            foreach (ClientProfileCodes profileCode in portfolioRequestValues.Authorization.ClientProfileCodes)
            {
                //this hold the object we are saving to the database
                List<AzPortfolio> localPortfolios = new List<AzPortfolio>();

                try
                {
                    //call api here
                    AzAPIUtils azAPIUtils = new AzAPIUtils();
                    HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonGetApi(endPoint, mediaType, auth, profileCode);

                    //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        List<AzGetPortfolioResponse> getValues = await JsonSerializer.DeserializeAsync<List<AzGetPortfolioResponse>>(responseMessage.Content.ReadAsStream());

                        foreach(var portfolioItem in getValues)
                        {
                            AzPortfolio portfolio = new AzPortfolio();
                            portfolio.AZPortfolioId = portfolioItem.portfolioId.ToString();
                            portfolio.ClientId = portfolioRequestValues.Authorization.ClientId;
                            portfolio.PortfolioName = portfolioItem.name;
                            portfolio.CountryId = profileCode.CountryId;

                            if (portfolioItem.state.ToLower() == "enabled")
                            {
                                portfolio.Active = true;
                            }
                            else
                            {
                                portfolio.Active = false;
                            }

                            //if they are active, we want to return them to the front end
                            PortfolioList portfolioList = new PortfolioList();

                            SaveData sd = new SaveData();
                            var dbId = await sd.SavePortfolios(portfolio);

                            if (portfolioItem.state.ToLower() == "enabled")
                            {
                                portfolioList.QapId = dbId;
                                portfolioList.CountryId = profileCode.CountryId;
                                portfolioList.AzPortfolioId = portfolioItem.portfolioId.ToString();
                                portfolioList.PortfolioName = portfolioItem.name;
                                portfolioListResponse.Portfolios.Add(portfolioList);
                            }
                        }


                    }
                    else
                    {

                        portfolioListResponse.APIAuthorization.ErrorMessage = "Failed on GetPortfolioInfo";
                        return portfolioListResponse;
                    }
                }
                catch (Exception ex)
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = ex.ToString();
                    logError.FailureMethod = "GetPortfolioInfo";
                    logError.ClientId = portfolioRequestValues.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(portfolioRequestValues);
                    await logging.WriteToLog(logError);

                    CountrySuccess countrySuccess = new CountrySuccess();
                    countrySuccess.CountryId = profileCode.CountryId;
                    countrySuccess.Success = false;

                    portfolioListResponse.APIAuthorization.ErrorMessage = "Failed on GetPortfolioInfo for at least one country";
                }
            }

            return portfolioListResponse;
        }

    }
}
