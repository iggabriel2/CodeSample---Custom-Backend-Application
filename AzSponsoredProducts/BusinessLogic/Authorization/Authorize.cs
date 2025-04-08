using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.AmazonAPI.Profiles;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Authorization
{
    public class Authorize
    {
        public async Task<OriginalAPIAuthorizationResponse> GetOriginalAuthorization(OriginalApiAuthorizationRequest originalApiRequest)
        {
            GetOriginalToken getOriginalToken = new GetOriginalToken();
            OriginalAPIAuthorizationResponse originalResponse = new OriginalAPIAuthorizationResponse();
            TokenResponse tokenResponse = new TokenResponse();

            try
            {
                tokenResponse = await getOriginalToken.GetTokenValue(originalApiRequest.ClientCode);

                //if we get an error, clear the token and try once more - SEE IF token was made
                if (string.IsNullOrEmpty(tokenResponse.access_token))
                {
                    tokenResponse = await getOriginalToken.GetTokenValue(originalApiRequest.ClientCode);
                }

                //if token wasn't made, return failure
                if (string.IsNullOrEmpty(tokenResponse.access_token) || tokenResponse.access_token.ToLower() == "invalid")
                {
                    originalResponse.ErrorMessage = "No token acquired";
                    return originalResponse;
                }

                originalResponse.AccessToken = tokenResponse.access_token;
                originalResponse.RefreshToken = tokenResponse.refresh_token;
                originalResponse.TokenExpirationTime = DateTime.Now.AddSeconds(tokenResponse.expires_in);

                //get profiles
                APIAuthorizationRequest aPIAuthorizationRequest = new APIAuthorizationRequest();
                aPIAuthorizationRequest.AccessToken = originalResponse.AccessToken;
                aPIAuthorizationRequest.RefreshToken = originalResponse.RefreshToken;
                aPIAuthorizationRequest.AppUserId = originalApiRequest.AppUserId;
                aPIAuthorizationRequest.TokenExpirationTime = originalResponse.TokenExpirationTime;
                aPIAuthorizationRequest.ClientId = originalApiRequest.ClientId;


                //this will check all Amazon api endpoints for profiles. We do not need to change it in the future if we add more countries.
                GetProfiles getProfiles = new GetProfiles();
                var rawProfileCodes = await getProfiles.GetProfileList(aPIAuthorizationRequest);


                RetrieveData rd = new RetrieveData();
                List<AzApiCountries> countries = await rd.GetCountries();

                //only keep profile codes in country table
                foreach (var country in countries)
                {
                    var countryExists = rawProfileCodes.Where(x => x.CountryId == country.Id).FirstOrDefault();
                    if (countryExists != null)
                    {
                        originalResponse.ClientProfileCodes.Add(countryExists);
                    }
                }



                aPIAuthorizationRequest.ClientProfileCodes = originalResponse.ClientProfileCodes;

                if (originalResponse.ClientProfileCodes.Count < 1)
                {
                    originalResponse.ErrorMessage = "Not Authorized In Any Market";
                    return originalResponse;
                }

                //save to the db
                SaveData saveData = new SaveData();
                Guid thisClientId = await saveData.CreateRefreshTokenAndProfileCodes(aPIAuthorizationRequest);

                originalResponse.ClientId = thisClientId;

                ProcessReportsLogic processReportsLogic = new ProcessReportsLogic();
                processReportsLogic.ProcessReportsLogicNow(thisClientId);

                return originalResponse;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "Authorize.cs";
                logError.ClientId = Guid.Empty;
                logError.Parameters = JsonSerializer.Serialize(originalApiRequest);
                await logging.WriteToLog(logError);

                originalResponse.ErrorMessage = "Failed to Authorize";
                return originalResponse;
            }
        }
    }
}
