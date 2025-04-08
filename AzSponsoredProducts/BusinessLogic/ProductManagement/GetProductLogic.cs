using AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
using AdTool.Entities.AzSpApi.ProductManagement;
using AdTool.Entities.CampaignsAdGroups;
using AdTool.Entities.View;
using Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProductManagement
{
    public class GetProductLogic
    {
        public async Task<GetProductResponseAPI> GetProducts(GetCampaignRequestApi request)
        {
            GetProductResponseAPI response = new GetProductResponseAPI();

            try
            {
                //List<KeywordPerformanceByCampaign> keywordPerformanceByCampaign = new List<KeywordPerformanceByCampaign>();
                //RetrieveKeywordManagementData rkd = new RetrieveKeywordManagementData();
                //keywordPerformanceByCampaign = await rkd.GetKeywordPerformanceByCampaign(request.countryId, request.campaignName, request.campaignStatus, request.productId, request.monthYearFrom, request.monthYearTo, request.campaignUsage, request.Authorization.ClientId);

                RetrieveData retrieveData = new RetrieveData();
                var allProducts = await retrieveData.GetAllProductsByCountry(request.Authorization.ClientId, request.countryId);

                List<AllCampaigns> allCampaigns = new List<AllCampaigns>();

                if (request.countryId != null && request.countryId != 0)
                {
                    allCampaigns = await retrieveData.GetAllCampaignsByCountry(request.Authorization.ClientId, Convert.ToInt32(request.countryId));
                }
                else
                {
                    allCampaigns = await retrieveData.GetAllCampaigns(request.Authorization.ClientId);
                }



                List<int> campaignIds = allCampaigns.Select(x => x.QAPCampaignId).ToList();


                if (request.countryId == null)
                {
                    request.countryId = 0;
                }

                //we are still asking for this by all campaigns and then compressing it down
                List<KeywordPerformanceByMonth> keywordPerformanceByMonthRaw1 = new List<KeywordPerformanceByMonth>();
                CombineKeywordData combineKeywordData = new CombineKeywordData();
                keywordPerformanceByMonthRaw1 = await combineKeywordData.GetData(request.monthYearFrom, request.monthYearTo, request.Authorization.ClientId, "allcampaigns", (int)request.countryId);

                var keywordPerformanceByMonthRaw = keywordPerformanceByMonthRaw1.Where(x => campaignIds.Contains(x.QAPCampaignId)).ToList();

                var keywordPerformanceByMonth = (from t in keywordPerformanceByMonthRaw
                                             group t by new { t.ProductId, t.Country } into grp
                                       select new KeywordPerformanceByMonth
                                       {
                                           ProductId = grp.Key.ProductId,
                                           Country = grp.Key.Country,
                                           AttributedSalesSameSku14d = grp.Sum(t => t.AttributedSalesSameSku14d) != null ? (decimal)grp.Sum(t => t.AttributedSalesSameSku14d) : 0,
                                           Clicks = grp.Sum(t => t.Clicks) != null ? (int)grp.Sum(t => t.Clicks) : 0,
                                           Cost = grp.Sum(t => t.Cost) != null ? (decimal)grp.Sum(t => t.Cost) : (decimal)0,
                                           Impressions = grp.Sum(t => t.Impressions) != null ? (int)grp.Sum(t => t.Impressions) : 0,
                                           KindleEditionNormalizedPagesRead14d = grp.Sum(t => t.KindleEditionNormalizedPagesRead14d) != null ? (int)grp.Sum(t => t.KindleEditionNormalizedPagesRead14d) : 0,
                                           purchases14d = grp.Sum(t => t.purchases14d) != null ? (int)grp.Sum(t => t.purchases14d) : 0,
                                           CPC = grp.Sum(t => t.Clicks) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Cost) != null ? grp.Sum(t => t.Cost) / grp.Sum(t => t.Clicks) : 0,
                                           ConversionRate = grp.Sum(t => t.purchases14d) != null && grp.Sum(t => t.Clicks) != 0 && grp.Sum(t => t.Clicks) != null ? (grp.Sum(t => t.purchases14d) / grp.Sum(t => t.Clicks)) * 100 : 0,
                                       }).ToList();


                foreach (var az in allProducts)
                { 
                    List<KeywordPerformanceByMonth> items = new List<KeywordPerformanceByMonth>();

                    if (request.countryId != null && request.countryId != 0)
                    {
                        items = keywordPerformanceByMonth.Where(x => x.ProductId == az.QAPProductId && x.Country == Convert.ToInt32(request.countryId)).ToList();
                    }
                    else
                    {
                        items = keywordPerformanceByMonth.Where(x => x.ProductId == az.QAPProductId).ToList();
                    }

                    if (items != null && items.Count > 0)
                    {
                        foreach (var performanceItem in items)
                        {
                            az.Clicks = az.Clicks + performanceItem.Clicks;
                            az.Spend = az.Spend + performanceItem.Cost;
                            az.CTC = await GeneralStaticUtils.Round(performanceItem.CPC);
                            az.KindlePageReads = az.KindlePageReads + performanceItem.KindleEditionNormalizedPagesRead14d;
                            az.Orders = az.Orders + performanceItem.purchases14d;
                            az.Sales = az.Sales + performanceItem.AttributedSalesSameSku14d;
                            az.Impressions = az.Impressions + performanceItem.Impressions;
                        }

                    }
                }

                List<AzSpProductSummaryData> azSpCampaignSummaries = new List<AzSpProductSummaryData>();
                foreach (var performanceItem in allProducts)
                {
                    decimal showRoundedCPC = await GeneralStaticUtils.Round(performanceItem.CTC);
                    decimal showRoundedSpend = await GeneralStaticUtils.Round(performanceItem.Spend);
                    decimal showRoundedSales = await GeneralStaticUtils.Round(performanceItem.Sales);

                    AzSpProductSummaryData az = new AzSpProductSummaryData();
                    az.CountryList = performanceItem.CountryList;
                    az.QAPProductId = performanceItem.QAPProductId;
                    az.ProductName = performanceItem.ProductName;
                    az.Clicks = performanceItem.Clicks.ToString();
                    az.Spend = showRoundedSpend;
                    az.CTC = showRoundedCPC;
                    az.KindlePageReads = performanceItem.KindlePageReads.ToString();
                    az.Orders = performanceItem.Orders.ToString();
                    az.Sales = showRoundedSales;
                    az.Impressions = performanceItem.Impressions;
                    az.Asin = performanceItem.Asin;
                    az.AzImageUrl = performanceItem.AzImageURL;
                    az.ResearchCampaigns = performanceItem.CampaignCount;
                    if (performanceItem.Sales != 0)
                    {
                        decimal result1 = (performanceItem.Spend / performanceItem.Sales) * 100;
                        decimal result = await GeneralStaticUtils.Round(result1);
                        az.ACOS = result;
                    }

                    if (performanceItem.Clicks != 0)
                    {
                        decimal result1 = (Convert.ToDecimal(performanceItem.Orders) / Convert.ToDecimal(performanceItem.Clicks)) * 100;
                        decimal result = await GeneralStaticUtils.Round(result1);
                        az.Conversion = result;
                    }

                    if (performanceItem.Impressions != 0)
                    {
                        decimal ctrRaw = (Convert.ToDecimal(performanceItem.Clicks) / Convert.ToDecimal(performanceItem.Impressions)) * 100;
                        decimal result = await GeneralStaticUtils.Round(ctrRaw);
                        az.CTR = result;
                    }

                    azSpCampaignSummaries.Add(az);
                }


                response.ProductSummaryData = azSpCampaignSummaries;

                response.APIAuthorization.ClientId = request.Authorization.ClientId;

            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetCampaigns", JsonSerializer.Serialize(request), request.Authorization.ClientId);
                response.APIAuthorization.ErrorMessage = "Failed to get campaigns";
            }

            return response;
        }
    }
}
