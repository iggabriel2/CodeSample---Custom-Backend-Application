using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Logging;
using Azure;
using Azure.Core;
using Configuration;
using System.Data.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace AdTool.AzSponsoredProducts.AmazonAPI.Tokens
{
    public class APITokenCreation
    {
       
        public async Task<APIAuthorization> ReturnRequestTokens(APIAuthorizationRequest APIAuthtorizationRequest)
        {
            APIAuthorization aPIAuthtorization = new APIAuthorization();
            aPIAuthtorization.ClientId = APIAuthtorizationRequest.ClientId;
            
            try
            {
                //check authorization object and return if it is still valid
                if (APIAuthtorizationRequest != null && !string.IsNullOrEmpty(APIAuthtorizationRequest.AccessToken) && APIAuthtorizationRequest.AccessToken != "Invalid" && APIAuthtorizationRequest.AccessToken != "Failed" && APIAuthtorizationRequest.TokenExpirationTime != null && Convert.ToDateTime(APIAuthtorizationRequest.TokenExpirationTime).AddMinutes(-10) > DateTime.Now)
                {
                    aPIAuthtorization.AccessToken = APIAuthtorizationRequest.AccessToken;
                    aPIAuthtorization.TokenExpirationTime = APIAuthtorizationRequest.TokenExpirationTime;
                    return aPIAuthtorization;
                }

                //check the db. If the token exists and is valid, use it. Otherwise, make token
                RetrieveData dr = new RetrieveData();
                AllAccessTokens? accessToken = await dr.GetAccessToken(APIAuthtorizationRequest.ClientId);

                if (accessToken != null && !string.IsNullOrEmpty(accessToken.AccessToken) && accessToken.AccessToken != "Invalid" && accessToken.AccessToken != "Failed" && accessToken.TokenExpirationTime != null && Convert.ToDateTime(accessToken.TokenExpirationTime).AddMinutes(-10) > DateTime.Now)
                {
                    aPIAuthtorization.AccessToken = accessToken.AccessToken;
                    aPIAuthtorization.TokenExpirationTime = accessToken.TokenExpirationTime;
                    //nothing to do. Token is still valid.
                    return aPIAuthtorization;
                }
                else
                {
                    //make a new token
                    APIAuthtorizationRequest.RefreshToken = accessToken.RefreshToken;
                    var response = await MakeANewToken(aPIAuthtorization, APIAuthtorizationRequest);
                    return aPIAuthtorization;
                }
            }
            catch (Exception ex)
            {
                //log
                aPIAuthtorization.ErrorMessage = "Unable to authorize.";
            }
          
            return aPIAuthtorization;
        }

        public async Task<bool> MakeANewToken(APIAuthorization aPIAuthtorization, APIAuthorizationRequest APIAuthtorizationRequest)
        {
            //make a new token
            TokenResponse token = await GetTokenValue(APIAuthtorizationRequest.RefreshToken);

            aPIAuthtorization.AccessToken = token.access_token;
            aPIAuthtorization.TokenExpirationTime = DateTime.Now.AddSeconds(token.expires_in);

            //save the new token back to the db
            AllAccessTokens saveAccessToken = new AllAccessTokens();
            saveAccessToken.AccessToken = aPIAuthtorization.AccessToken;
            saveAccessToken.TokenExpirationTime = Convert.ToDateTime(aPIAuthtorization.TokenExpirationTime);
            saveAccessToken.ClientId = aPIAuthtorization.ClientId;

            SaveData sd = new SaveData();
            await sd.UpdateAccessToken(saveAccessToken);

            return true;
        }

        public async Task<TokenResponse> GetTokenValue(string RefreshTokenValue)
        {
            TokenResponse? myResponse = new TokenResponse();

            try
            {
                RefreshTokenRequest refreshToken = new RefreshTokenRequest();
                refreshToken.refresh_token = RefreshTokenValue;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(AzApiInfo.AuthorizeAPI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("token", refreshToken);

                    if (response.IsSuccessStatusCode)
                    {
                        myResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());

                        if (myResponse != null)
                        {
                            return myResponse;
                        }
                        else
                        {
                            return null;
                        }

                    }
                    else
                    {
                        myResponse.access_token = "Invalid";
                        return myResponse;
                    }
                }
            }
            catch (Exception ex) {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetTokenValue";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "Failed to get token value. Make sure Refresh Token works correctly.";
                await logging.WriteToLog(logError);

                myResponse.access_token = "Failed";
                return myResponse;
            }
        }

    }
}
