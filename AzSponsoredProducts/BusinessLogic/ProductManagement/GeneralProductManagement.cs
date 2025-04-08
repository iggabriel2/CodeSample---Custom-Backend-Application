using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using System.Text.Json;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProductManagement
{
    public class GeneralProductManagement
    {
        public async Task<ProductResponse> GetProductInfo(ProductRequest productRequest)
        {
            GetProduct getProduct = new GetProduct();
            ProductResponse productResponse = new ProductResponse();

            try
            {
                productResponse = await getProduct.GetProductInfo(productRequest);

                //if we get an error, clear the token and try once more - SEE IF PRODUCT WAS MADE HERE ON CREATION
                if (!string.IsNullOrEmpty(productResponse.APIAuthorization.ErrorMessage))
                {
                    productRequest.Authorization.AccessToken = "";

                    //clear token and try again
                    productResponse = await getProduct.GetProductInfo(productRequest);
                }

                return productResponse;

            }
            catch(Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetProductInfo - GeneralProductManagement.cs";
                logError.ClientId = productRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(productRequest);
                await logging.WriteToLog(logError);

                productResponse.APIAuthorization.ErrorMessage = "No Book Found";
                return productResponse;
            }
        }
    }
}
