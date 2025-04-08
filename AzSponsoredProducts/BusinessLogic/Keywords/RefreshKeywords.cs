using AdTool.AzSponsoredProducts.ASINAPI;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.D4API;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.D4Api;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class RefreshKeywords
    {
        public async Task<bool> RefreshExpiredKeywords()
        {
            List<SearchTermRefresh> searchTerms = new List<SearchTermRefresh>();

            GetBooksAndAsins getBooksAndAsins = new GetBooksAndAsins();
            GetKeywords getKeywords = new GetKeywords();

            RetrieveData rd = new RetrieveData();
            searchTerms = await rd.GetExpiredSearchTerms();

            KeywordRequest keywordRequest = new KeywordRequest();
            keywordRequest.Authorization.TokenExpirationTime = DateTime.Now.AddDays(3);
            keywordRequest.Authorization.ClientId = Guid.Empty;

            if (searchTerms.Count > 0)
            {
                foreach (var searchTerm in searchTerms)
                {
                    try
                    {
                        keywordRequest.SearchTerm = searchTerm.FriendlyName;
                        keywordRequest.CompressedSearchTerm = searchTerm.SearchTerm;

                        //var task1 = getBooksAndAsins.GetAsinKeywords(keywordRequest);
                        //var task2 = getKeywords.GetD4AzKeywords(keywordRequest);
                        //await System.Threading.Tasks.Task.WhenAll(task1, task2);

                        //just refreshing titles
                        var task1 = await getBooksAndAsins.GetAsinKeywords(keywordRequest);
                    }
                    catch (Exception ex)
                    {
                        await ErrorLogging.LogError(ex.ToString(), "RefreshExpiredKeywords", "none", Guid.Empty);
                    }
                }
            }

            return true;
        }
    }
}
