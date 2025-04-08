using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.Logging;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.Tokens
{
    public class GetOriginalToken
    {
        public async Task<TokenResponse> GetTokenValue(string CodeValue)
        {
            TokenResponse? myResponse = new TokenResponse();

            try
            {
                //make append query
                string parameters = "?grant_type=authorization_code&code=" + CodeValue  + "&redirect_uri=https://authorize.faktoriq.com/AzConnection/AzSpRegistrationReturn&client_id=" + AzApiInfo.ClientId + "&client_secret=" + AzApiInfo.ClientSecret;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(AzApiInfo.AuthorizeAPI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsync("token" + parameters, null);

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
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetTokenValue";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "Failed to get original token value. Make sure Original Token works correctly.";
                await logging.WriteToLog(logError);

                myResponse.access_token = "Failed";
                return myResponse;
            }
        }
    }
}
