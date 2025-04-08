using AdTool.Entities.AzSpApi.Keywords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Configuration;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
using AdTool.Entities.Keywords;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class GetUserDefinedKeywordsLogic
    {
        public async Task<GetUserDefinedKeywordsResponse> GetUserDefinedKeywords(GetUserDefinedKeywordsRequest request)
        {
            GetUserDefinedKeywordsResponse result = new GetUserDefinedKeywordsResponse();

            try
            {
                if (request.ClientId != Guid.Empty && request.SavedSearchId > 0)
                {
                    Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);

                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(Cosmos.CosmosUserDefinedKeywordsContainer);

                    var itemid = request.ClientId.ToString() + "." + request.SavedSearchId.ToString();
                    var partitionKey = request.ClientId.ToString();
                    using (ResponseMessage responseMessage = await container.ReadItemStreamAsync(itemid, new PartitionKey(partitionKey)))
                    {
                        if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            result.Success = true;
                            return result;
                        }
                        else if(responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };
                            var obj = await JsonSerializer.DeserializeAsync<UserDefinedKeywordsObj>(responseMessage.Content, options);

                            result.DeletedKeywords = obj.DeletedKeywords;
                            result.DeletedAsins = obj.DeletedAsins;
                            result.DeletedPrint = obj.DeletedPrint;
                            result.Success = true;
                        }
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
                result.ErrorMessage = "Failed to get keywods.";
                await ErrorLogging.LogError(ex.ToString(), "GetUserDeletedKeywords - GetUserDeletedKeywordsLogic.cs", System.Text.Json.JsonSerializer.Serialize(request), request.ClientId);
            }

            return result;
        }
    }
}
