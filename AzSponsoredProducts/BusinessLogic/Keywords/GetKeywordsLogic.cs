using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.ASINAPI;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.D4API;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.D4Api;
using AdTool.Entities.Logging;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class GetKeywordsLogic
    {
        public async Task<KeywordResponse> GetKeywords(KeywordRequest myRequest, bool WaitForBooksAndAsins = false)
        {

            KeywordResponse KeywordResponse = new KeywordResponse();

            try
            {

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                string searchTerm = regex.Replace(myRequest.SearchTerm, " ");

                myRequest.CompressedSearchTerm = searchTerm.Trim().ToLower().Replace(".", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace(" ", "");


                if (myRequest.AccountType == 1)
                {
                    //get seller search terms
                    await GetSellerSearchTerms(myRequest);
                }
                else
                {
                    //get author search terms
                    await GetAuthorSearchTerms(myRequest, WaitForBooksAndAsins);
                }

                //prepare object to send back
                RetrieveData rd = new RetrieveData();
                var keywordsInDbRaw = await rd.GetKeywordsFromDb(myRequest.CompressedSearchTerm);
                int searchTermId = await rd.GetSearchTermId(myRequest.CompressedSearchTerm);

                List<D4Keyword> keywordsInDbClean = new List<D4Keyword>();

                List<string> keywordsToExclude = await rd.GetKeywordsToExclude();

                //finish excluding here
                foreach(var keywordValue in keywordsInDbRaw)
                {
                    if (!keywordsToExclude.Any(s => keywordValue.Keyword.ToLower().Contains(s)))
                    {
                        keywordsInDbClean.Add(keywordValue);
                    }
                }


 
                KeywordResponse.APIAuthorization.ClientId = myRequest.Authorization.ClientId;
                KeywordResponse.Keywords = keywordsInDbClean;
                KeywordResponse.SearchTerm = myRequest.SearchTerm;
                KeywordResponse.SearchTermId = searchTermId;


                KeywordResponse.APIAuthorization.AccessToken = myRequest.Authorization.AccessToken;
                KeywordResponse.APIAuthorization.TokenExpirationTime = myRequest.Authorization.TokenExpirationTime;

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetKeywords", System.Text.Json.JsonSerializer.Serialize(myRequest), myRequest.Authorization.ClientId);
                KeywordResponse.APIAuthorization.ErrorMessage = "Failed to get keyword data";
            }

            return KeywordResponse;
        }

        public async System.Threading.Tasks.Task GetSellerSearchTerms(KeywordRequest myRequest)
        {
            GetSellerKeywords getSellerKeywords = new GetSellerKeywords();
            GetProductAsins getProductAsins = new GetProductAsins();

            RetrieveData rd = new RetrieveData();
            List<D4Keyword> keywordsInDb = await rd.GetKeywordsFromDb(myRequest.CompressedSearchTerm);

            LastProcessedSearchTerms lastProcessedSearchTerms = new LastProcessedSearchTerms();
            if (keywordsInDb != null && keywordsInDb.Count > 0)
            {
                lastProcessedSearchTerms = await rd.GetKeywordProcessedDate(myRequest.CompressedSearchTerm);
            }

            //make sure there are products
            List<D4Keyword> keywordProducts = new List<D4Keyword>();

            if (keywordsInDb != null)
            {
                keywordProducts = keywordsInDb.Where(x => x.TypeId == 3).ToList();
            }

            if ((lastProcessedSearchTerms.DateUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateUpdated).AddMonths(12) < DateTime.Now) && (lastProcessedSearchTerms.DateBooksUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateBooksUpdated).AddMonths(4) < DateTime.Now))
            {
                var waitForBooksAndAsinsResponse = getProductAsins.GetAsinKeywords(myRequest);
                var response = getSellerKeywords.GetD4AzKeywords(myRequest);
                await System.Threading.Tasks.Task.WhenAll(waitForBooksAndAsinsResponse, response);
            }
            else if (lastProcessedSearchTerms.DateUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateUpdated).AddMonths(12) < DateTime.Now)
            {
                var response = await getSellerKeywords.GetD4AzKeywords(myRequest);
            }
            else if (lastProcessedSearchTerms.DateBooksUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateBooksUpdated).AddMonths(4) < DateTime.Now)
            {
                var response = await getProductAsins.GetAsinKeywords(myRequest);
            }

        }


        public async System.Threading.Tasks.Task GetAuthorSearchTerms(KeywordRequest myRequest, bool WaitForBooksAndAsins = false)
        {
            GetKeywords GetKeywords = new GetKeywords();
            GetBooksAndAsins GetBooksAndAsins = new GetBooksAndAsins();

            RetrieveData rd = new RetrieveData();
            List<D4Keyword> keywordsInDb = await rd.GetKeywordsFromDb(myRequest.CompressedSearchTerm);

            LastProcessedSearchTerms lastProcessedSearchTerms = new LastProcessedSearchTerms();
            if (keywordsInDb != null && keywordsInDb.Count > 0)
            {
                lastProcessedSearchTerms = await rd.GetKeywordProcessedDate(myRequest.CompressedSearchTerm);
            }

            //make sure there are titles
            List<D4Keyword> keywordTitles = new List<D4Keyword>();

            if (keywordsInDb != null)
            {
                keywordTitles = keywordsInDb.Where(x => x.TypeId == 3).ToList();
            }

            if ((lastProcessedSearchTerms.DateUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateUpdated).AddMonths(12) < DateTime.Now) && (lastProcessedSearchTerms.DateBooksUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateBooksUpdated).AddMonths(4) < DateTime.Now))
            {
                //we are now getting and saving to db here, but not senidng back
                if (WaitForBooksAndAsins)
                {
                    var waitForBooksAndAsinsResponse = GetBooksAndAsins.GetAsinKeywords(myRequest);
                    var response = GetKeywords.GetD4AzKeywords(myRequest);
                    await System.Threading.Tasks.Task.WhenAll(waitForBooksAndAsinsResponse, response);
                }
                else
                {
                    GetBooksAndAsins.GetAsinKeywords(myRequest);
                    var response = await GetKeywords.GetD4AzKeywords(myRequest);
                }
            }
            else if (lastProcessedSearchTerms.DateUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateUpdated).AddMonths(12) < DateTime.Now)
            {
                var response = await GetKeywords.GetD4AzKeywords(myRequest);
            }
            else if (lastProcessedSearchTerms.DateBooksUpdated == null || Convert.ToDateTime(lastProcessedSearchTerms.DateBooksUpdated).AddMonths(4) < DateTime.Now)
            {
                var response = await GetBooksAndAsins.GetAsinKeywords(myRequest);
            }
        }
    }
}
