using AdTool.AzSponsoredProducts.BusinessLogic.Campaigns;
using AdTool.AzSponsoredProducts.BusinessLogic.CampaignsAdGroups;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.CampaignsAdGroups;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdToolApi.Controllers.AdGroups
{
    [Route("api/AdGroups")]
    [ApiController]
    public class AdGroupsController : ControllerBase
    {
        [HttpPost]
        [Route("GetByCampaign")] //api/AdGroups/GetByCampaign
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<GetAdGroupsResponseAPI>> GetByCampaign([FromBody] GetAdGroupsRequest request)
        {
            GetAdGroupLogic getAdGroupLogic = new GetAdGroupLogic();
            var result = await getAdGroupLogic.GetAdGroups(request);
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("Update")] //api/AdGroups/Update
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimpleResponse>> Update([FromBody] UpdateAdGroupRequest request)
        {
            UpdateAdGroupsLogic updateAdGroupsLogic = new UpdateAdGroupsLogic();
            var result = await updateAdGroupsLogic.Update(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }
    }
}