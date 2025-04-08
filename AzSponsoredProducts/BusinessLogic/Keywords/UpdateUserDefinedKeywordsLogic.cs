using AdTool.Entities.AzSpApi.Keywords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Configuration;
using AdTool.Entities.Keywords;
using AdTool.BusinessLogic.Utilities;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class UpdateUserDefinedKeywordsLogic
    {
        public async Task<UpdateUserDefinedKeywordsResponse> UpdateUserDefinedKeywords(UpdateUserDefinedKeywordsRequest request)
        {
            UpdateUserDefinedKeywordsResponse result = new UpdateUserDefinedKeywordsResponse();
            try
            {
                if (request.ClientId != Guid.Empty && request.SavedSearchId > 0)
                {
                    Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                    Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosUserDefinedKeywordsContainer, "/partitionKey");

                    if ((request.DeletedKeywords != null && request.DeletedKeywords.Count > 0) || (request.DeletedAsins != null && request.DeletedAsins.Count() > 0) || (request.DeletedPrint != null && request.DeletedPrint.Count() > 0))
                    {
                        UserDefinedKeywordsObj item = new UserDefinedKeywordsObj();
                        item.partitionKey = request.ClientId.ToString();
                        item.DeletedKeywords = request.DeletedKeywords;
                        item.DeletedAsins = request.DeletedAsins;
                        item.DeletedPrint = request.DeletedPrint;
                        item.SavedSearchId = request.SavedSearchId;
                        item.ClientId = request.ClientId;
                        item.id = request.ClientId.ToString() + "." + request.SavedSearchId.ToString();

                        var cosmosresult = await container.UpsertItemAsync<UserDefinedKeywordsObj>(item, new PartitionKey(item.partitionKey));
                        result.Success = true;
                    }
                    else
                    {
                        var itemid = request.ClientId.ToString() + "." + request.SavedSearchId.ToString();
                        var partitionKey = request.ClientId.ToString();
                        using (ResponseMessage responseMessage = await container.ReadItemStreamAsync(itemid, new PartitionKey(partitionKey)))
                        {
                            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                result.Success = true;
                                return result;
                            }
                            else
                            {
                                var cosmosDeleteResult = await container.DeleteItemAsync<UserDefinedKeywordsObj>(itemid, new PartitionKey(partitionKey));
                            }
                        }

                        result.Success = true;
                    }
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = "Data provided is invalid.";
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = "Failed to update record.";
                await ErrorLogging.LogError(ex.ToString(), "UpdateUserDefinedKeywords - UpdateUserDefinedKeywordsLogic.cs", System.Text.Json.JsonSerializer.Serialize(request), request.ClientId);
            }

            return result;
        }
    }
}
