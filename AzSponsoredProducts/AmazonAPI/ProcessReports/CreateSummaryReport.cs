using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports.Summary;
using System.Text.Json;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports.Summary.Response;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using Azure;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports
{
    public class CreateSummaryReport
    {
        public async Task<string> CreateSummaryReportHere(APIAuthorizationRequest request, DateTime startDate, DateTime endDate, ClientProfileCodes profileCode)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/vnd.createasyncreportrequest.v3+json";
            string endPoint = "reporting/reports";

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request);

            //make object
            string serlializedJson = await MakeObjectToSend(request, startDate, endDate);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

            //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
            if (responseMessage.IsSuccessStatusCode)
            {
                ReportRequestResponseRoot getValues = await JsonSerializer.DeserializeAsync<ReportRequestResponseRoot>(responseMessage.Content.ReadAsStream());
                return getValues.reportId;
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

                //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                if (responseMessage.IsSuccessStatusCode)
                {
                    ReportRequestResponseRoot getValues = await JsonSerializer.DeserializeAsync<ReportRequestResponseRoot>(responseMessage.Content.ReadAsStream());
                    return getValues.reportId;
                }
                else
                {
                    return null;
                }

            }
        }

        public async Task<string> MakeObjectToSend(APIAuthorizationRequest request, DateTime startDate, DateTime endDate)
        {
            RootSummaryReportRequest rootSummaryReportRequest = new RootSummaryReportRequest();

            //populate - last date is minus three because it takes 72 hours for Amazon data to settle
            rootSummaryReportRequest.name = "summary report";
            rootSummaryReportRequest.startDate = startDate.ToString("yyyy'-'MM'-'dd");
            rootSummaryReportRequest.endDate = endDate.ToString("yyyy'-'MM'-'dd");

            BusinessObjects.Reports.Summary.Configuration configuration = new BusinessObjects.Reports.Summary.Configuration();
            configuration.adProduct = "SPONSORED_PRODUCTS";
            configuration.reportTypeId = "spSearchTerm";
            configuration.timeUnit = "SUMMARY";
            configuration.format = "GZIP_JSON";
            configuration.groupBy.Add("searchTerm");

            //add all columns I really want
            configuration.columns.Add("impressions");
            configuration.columns.Add("clicks");
            configuration.columns.Add("searchTerm");
            configuration.columns.Add("campaignId");
            configuration.columns.Add("campaignName");
            configuration.columns.Add("keywordType");
            configuration.columns.Add("adGroupId");
            configuration.columns.Add("costPerClick");
            configuration.columns.Add("portfolioId");
            configuration.columns.Add("purchases14d");
            configuration.columns.Add("keywordId");
            configuration.columns.Add("keyword");
            configuration.columns.Add("cost");
            configuration.columns.Add("adGroupName");
            configuration.columns.Add("kindleEditionNormalizedPagesRead14d");
            configuration.columns.Add("attributedSalesSameSku14d");
            configuration.columns.Add("clickThroughRate");
            configuration.columns.Add("roasClicks14d");
            configuration.columns.Add("unitsSoldClicks14d");
            configuration.columns.Add("campaignStatus");

            //add all filters I really want
            BusinessObjects.Reports.Summary.Filter filter = new BusinessObjects.Reports.Summary.Filter();
            filter.field = "keywordType";

            List<string> values = new List<string>();

            //use these when targeting products
            values.Add("TARGETING_EXPRESSION");
            values.Add("TARGETING_EXPRESSION_PREDEFINED");

            //use these when targeting keywords
            values.Add("BROAD");
            values.Add("PHRASE");
            values.Add("EXACT");
            filter.values = values;

            configuration.filters.Add(filter);
            rootSummaryReportRequest.configuration = configuration;

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(rootSummaryReportRequest);

            return serlializedJson;
        }
    }
}
