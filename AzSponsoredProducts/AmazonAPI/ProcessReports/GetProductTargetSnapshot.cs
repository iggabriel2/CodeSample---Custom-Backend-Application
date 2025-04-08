using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports
{
    public class GetProductTargetSnapshot
    {
        public async Task<List<ProductTargetSnapshot>> GetSnapshot(APIAuthorizationRequest request, ClientProfileCodes profileCode, string snapshotUrl)
        {
            try
            {
                //basic api setup - CUSTOMIZE VALUES
                string mediaType = "application/json";
                string endPoint = snapshotUrl.Substring(snapshotUrl.IndexOf("v1/"));

                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request);

                //call api here
                AzAPIUtils azAPIUtils = new AzAPIUtils();
                HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonGetApiReports(endPoint, mediaType, auth, profileCode);

                //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                if (responseMessage.IsSuccessStatusCode)
                {
                    string rawReportOutput = "";

                    using (var stream = await responseMessage.Content.ReadAsStreamAsync())
                    using (GZipStream csStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        StreamReader reader = new StreamReader(csStream);
                        rawReportOutput = reader.ReadToEnd();
                    }

                    List<ProductTargetSnapshot> getValues = JsonConvert.DeserializeObject<List<ProductTargetSnapshot>>(rawReportOutput);
                    return getValues;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetSnapshot - GetProductTargetSnapshot", JsonConvert.SerializeObject(request) + " " + JsonConvert.SerializeObject(profileCode) + " " + snapshotUrl + " ", request.ClientId);
                return null;
            }
        }
    }
}
