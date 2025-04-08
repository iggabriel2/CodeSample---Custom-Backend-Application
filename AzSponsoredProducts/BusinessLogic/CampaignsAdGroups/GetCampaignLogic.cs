using AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
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

namespace AdTool.AzSponsoredProducts.BusinessLogic.CampaignsAdGroups
{
    public class GetCampaignLogic
    {
        public async Task<GetCampaignResponseApi> GetCampaigns(GetCampaignRequestApi request)
        {
            GetCampaignResponseApi response = new GetCampaignResponseApi();

            try
            {
                //List<KeywordPerformanceByCampaign> keywordPerformanceByCampaign = new List<KeywordPerformanceByCampaign>();
                //RetrieveKeywordManagementData rkd = new RetrieveKeywordManagementData();
                //keywordPerformanceByCampaign = await rkd.GetKeywordPerformanceByCampaign(request.countryId, request.campaignName, request.campaignStatus, request.productId, request.monthYearFrom, request.monthYearTo, request.campaignUsage, request.Authorization.ClientId);

                RetrieveData retrieveData = new RetrieveData();
                var allCampaigns = await retrieveData.GetAllCampaignsByCountryGeneralView(request.Authorization.ClientId, Convert.ToInt32(request.countryId));

                if (request.countryId == null)
                {
                    request.countryId = 0;
                }

                List<KeywordPerformanceByMonth> keywordPerformanceByMonth = new List<KeywordPerformanceByMonth>();
                CombineKeywordData combineKeywordData = new CombineKeywordData();
                keywordPerformanceByMonth = await combineKeywordData.GetData(request.monthYearFrom, request.monthYearTo, request.Authorization.ClientId, "allcampaigns", (int)request.countryId);

                foreach(var az in  allCampaigns)
                {
                    List<KeywordPerformanceByMonth> items = keywordPerformanceByMonth.Where(x => x.CampaignId == az.AZCampaignId && x.Country == Convert.ToInt32(az.CountryId)).ToList();

                    if (items != null && items.Count > 0)
                    {
                        foreach(var performanceItem in items)
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

                List<AzSpCampaignSummary> azSpCampaignSummaries = new List<AzSpCampaignSummary>();
                foreach (var performanceItem in allCampaigns)
                {
                    decimal showRoundedCPC = await GeneralStaticUtils.Round(performanceItem.CTC);
                    decimal showRoundedSpend = await GeneralStaticUtils.Round(performanceItem.Spend);
                    decimal showRoundedSales = await GeneralStaticUtils.Round(performanceItem.Sales);

                    AzSpCampaignSummary az = new AzSpCampaignSummary();
                    az.Country = performanceItem.Country;
                    az.CountryId = performanceItem.CountryId.ToString();
                    az.QAPCampaignId = performanceItem.QAPCampaignId;
                    az.AZCampaignId = performanceItem.AZCampaignId;
                    az.CampaignName = performanceItem.CampaignName;
                    az.QAPProductId = performanceItem.QAPProductId;
                    az.ProductName = performanceItem.ProductName;
                    az.Status = performanceItem.Status;
                    az.Usage = performanceItem.Usage;
                    az.Clicks = performanceItem.Clicks.ToString();
                    az.Spend =  showRoundedSpend;
                    az.CTC = showRoundedCPC;
                    az.KindlePageReads = performanceItem.KindlePageReads.ToString();
                    az.Orders = performanceItem.Orders.ToString();
                    az.Sales = showRoundedSales;
                    az.UsageTypeId = performanceItem.UsageTypeId;
                    az.Impressions = performanceItem.Impressions;
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

                
                response.CampaignSummaryData = azSpCampaignSummaries;

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
