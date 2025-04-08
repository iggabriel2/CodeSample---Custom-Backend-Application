using AdTool.AzSponsoredProducts.BusinessLogic.Campaigns;
using AdTool.AzSponsoredProducts.BusinessLogic.CampaignsAdGroups;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.AzSpApi.CampaignManagement;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdToolApi.Controllers.Campaigns
{
    [Route("api/Campaign")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        [HttpPost]
        [Route("Create")] //api/Campaign/Create
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<CampaignResponse>> Create([FromBody] CampaignRequest campaignRequest)
        {
            CreateCampaignLogic createCampaignLogic = new CreateCampaignLogic();
            var result = await createCampaignLogic.CreateCampaign(campaignRequest);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();



            //REAL WORLD
            //CreateCampaignLogic createCampaignLogic = new CreateCampaignLogic();

            ////if tier1 or performance, wait for response. otherwise, just make it and send back a success
            //if (campaignRequest.CampaignUsageType == 2 || campaignRequest.CampaignUsageType == 3)
            //{
            //    var result = await createCampaignLogic.CreateCampaign(campaignRequest);
            //    return result != null ? Ok(result) : NoContent();
            //}
            //else
            //{
            //    createCampaignLogic.CreateCampaign(campaignRequest);

            //    //we are going to move on and let it create in the background
            //    SimpleResponse simpleResponse = new SimpleResponse();
            //    simpleResponse.APIAuthorization.AccessToken = campaignRequest.Authorization.AccessToken;
            //    simpleResponse.APIAuthorization.ClientId = campaignRequest.Authorization.ClientId;
            //    simpleResponse.APIAuthorization.TokenExpirationTime = campaignRequest.Authorization.TokenExpirationTime;

            //    foreach(var countryId in campaignRequest.CountriesToCreate)
            //    {
            //        CountrySuccess countrySuccess = new CountrySuccess();
            //        countrySuccess.CountryId = countryId;
            //        countrySuccess.Success = true;
            //        simpleResponse.CountrySuccess.Add(countrySuccess);
            //    }

            //    return simpleResponse != null ? Ok(simpleResponse) : NoContent();
            //}
        }

        //success for a country means you are clear to use the campaign name. Failure means it already exists.
        [HttpPost]
        [Route("GetCampaignName")] //api/Campaign/GetCampaignName
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimpleResponse>> GetCampaignName([FromBody] CampaignNameRequest campaignNameRequest)
        {
            GetCampaignNameLogic getCampaignNameLogic = new GetCampaignNameLogic();
            var result = await getCampaignNameLogic.GetCampaignName(campaignNameRequest);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }
        
        [HttpPost]
        [Route("UpdateCampaign")] //api/Campaign/UpdateCampaign
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimpleResponse>> UpdateCampaign([FromBody] CampaignUpdateRequest request)
        {
            UpdateCampaignLogic updateCampaignLogic = new UpdateCampaignLogic();
            var result = await updateCampaignLogic.UpdateCampaign(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("UpdateMultipleCampaigns")] //api/Campaign/UpdateMultipleCampaigns
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimpleResponse>> UpdateMultipleCampaigns([FromBody] CampaignUpdateMultipleRequest request)
        {
            UpdateMultipleCampaignLogic updateCampaignLogic = new UpdateMultipleCampaignLogic();
            var result = await updateCampaignLogic.UpdateCampaign(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("GetCampaigns")] //api/Campaign/GetCampaigns
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<GetCampaignResponseApi>> GetCampaigns([FromBody] GetCampaignRequestApi request)
        {
            GetCampaignLogic getCampaignLogic = new GetCampaignLogic();
            var result = await getCampaignLogic.GetCampaigns(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }
    }
}
