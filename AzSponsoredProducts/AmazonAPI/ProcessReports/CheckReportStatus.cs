using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports.Summary.Response;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports
{
    public class CheckReportStatus
    {
        public async Task<string> CheckStatus(APIAuthorizationRequest request, ClientProfileCodes profileCode, string reportId)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/vnd.createasyncreportrequest.v3+json";
            string endPoint = "reporting/reports/" + reportId;

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonGetApiReports(endPoint, mediaType, auth, profileCode);

            //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
            if (responseMessage.IsSuccessStatusCode)
            {
                ReportLocation.ReportLocationRoot getValues = await JsonSerializer.DeserializeAsync<ReportLocation.ReportLocationRoot>(responseMessage.Content.ReadAsStream());
                if (getValues.status == "COMPLETED")
                {
                    return getValues.url;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
