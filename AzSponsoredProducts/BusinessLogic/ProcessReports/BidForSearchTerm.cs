using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using Configuration;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class BidForSearchTerm
    {
        public async Task<decimal?> GetBidForSearchTerm(string searchTerm, ReportUser reportUser, int CountryId)
        {
            try
            {
                List<DailyKeywordDataOutput> searchTermOutput = new List<DailyKeywordDataOutput>();
                List<decimal?> bids = new List<decimal?>();

                Database database = Cosmos.cosmosInstance.GetDatabase(Cosmos.CosmosAzDb);
                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosSearchTermsDataContainer, "/partitionKey");

                //bring in keywords that had a sale today
                DateTime CreationDate = DateTime.Now.AddDays(-35);

                IReadOnlyList<FeedRange> feedRanges = await container.GetFeedRangesAsync();
                QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c where c.partitionKey = @clientId and c.dateRecord >= @startDate and c.searchTerm = @searchterm and c.Country = @country and c.clicks > 0")
                       .WithParameter("@clientId", reportUser.ClientId.ToString())
                         .WithParameter("@searchterm", searchTerm)
                          .WithParameter("@country", CountryId)
                         .WithParameter("@startDate", CreationDate.ToUniversalTime());

                using (FeedIterator<DailyKeywordDataOutput> feedIterator = container.GetItemQueryIterator<DailyKeywordDataOutput>(
                      feedRanges[0],
                      queryDefinition,
                      null,
                      new QueryRequestOptions() { }))
                {
                    // Iterate query result pages
                    while (feedIterator.HasMoreResults)
                    {
                        FeedResponse<DailyKeywordDataOutput> snapshotResponse = await feedIterator.ReadNextAsync();

                        // Iterate query results
                        foreach (var item in snapshotResponse)
                        {
                            searchTermOutput.Add(item);
                        }
                    }
                }

                decimal? highestBid = null;

                try
                {
                    bids = searchTermOutput.Select(t => (t.cost / t.clicks)).OrderByDescending(x => x).ToList();
                    highestBid = bids.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    //failed to find a bid. return null.
                }

                if (highestBid != null)
                {
                    return await GeneralStaticUtils.Round((decimal)highestBid);
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetBidForSearchTerm", searchTerm + " " + JsonConvert.SerializeObject(reportUser) + " " + CountryId.ToString(), reportUser.ClientId);
                return null;
            }
          
        }
    }
}
