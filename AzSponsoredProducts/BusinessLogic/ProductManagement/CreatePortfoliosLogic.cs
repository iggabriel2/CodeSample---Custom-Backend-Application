using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProductManagement
{
    public class CreatePortfoliosLogic
    {
        public async Task<PortfolioResponse> CreatePortfolio(CreatePortfolioRequest myRequest)
        {
            CreatePortfolios createPortfolios = new CreatePortfolios();
            PortfolioResponse simpleResponse = new PortfolioResponse();

            try
            {
                simpleResponse = await createPortfolios.CreatePortfolio(myRequest);

                //if we get an error, clear the token and try once more
                if (!string.IsNullOrEmpty(simpleResponse.APIAuthorization.ErrorMessage))
                {
                    myRequest.Authorization.AccessToken = "";

                    //clear token and try again
                    simpleResponse = await createPortfolios.CreatePortfolio(myRequest);
                }

                return simpleResponse;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreatePortfoliosLogic";
                logError.ClientId = myRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(myRequest);
                await logging.WriteToLog(logError);

                simpleResponse.APIAuthorization.ErrorMessage = "Errored on CreatePortfolio";
                return simpleResponse;
            }
        }
    }
}
