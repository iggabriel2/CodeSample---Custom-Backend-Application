using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.AmazonAPI.Profiles;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Authorization
{
    public class RecheckAllCountries
    {
        public async Task<CountryAuthorizationUpdateResponse> RecheckCountries(CountryAuthorizationUpdateRequest recheckCountriesRequest)
        {
            CountryAuthorizationUpdateResponse myResponse = new CountryAuthorizationUpdateResponse();

            try
            {
                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(recheckCountriesRequest.Authorization);

                myResponse.APIAuthorization = auth;

                recheckCountriesRequest.Authorization.AccessToken = auth.AccessToken;
                recheckCountriesRequest.Authorization.TokenExpirationTime = auth.TokenExpirationTime;

                //this will check all Amazon api endpoints for profiles. We do not need to change it in the future if we add more countries.
                GetProfiles getProfiles = new GetProfiles();
                List<ClientProfileCodes> rawProfileCodes = await getProfiles.GetProfileList(recheckCountriesRequest.Authorization);

                RetrieveData rd = new RetrieveData();
                List<AzApiCountries> countries = await rd.GetCountries();

                //only keep profile codes in country table
                foreach (var country in countries)
                {
                    var countryExists = rawProfileCodes.Where(x => x.CountryId == country.Id).FirstOrDefault();
                    if (countryExists != null)
                    {
                        myResponse.ClientProfileCodes.Add(countryExists);
                    }
                }
                recheckCountriesRequest.Authorization.ClientProfileCodes = myResponse.ClientProfileCodes;

                if (myResponse.ClientProfileCodes.Count < 1)
                {
                    myResponse.APIAuthorization.ErrorMessage = "Not Authorized In Any Market";
                    return myResponse;
                }

                //save to the db
                SaveData saveData = new SaveData();
                await saveData.RecreateProfileCodes(recheckCountriesRequest.Authorization);

                return myResponse;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "RecheckCountries";
                logError.ClientId = recheckCountriesRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(recheckCountriesRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to recheck countries.";
                return myResponse;
            }
        }
    }
}
