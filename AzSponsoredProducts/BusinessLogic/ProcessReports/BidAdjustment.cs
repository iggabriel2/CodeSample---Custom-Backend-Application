using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using Configuration;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class BidAdjustment
    {
        private CosmosClient cosmosClient;

        public async System.Threading.Tasks.Task<List<SaveKeywordHistory>> AdjustBid(ReportUser reportUser, List<PromoNegativeRules> promoNegativeRules)
        {
            List<SaveKeywordHistory> allItems = new List<SaveKeywordHistory>();

            try
            {
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(reportUser.aPIAuthorizationRequest);

                //get profile codes
                RetrieveReportData rrdCodes = new RetrieveReportData();
                List<ClientProfileCodes> profileCodes = await rrdCodes.GetProfileCodes(reportUser.ClientId);

                //handle if token fails
                if (auth.AccessToken != "Invalid" && auth.AccessToken != "Failed")
                {
                    BidAdjustmentKeywords keywords = new BidAdjustmentKeywords();
                    BidAdjustmentProductTargets products = new BidAdjustmentProductTargets();
                    List<SaveKeywordHistory> keywordItems = await keywords.AdjustBid(reportUser, promoNegativeRules, auth, profileCodes);
                    List<SaveKeywordHistory> productItems = await products.AdjustBid(reportUser, promoNegativeRules, auth, profileCodes);

                    if (keywordItems != null && keywordItems.Count > 0)
                        allItems = allItems.Union(keywordItems).ToList();

                    if (productItems != null && productItems.Count > 0)
                        allItems = allItems.Union(productItems).ToList();
                }
                else
                {
                    await ErrorLogging.LogError("Auth token failed to create", "BidAdjustment - AdjustBid", JsonConvert.SerializeObject(reportUser) + " " + JsonConvert.SerializeObject(promoNegativeRules), reportUser.ClientId);
                }




            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "BidAdjustment - AdjustBid", JsonConvert.SerializeObject(reportUser) + " " + JsonConvert.SerializeObject(promoNegativeRules), reportUser.ClientId);
            }

            return allItems;
        }
    }
}
