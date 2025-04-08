using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Campaigns
{
    public class GetCampaignNameLogic
    {
        public async Task<SimpleResponse> GetCampaignName(CampaignNameRequest campaignNameRequest)
        {
            GetCampaign getCampaign = new GetCampaign();
            SimpleResponse simpleResponse = new SimpleResponse();

            try
            {
                simpleResponse = await getCampaign.GetCampaignName(campaignNameRequest);

                //if we get an error, clear the token and try once more
                if (!string.IsNullOrEmpty(simpleResponse.APIAuthorization.ErrorMessage))
                {
                    campaignNameRequest.Authorization.AccessToken = "";

                    //clear token and try again
                    simpleResponse = await getCampaign.GetCampaignName(campaignNameRequest);
                }

                return simpleResponse;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCampaignName";
                logError.ClientId = campaignNameRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(campaignNameRequest);
                await logging.WriteToLog(logError);

                simpleResponse.APIAuthorization.ErrorMessage = "Errored on request";
                return simpleResponse;
            }
        }
    }
}
