using AdTool.AzSponsoredProducts.AmazonAPI.Keyword;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignManagement;
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
    public class UpdateProductTargetLogic
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

                string adGroupRequestEndpoint = "/sp/targets";
                string adGroupRequestMediaType = "application/vnd.spTargetingClause.v3+json";

                UpdateProductTarget updateProductApi = new UpdateProductTarget();
                var productUpdated = await updateProductApi.Update(request.CountryId, request, adGroupRequestEndpoint, adGroupRequestMediaType, auth);

                if (productUpdated == "1")
                {
                    Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                    Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosProductTargets, "/partitionKey");


                    try
                    {
                        ProductTargetSnapshot item = new ProductTargetSnapshot();
                        item.id = request.Authorization.ClientId.ToString() + "." + request.CountryId.ToString() + "." + request.CampaignId + "." + request.keywordId.ToString();
                        item.partitionKey = request.Authorization.ClientId.ToString();

                        // Read the item to see if it exists
                        ItemResponse<ProductTargetSnapshot> itemResource = await container.ReadItemAsync<ProductTargetSnapshot>(item.id, new PartitionKey(item.partitionKey));

                        var itemBody = itemResource.Resource;

                        itemBody.state = request.state;
                        itemBody.bid = request.bid;

                        // replace the item with the updated content
                        await container.ReplaceItemAsync<ProductTargetSnapshot>(itemBody, itemBody.id, new PartitionKey(itemBody.partitionKey));

                        //add a bid change history record
                        if (request.BidUpdated)
                        {
                            Microsoft.Azure.Cosmos.Container BidTrackingContainer = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosBidTrackingContainer, "/partitionKey");

                            KeywordBidTracker kyBid = new KeywordBidTracker();
                            kyBid.KeywordType = "ProductTarget";
                            kyBid.ClientId = request.Authorization.ClientId.ToString();
                            kyBid.CountryId = request.CountryId;
                            kyBid.keywordId = request.keywordId;
                            kyBid.LastUpdated = DateTime.UtcNow.Date;
                            kyBid.id = request.Authorization.ClientId.ToString() + "." + request.CountryId.ToString() + "." + request.keywordId;
                            kyBid.partitionKey = request.Authorization.ClientId.ToString();

                            await BidTrackingContainer.UpsertItemAsync<KeywordBidTracker>(kyBid, new PartitionKey(item.partitionKey));
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
                await ErrorLogging.LogError(ex.ToString(), "UpdateProductTargetLogic - Update", System.Text.Json.JsonSerializer.Serialize(request), request.Authorization.ClientId);
                simpleResponse.APIAuthorization.ErrorMessage = "UpdateProductTargetLogic failed";
                return simpleResponse;
            }

        }
    }
}
