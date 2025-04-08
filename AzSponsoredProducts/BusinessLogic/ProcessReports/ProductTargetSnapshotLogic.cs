using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class ProductTargetSnapshotLogic
    {
        public async Task<bool> ProccessSnapshot(APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, string snapshotUrl)
        {
            try
            {
                List<ProductTargetSnapshot> productTargetSnapshotList = new List<ProductTargetSnapshot>();
                GetProductTargetSnapshot getProductTargetSnapshot = new GetProductTargetSnapshot();
                RetrieveReportData rd = new RetrieveReportData();

                if (keepProccessing)
                {
                    productTargetSnapshotList = await getProductTargetSnapshot.GetSnapshot(aPIAuthorizationRequest, profileCode, snapshotUrl);
                }

                if (productTargetSnapshotList.IsNullOrEmpty() || productTargetSnapshotList.Count == 0)
                {
                    keepProccessing = false;
                }

                if (keepProccessing)
                {
                    //finish code
                    productTargetSnapshotList.ForEach(c => { c.ClientId = aPIAuthorizationRequest.ClientId.ToString(); });
                    productTargetSnapshotList.ForEach(c => { c.CountryId = profileCode.CountryId; });
                    productTargetSnapshotList.ForEach(c => { c.partitionKey = aPIAuthorizationRequest.ClientId.ToString(); });
                    productTargetSnapshotList.ForEach(c => { c.id = aPIAuthorizationRequest.ClientId.ToString() + "." + profileCode.CountryId.ToString() + "." + c.campaignId.ToString() + "." + c.targetId.ToString(); });

                    List<string> activeProductTargetIds = await rd.GetKeywordIdsWithActivity(aPIAuthorizationRequest.ClientId, profileCode.CountryId);

                    foreach (var activeId in activeProductTargetIds)
                    {
                        productTargetSnapshotList.Where(w => w.targetId.ToString() == activeId).ToList().ForEach(c => { c.HasData = true; });
                    }

                    //save keywords to snapshot
                    var saveSnapshotResponse = await SendProductTargetsToSnapshot(productTargetSnapshotList, aPIAuthorizationRequest.ClientId);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> SendProductTargetsToSnapshot(List<ProductTargetSnapshot> productTargetSnapshotList, Guid clientId)
        {
            try
            {
                //make cosmos client
                CosmosClient cosmosInstance = new CosmosClient(Cosmos.CosmosUri, Cosmos.CosmosKey, new CosmosClientOptions() { AllowBulkExecution = true });
                Database database = await cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosProductTargets, "/partitionKey");

                var concurrentTasks = new List<System.Threading.Tasks.Task>();

                foreach (ProductTargetSnapshot item in productTargetSnapshotList)
                {
                    concurrentTasks.Add(container.UpsertItemAsync<ProductTargetSnapshot>(item, new PartitionKey(item.partitionKey)));
                }

                await System.Threading.Tasks.Task.WhenAll(concurrentTasks);

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SendProductTargetsToSnapshot", JsonConvert.SerializeObject(productTargetSnapshotList), clientId);
                return false;
            }

            return true;

        }
    }
}
