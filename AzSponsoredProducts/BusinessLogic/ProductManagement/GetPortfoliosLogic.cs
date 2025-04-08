using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.ProductManagement;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProductManagement
{
    public class GetPortfoliosLogic
    {
        public async Task<PortfolioListResponse> GetPortfolioList(PortfolioRequest portfolioRequest)
        {
            GetPortfolios getPortfolios = new GetPortfolios();
            PortfolioListResponse portfolioListresponse = new PortfolioListResponse();

            try
            {
                portfolioListresponse = await getPortfolios.GetPortfolioInfo(portfolioRequest);

                //if we get an error, clear the token and try once more
                if (!string.IsNullOrEmpty(portfolioListresponse.APIAuthorization.ErrorMessage))
                {
                    portfolioRequest.Authorization.AccessToken = "";

                    //clear token and try again
                    portfolioListresponse = await getPortfolios.GetPortfolioInfo(portfolioRequest);
                }

                return portfolioListresponse;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetPortfoliosLogic";
                logError.ClientId = portfolioRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(portfolioRequest);
                await logging.WriteToLog(logError);

                portfolioListresponse.APIAuthorization.ErrorMessage = "Errored on request";
                return portfolioListresponse;
            }
        }
    }
}
