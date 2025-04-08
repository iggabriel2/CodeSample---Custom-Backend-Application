using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class KeywordSnapshotLogic
    {
        private static readonly object ReportLock = new object();

        public async Task<bool> ProccessSnapshot(APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, string snapshotUrl)
        {
            try
            {
                List<KeywordSnapshot> keywordSnapshotList = new List<KeywordSnapshot>();
                GetKeywordsSnapshot getKeywordsSnapshot = new GetKeywordsSnapshot();
                RetrieveReportData rd = new RetrieveReportData();

                if (keepProccessing)
                {
                    keywordSnapshotList = await getKeywordsSnapshot.GetSnapshot(aPIAuthorizationRequest, profileCode, snapshotUrl);
                }

                if (keywordSnapshotList.IsNullOrEmpty() || keywordSnapshotList.Count == 0)
                {
                    keepProccessing = false;
                }

                if (keepProccessing)
                {
                    //finish code
                    keywordSnapshotList.ForEach(c => { c.ClientId = aPIAuthorizationRequest.ClientId.ToString(); });
                    keywordSnapshotList.ForEach(c => { c.CountryId = profileCode.CountryId; });
                    keywordSnapshotList.ForEach(c => { c.partitionKey = aPIAuthorizationRequest.ClientId.ToString(); });
                    keywordSnapshotList.ForEach(c => { c.id = aPIAuthorizationRequest.ClientId.ToString() + "." + profileCode.CountryId.ToString() + "." + c.keywordId.ToString(); });

                    List<string> activeKeywordIds = await rd.GetKeywordIdsWithActivity(aPIAuthorizationRequest.ClientId, profileCode.CountryId);

                    foreach(var activeId in activeKeywordIds)
                    {
                        keywordSnapshotList.Where(w => w.keywordId.ToString() == activeId).ToList().ForEach(c => { c.HasData = true; });
                    }

                    List<KeywordSnapshot> activeKeywords = keywordSnapshotList.Where(x => x.HasData).ToList();

                    //save keywords to snapshot
                    var saveSnapshotResponse = await SendKeywordsToSnapshot(activeKeywords, aPIAuthorizationRequest.ClientId);

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

        public async Task<bool> SendKeywordsToSnapshot(List<KeywordSnapshot> keywordSnapshotList, Guid clientId)
        {
            try
            {
                //make cosmos client
                Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosKeywords, "/partitionKey");

                var concurrentTasks = new List<System.Threading.Tasks.Task>();

                foreach (KeywordSnapshot item in keywordSnapshotList)
                {
                    concurrentTasks.Add(container.UpsertItemAsync<KeywordSnapshot>(item, new PartitionKey(item.partitionKey)));
                }

                await System.Threading.Tasks.Task.WhenAll(concurrentTasks);

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "SendKeywordsToSnapshot", JsonConvert.SerializeObject(keywordSnapshotList), clientId);
                return false;
            }

            return true;

        }
    }
}
