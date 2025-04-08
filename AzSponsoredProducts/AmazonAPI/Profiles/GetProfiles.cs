using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using Azure;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.Profiles
{
    public class GetProfiles
    {
        public async Task<List<ClientProfileCodes>> GetProfileList(APIAuthorizationRequest authorization)
        {
            string mediaType = "application/json";
            string endPoint = "v2/profiles";

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(authorization);

            //profile codes resoponse
            List<ClientProfileCodes> myResponse = new List<ClientProfileCodes>();

            //empty profile codes to authorize
            List<AzApiCountries> countries = new List<AzApiCountries>();
            RetrieveData rd = new RetrieveData();
            countries = await rd.GetCountries();

            //this will check all Amazon api endpoints for profiles. We do not need to change it in the future if we add more countries.
            List<int> apisToCheck = new List<int>();
            for(int i = 1; i < 4; i++)
            {
                apisToCheck.Add(i);
            }
         
            //handle if token fails
            if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
            {
                return myResponse;
            }

            foreach (int apiToCheck in apisToCheck)
            {
                try
                {
                    //call api here
                    AzAPIUtils azAPIUtils = new AzAPIUtils();
                    HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonApiProfilesOnly(endPoint, mediaType, auth, apiToCheck);

                    //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        List<ProfileCodes>? getValues = await JsonSerializer.DeserializeAsync<List<ProfileCodes>>(responseMessage.Content.ReadAsStream());

                        foreach (var value in getValues)
                        {
                            if (value.accountInfo.validPaymentMethod == true)
                            {
                                AzApiCountries thisCountry = new AzApiCountries();
                                thisCountry = countries.Where(x => x.ShortName == value.countryCode).FirstOrDefault();

                                if (thisCountry != null && !string.IsNullOrEmpty(thisCountry.Country))
                                {
                                    ClientProfileCodes clientProfileCode = new ClientProfileCodes();
                                    clientProfileCode.ProfileCode = value.profileId.ToString();
                                    clientProfileCode.CountryId = thisCountry.Id;
                                    clientProfileCode.TimeZone = value.timezone;
                                    myResponse.Add(clientProfileCode);

                                }
                            }
                        }
                    }
                    else
                    {
                        //nohting to do. this call failed
                    }

                }
                catch (Exception ex)
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = ex.ToString();
                    logError.FailureMethod = "GetProfileList";
                    logError.ClientId = authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(authorization);
                    await logging.WriteToLog(logError);

                    return myResponse;
                }
            }

            return myResponse;
        }
    }
}
