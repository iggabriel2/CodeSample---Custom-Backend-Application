using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.BusinessLogic.Utilities;
using Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class ReconcileProductsOnCosmos
    {
        public async System.Threading.Tasks.Task Reconcile(Guid ClientId)
        {
            try
            {
                List<ReconcileHistory> itemsRetrieved = new List<ReconcileHistory>();

                RetrieveReportData retrieveReportData = new RetrieveReportData();
                SaveReportData saveReportData = new SaveReportData();

                itemsRetrieved = await retrieveReportData.GetCampaignsToReconcile(ClientId);

                foreach (var item in itemsRetrieved)
                {
                    //get product name
                    ProductValueForReport productValue = new ProductValueForReport();
                    productValue = await retrieveReportData.GetProductName(item);

                    //reconcile Cosmos
                    var reconciled = await ReconcileCosmos(item, productValue);

                    if (reconciled)
                    {
                        //update item to save
                        item.Reconcile = false;

                        //save back to db
                        await saveReportData.SaveHistoryItems(item);
                    }

                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "Reconcile on ReconcileProductsOnCosmos", JsonSerializer.Serialize(ClientId), ClientId);
            }
        }

        public async Task<bool> ReconcileCosmos(ReconcileHistory reconcileHistory, ProductValueForReport productValue)
        {
            try
            {
                Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);
                

                var campaigns = await UpdateCampaignData(reconcileHistory, database, productValue);
                var keywords = await UpdateKeywordData(reconcileHistory, database, productValue);
                var searchTerms = await UpdateSearchTermData(reconcileHistory, database, productValue);

                if (campaigns && keywords && searchTerms)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "ReconcileCosmos on ReconcileProductsOnCosmos", JsonSerializer.Serialize(reconcileHistory), reconcileHistory.ClientId);
                return false;
            }
        }

        public async Task<bool> UpdateCampaignData(ReconcileHistory reconcileHistory, Database database, ProductValueForReport productValue)
        {
            try
            {
                List<DailyCampaignData> dailyCampaignData = new List<DailyCampaignData>();

                Container container = database.GetContainer(Cosmos.CosmosCampaignDataContainer);

                IReadOnlyList<FeedRange> feedRanges2 = await container.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.campaignId = @campaignId and c.Country = @Country")
                          .WithParameter("@clientId", reconcileHistory.ClientId.ToString())
                          .WithParameter("@campaignId", reconcileHistory.AzCampaignId)
                          .WithParameter("@Country", reconcileHistory.CountryId);
                using (FeedIterator<DailyCampaignData> feedIterator2 = container.GetItemQueryIterator<DailyCampaignData>(
                    feedRanges2[0],
                    queryDefinition2,
                    null,
                    new QueryRequestOptions() { }))
                {
                    // Iterate query result pages
                    while (feedIterator2.HasMoreResults)
                    {
                        FeedResponse<DailyCampaignData> snapshotResponse = await feedIterator2.ReadNextAsync();

                        // Iterate query results
                        foreach (DailyCampaignData item in snapshotResponse)
                        {
                            dailyCampaignData.Add(item);
                        }
                    }
                }

                foreach(var item in dailyCampaignData)
                {
                    item.ProductId = productValue.QAPProductId;
                    item.ProductName = productValue.AzProductName;
                    await container.ReplaceItemAsync<DailyCampaignData>(item, item.id, new PartitionKey(item.partitionKey));
                }

                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateCampaignData on ReconcileProductsOnCosmos", JsonSerializer.Serialize(reconcileHistory), reconcileHistory.ClientId);
                return false;
            }
        }

        public async Task<bool> UpdateKeywordData(ReconcileHistory reconcileHistory, Database database, ProductValueForReport productValue)
        {
            try
            {
                Container container = database.GetContainer(Cosmos.CosmosKeywordDataContainer);


                List<DailyKeywordDataOutputForKeywords> dailyData = new List<DailyKeywordDataOutputForKeywords>();

                IReadOnlyList<FeedRange> feedRanges2 = await container.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.campaignId = @campaignId and c.Country = @Country")
                          .WithParameter("@clientId", reconcileHistory.ClientId.ToString())
                          .WithParameter("@campaignId", reconcileHistory.AzCampaignId)
                          .WithParameter("@Country", reconcileHistory.CountryId);
                using (FeedIterator<DailyKeywordDataOutputForKeywords> feedIterator2 = container.GetItemQueryIterator<DailyKeywordDataOutputForKeywords>(
                    feedRanges2[0],
                    queryDefinition2,
                    null,
                    new QueryRequestOptions() { }))
                {
                    // Iterate query result pages
                    while (feedIterator2.HasMoreResults)
                    {
                        FeedResponse<DailyKeywordDataOutputForKeywords> snapshotResponse = await feedIterator2.ReadNextAsync();

                        // Iterate query results
                        foreach (DailyKeywordDataOutputForKeywords item in snapshotResponse)
                        {
                            dailyData.Add(item);
                        }
                    }
                }

                foreach (var item in dailyData)
                {
                    item.ProductId = productValue.QAPProductId;
                    item.ProductName = productValue.AzProductName;
                    await container.ReplaceItemAsync<DailyKeywordDataOutputForKeywords>(item, item.id, new PartitionKey(item.partitionKey));
                }

                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateKeywordData on ReconcileProductsOnCosmos", JsonSerializer.Serialize(reconcileHistory), reconcileHistory.ClientId);
                return false;
            }
        }

        public async Task<bool> UpdateSearchTermData(ReconcileHistory reconcileHistory, Database database, ProductValueForReport productValue)
        {
            try
            {
                Container container = database.GetContainer(Cosmos.CosmosSearchTermsDataContainer);

                List<DailyKeywordDataOutput> dailyData = new List<DailyKeywordDataOutput>();

                IReadOnlyList<FeedRange> feedRanges2 = await container.GetFeedRangesAsync();
                // Distribute feedRanges across multiple compute units and pass each one to a different iterator
                QueryDefinition queryDefinition2 = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.campaignId = @campaignId and c.Country = @Country")
                          .WithParameter("@clientId", reconcileHistory.ClientId.ToString())
                          .WithParameter("@campaignId", reconcileHistory.AzCampaignId)
                          .WithParameter("@Country", reconcileHistory.CountryId);
                using (FeedIterator<DailyKeywordDataOutput> feedIterator2 = container.GetItemQueryIterator<DailyKeywordDataOutput>(
                    feedRanges2[0],
                    queryDefinition2,
                    null,
                    new QueryRequestOptions() { }))
                {
                    // Iterate query result pages
                    while (feedIterator2.HasMoreResults)
                    {
                        FeedResponse<DailyKeywordDataOutput> snapshotResponse = await feedIterator2.ReadNextAsync();

                        // Iterate query results
                        foreach (DailyKeywordDataOutput item in snapshotResponse)
                        {
                            dailyData.Add(item);
                        }
                    }
                }

                foreach (var item in dailyData)
                {
                    item.ProductId = productValue.QAPProductId;
                    item.ProductName = productValue.AzProductName;
                    await container.ReplaceItemAsync<DailyKeywordDataOutput>(item, item.id, new PartitionKey(item.partitionKey));
                }

                return true;
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "UpdateSearchTermData on ReconcileProductsOnCosmos", JsonSerializer.Serialize(reconcileHistory), reconcileHistory.ClientId);
                return false;
            }
        }
    }
}
