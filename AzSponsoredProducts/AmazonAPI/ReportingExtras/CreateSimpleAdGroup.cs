using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CampaignExtra
{
    public class CreateSimpleAdGroup
    {
        public async Task<string> CreateAdGroup(int CountryId, string CampaignID, string AdGroupName, CampaignRequest request, CountrySpecificRules CountryToCreate, APIAuthorization Auth)
        {
            try
            {
                string endPoint = "sp/adGroups";
                string mediaType = "application/vnd.spAdGroup.v3+json";

                AdGroupCreation adGroupCreation = new AdGroupCreation();
                var newAdGroupId = await adGroupCreation.CreateThisAdGroup(CountryId, CampaignID, AdGroupName, request, CountryToCreate, endPoint, mediaType, Auth);

                if (newAdGroupId != "0")
                {
                    return newAdGroupId;
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreateAdGroup on CreateSimpleAdGroup";
                logError.ClientId = Auth.ClientId;
                logError.Parameters = "In order of method: " + CountryId.ToString() + " " + CampaignID + " " + AdGroupName + " " + JsonSerializer.Serialize(request) + " " + JsonSerializer.Serialize(CountryToCreate) + " " + JsonSerializer.Serialize(Auth);
                await logging.WriteToLog(logError);

                return null;
            }

        }
    }
}
