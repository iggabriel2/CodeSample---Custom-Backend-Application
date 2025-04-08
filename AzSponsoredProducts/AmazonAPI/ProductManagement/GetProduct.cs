using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using System.Net;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using static System.Formats.Asn1.AsnWriter;
using System.Diagnostics.Metrics;
using System.Net.Mime;
using System.Text.Json;
using System.Linq.Expressions;
using System.Text;
using AdTool.AzSponsoredProducts.Utils;
using Azure;
using AdTool.AzSponsoredProducts.Data;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Logging;
using AdTool.BusinessLogic.DataAccess;
using AdTool.AzSponsoredProducts.Data.ReportData;

namespace AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement
{
    public class GetProduct
    {
        public async Task<ProductResponse> GetProductInfo(ProductRequest productRequestValues)
        {
            //basic api setup - CUSTOMIZE VALUES
            string mediaType = "application/vnd.productmetadataresponse.v3+json";
            string endPoint = "product/metadata";

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(productRequestValues.Authorization);

            //get profile codes
            RetrieveReportData rrdCodes = new RetrieveReportData();
            productRequestValues.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(productRequestValues.Authorization.ClientId);

            //handle if token fails
            if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
            {
                ProductResponse productResponse = new ProductResponse();
                productResponse.APIAuthorization.ErrorMessage = "Token Failed";
                return productResponse;
            }


            //this holds our response - CUSTOMIZE OBJECT
            ProductResponse myResponse = new ProductResponse();
            myResponse.APIAuthorization = auth;


            //check each country
            foreach (ClientProfileCodes profileCode in productRequestValues.Authorization.ClientProfileCodes)
            {
                try
                {

                    //make object
                    string serlializedJson = await MakeObjectToSend(productRequestValues);

                    //call api here
                    AzAPIUtils azAPIUtils = new AzAPIUtils();
                    HttpResponseMessage responseMessage = await azAPIUtils.CallAmazonPostApi(endPoint, mediaType, auth, profileCode, serlializedJson);

                    //handle response - CUSTOMIZE RESPONSE VALUES AND ERROR MESSAGES
                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        AzProductResponse? getValues = await JsonSerializer.DeserializeAsync<AzProductResponse>(responseMessage.Content.ReadAsStream());

                        if (string.IsNullOrEmpty(myResponse.ProductName))
                        {
                            myResponse.ProductName = getValues.ProductMetadataList[0].title;
                            myResponse.ImageURL = getValues.ProductMetadataList[0].imageUrl;
                            myResponse.Author = getValues.ProductMetadataList[0].brand;
                            myResponse.Asin = productRequestValues.Asin;
                        }


                        if (getValues.ProductMetadataList[0].availability.ToUpper() != "OUT_OF_STOCK" && getValues.ProductMetadataList[0].availability.ToUpper() != "AVAILABLE_DATE" && getValues.ProductMetadataList[0].eligibilityStatus.ToUpper() == "ELIGIBLE")
                        {
                            myResponse.ValidCountries.Add(profileCode.CountryId);
                        }
                    }
                    else
                    {

                        myResponse.APIAuthorization.ErrorMessage = "Failed on GetProduct: " + responseMessage.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = ex.ToString();
                    logError.FailureMethod = "GetProductInfo";
                    logError.ClientId = productRequestValues.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(productRequestValues);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed on GetProduct";
                    return myResponse;
                }
            }

            return myResponse;
        }

        //CUSTOMIZE OBJECT
        public async Task<string> MakeObjectToSend(ProductRequest productRequestValues)
        {
            //make object to send
            AzAPIProductRequest productRequest = new AzAPIProductRequest(); //this holds your post parameters
            productRequest.checkItemDetails = true;
            productRequest.cursorToken = "AVC";
            productRequest.adType = productRequestValues.AdType;
            productRequest.checkEligibility = true;
            productRequest.pageSize = 1;
            productRequest.asins.Add(productRequestValues.Asin);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(productRequest);

            return serlializedJson;
        }

    }
}
