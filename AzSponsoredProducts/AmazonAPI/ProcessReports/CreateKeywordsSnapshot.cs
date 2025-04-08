using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports.Summary.Response;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports.Summary;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using System.Text.Json;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports
{
    public class SnapshotCreation
    {
        public async Task<string> CreateSnapshotHere(APIAuthorizationRequest request, ClientProfileCodes profileCode, SnapshotReqestKeywords snapshotReqestKeywords, string snapshotType)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/json";
            string endPoint = "v2/sp/" + snapshotType + "/snapshot";

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request);

            //make object
            string serlializedJson = await MakeObjectToSend(request, snapshotReqestKeywords);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

            //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
            if (responseMessage.IsSuccessStatusCode)
            {
                SnapshotRequestResponseRoot getValues = await JsonSerializer.DeserializeAsync<SnapshotRequestResponseRoot>(responseMessage.Content.ReadAsStream());
                return getValues.snapshotId;
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

                //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                if (responseMessage.IsSuccessStatusCode)
                {
                    SnapshotRequestResponseRoot getValues = await JsonSerializer.DeserializeAsync<SnapshotRequestResponseRoot>(responseMessage.Content.ReadAsStream());
                    return getValues.snapshotId;
                }
                else
                {
                    return null;
                }

            }
        }

        public async Task<string> MakeObjectToSend(APIAuthorizationRequest request, SnapshotReqestKeywords snapshotReqestKeywords)
        {
            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(snapshotReqestKeywords);

            return serlializedJson;
        }
    }
}
