using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports
{
    public class CheckSnapshotStatus
    {
        public async Task<string> CheckStatus(APIAuthorizationRequest request, ClientProfileCodes profileCode, string snapshotId)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/json";
            string endPoint = "v2/sp/snapshots/" + snapshotId;

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonGetApiReports(endPoint, mediaType, auth, profileCode);

            //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
            if (responseMessage.IsSuccessStatusCode)
            {
                SnapshotLocationRoot getValues = await JsonSerializer.DeserializeAsync<SnapshotLocationRoot>(responseMessage.Content.ReadAsStream());
                if (getValues.status == "SUCCESS")
                {
                    return getValues.location;
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
