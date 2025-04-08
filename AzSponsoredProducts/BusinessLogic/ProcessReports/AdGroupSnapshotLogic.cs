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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class AdGroupSnapshotLogic
    {
        public async Task<bool> ProccessSnapshot(APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, string snapshotUrl)
        {
            try
            {
                List<AdGroupSnapshot> adgroupSnapshotList = new List<AdGroupSnapshot>();
                GetAdGroupsSnapshot getAdgroupSnapshot = new GetAdGroupsSnapshot();
                RetrieveReportData rd = new RetrieveReportData();

                if (keepProccessing)
                {
                    adgroupSnapshotList = await getAdgroupSnapshot.GetSnapshot(aPIAuthorizationRequest, profileCode, snapshotUrl);
                }

                if (adgroupSnapshotList.IsNullOrEmpty() || adgroupSnapshotList.Count == 0)
                {
                    keepProccessing = false;
                }

                if (keepProccessing)
                {
                    //finish code
                    adgroupSnapshotList.ForEach(c => { c.ClientId = aPIAuthorizationRequest.ClientId.ToString(); });
                    adgroupSnapshotList.ForEach(c => { c.CountryId = profileCode.CountryId; });
                    adgroupSnapshotList.ForEach(c => { c.partitionKey = aPIAuthorizationRequest.ClientId.ToString(); });
                    adgroupSnapshotList.ForEach(c => { c.id = aPIAuthorizationRequest.ClientId.ToString() + "." + profileCode.CountryId.ToString() + "." + c.campaignId.ToString() + "." + c.adGroupId.ToString(); });

                    List<string> activeAGIds = await rd.GetKeywordIdsWithActivityForAG(aPIAuthorizationRequest.ClientId, profileCode.CountryId);
                    
                    foreach (var activeId in activeAGIds)
                    {
                        adgroupSnapshotList.Where(w => w.adGroupId.ToString() == activeId).ToList().ForEach(c => { c.HasData = true; });
                    }

                    //save keywords to snapshot
                    var saveSnapshotResponse = await SendAdgroupsToSnapshot(adgroupSnapshotList, aPIAuthorizationRequest.ClientId);

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

        public async Task<bool> SendAdgroupsToSnapshot(List<AdGroupSnapshot> adgroupSnapshotList, Guid clientId)
        {
            try
            {
                //make cosmos client
                CosmosClient cosmosInstance = new CosmosClient(Cosmos.CosmosUri, Cosmos.CosmosKey, new CosmosClientOptions() { AllowBulkExecution = true });
                Database database = await cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosAdGroups, "/partitionKey");

                var concurrentTasks = new List<System.Threading.Tasks.Task>();

                foreach (AdGroupSnapshot item in adgroupSnapshotList)
                {
                    concurrentTasks.Add(container.UpsertItemAsync<AdGroupSnapshot>(item, new PartitionKey(item.partitionKey)));
                }

                await System.Threading.Tasks.Task.WhenAll(concurrentTasks);

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SendAdgroupsToSnapshot", JsonConvert.SerializeObject(adgroupSnapshotList), clientId);
                return false;
            }

            return true;

        }

    }
}
