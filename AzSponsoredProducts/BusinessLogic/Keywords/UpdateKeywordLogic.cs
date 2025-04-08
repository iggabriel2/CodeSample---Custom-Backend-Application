using AdTool.AzSponsoredProducts.AmazonAPI.AdGroups;
using AdTool.AzSponsoredProducts.AmazonAPI.Keyword;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignManagement;
using AdTool.Entities.CampaignsAdGroups;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class UpdateKeywordLogic
    {
        public async Task<SimpleResponse> Update(KeywordChangeRequest request)
        {
            SimpleResponse simpleResponse = new SimpleResponse();

            try
            {
                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request.Authorization);


                //get profile codes
                RetrieveReportData rrdCodes = new RetrieveReportData();
                request.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(request.Authorization.ClientId);

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

                request.Authorization.AccessToken = auth.AccessToken;
                request.Authorization.TokenExpirationTime = auth.TokenExpirationTime;

                string adGroupRequestEndpoint = "/sp/keywords";
                string adGroupRequestMediaType = "application/vnd.spKeyword.v3+json";

                UpdateKeyword updateKeywordApi = new UpdateKeyword();
                var keywordUpdated = await updateKeywordApi.Update(request.CountryId, request, adGroupRequestEndpoint, adGroupRequestMediaType, auth);
                
                if (keywordUpdated == "1")
                {
                    //we are no longer saving all keywords in Cosmos
                    Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                    //Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosKeywords, "/partitionKey");

                    try
                    {
                        //    KeywordSnapshot item = new KeywordSnapshot();
                        //    item.id = request.Authorization.ClientId.ToString() + "." + request.CountryId.ToString() + "." + request.keywordId.ToString();
                        //    item.partitionKey = request.Authorization.ClientId.ToString();

                        //    // Read the item to see if it exists
                        //    ItemResponse<KeywordSnapshot> itemResource = await container.ReadItemAsync<KeywordSnapshot>(item.id, new PartitionKey(item.partitionKey));

                        //    var itemBody = itemResource.Resource;

                        //    itemBody.state = request.state;
                        //    itemBody.bid = request.bid;

                        //    // replace the item with the updated content
                        //    await container.ReplaceItemAsync<KeywordSnapshot>(itemBody, itemBody.id, new PartitionKey(itemBody.partitionKey));

                        //add a bid change history record
                        if (request.BidUpdated)
                        {
                        Microsoft.Azure.Cosmos.Container BidTrackingContainer = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosBidTrackingContainer, "/partitionKey");

                        KeywordBidTracker kyBid = new KeywordBidTracker();
                        kyBid.KeywordType = "Keyword";
                        kyBid.ClientId = request.Authorization.ClientId.ToString();
                        kyBid.CountryId = request.CountryId;
                        kyBid.keywordId = request.keywordId;
                        kyBid.LastUpdated = DateTime.UtcNow.Date;
                        kyBid.id = request.Authorization.ClientId.ToString() + "." + request.CountryId.ToString() + "." + request.keywordId;
                        kyBid.partitionKey = request.Authorization.ClientId.ToString();

                        await BidTrackingContainer.UpsertItemAsync<KeywordBidTracker>(kyBid, new PartitionKey(kyBid.partitionKey));
                        }


                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        //nothing to do. Record doesn't exist on Cosmos, so it doesn't need to be updated.
                    }


            CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = request.CountryId;
                    countrySucces.Success = true;

                    simpleResponse.CountrySuccess.Add(countrySucces);
                    return simpleResponse;
                }
                else
                {
                    CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = request.CountryId;
                    countrySucces.Success = false;

                    simpleResponse.CountrySuccess.Add(countrySucces);
                    return simpleResponse;
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateKeywordLogic - Update", System.Text.Json.JsonSerializer.Serialize(request), request.Authorization.ClientId);
                simpleResponse.APIAuthorization.ErrorMessage = "UpdateKeywordLogic failed";
                return simpleResponse;
            }

        }
    }
}
