using AdTool.AzSponsoredProducts.AmazonAPI.AdGroups;
using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.AmazonAPI.ExtraKeywordPromoManagement;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.CampaignsAdGroups;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.CampaignsAdGroups
{
    public class UpdateAdGroupsLogic
    {
        public async Task<SimpleResponse> Update(UpdateAdGroupRequest adGroupRequest)
        {
            SimpleResponse simpleResponse = new SimpleResponse();

            try
            {
                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(adGroupRequest.Authorization);


                //get profile codes
                RetrieveReportData rrdCodes = new RetrieveReportData();
                adGroupRequest.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(adGroupRequest.Authorization.ClientId);

                //handle if token fails
                if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
                {
                    simpleResponse.APIAuthorization.AccessToken = "";
                    simpleResponse.APIAuthorization.ErrorMessage = "Token Failed";
                    return simpleResponse;
                }
                else
                {
                    simpleResponse.APIAuthorization = auth;
                }

                adGroupRequest.Authorization.AccessToken = auth.AccessToken;
                adGroupRequest.Authorization.TokenExpirationTime = auth.TokenExpirationTime;

                string adGroupRequestEndpoint = "/sp/adGroups";
                string adGroupRequestMediaType = "application/vnd.spAdGroup.v3+json";

                UpdateAzAdGroup updateAzAdGroup = new UpdateAzAdGroup();
                var adGroupAdded = await updateAzAdGroup.Update(adGroupRequest.CountryId, adGroupRequest, adGroupRequestEndpoint, adGroupRequestMediaType, auth);

                if (adGroupAdded == "1")
                {
                    //upsert for adgroup
                    Entities.CampaignsAdGroups.AdGroupSnapshot item = new Entities.CampaignsAdGroups.AdGroupSnapshot();
                    item.adGroupId = Convert.ToInt64(adGroupRequest.adGroupId);
                    item.name = adGroupRequest.name;
                    item.campaignId = Convert.ToInt64(adGroupRequest.campaignId);
                    item.defaultBid = adGroupRequest.defaultBid;
                    item.state = adGroupRequest.state;
                    item.ClientId = adGroupRequest.Authorization.ClientId.ToString();
                    item.CountryId = adGroupRequest.CountryId;
                    item.partitionKey = adGroupRequest.Authorization.ClientId.ToString();
                    item.id = adGroupRequest.Authorization.ClientId.ToString() + "." + adGroupRequest.CountryId.ToString() + "." + adGroupRequest.campaignId.ToString() + "." + adGroupRequest.adGroupId.ToString();

                    Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                    Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosAdGroups, "/partitionKey");

                    await container.UpsertItemAsync<Entities.CampaignsAdGroups.AdGroupSnapshot>(item, new PartitionKey(item.partitionKey));


                    
                    CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = adGroupRequest.CountryId;
                    countrySucces.Success = true;

                    simpleResponse.CountrySuccess.Add(countrySucces);
                    return simpleResponse;
                }
                else
                {
                    CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = adGroupRequest.CountryId;
                    countrySucces.Success = false;

                    simpleResponse.CountrySuccess.Add(countrySucces);
                    return simpleResponse;
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateAdGroupsLogic - Update", System.Text.Json.JsonSerializer.Serialize(adGroupRequest), adGroupRequest.Authorization.ClientId);
                simpleResponse.APIAuthorization.ErrorMessage = "UpdateAdGroupsLogic failed";
                return simpleResponse;
            }

        }
    }
}
