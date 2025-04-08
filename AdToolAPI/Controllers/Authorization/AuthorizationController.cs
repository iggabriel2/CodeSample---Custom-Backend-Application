using AdTool.AzSponsoredProducts.BusinessLogic.Authorization;
using AdTool.Entities.AzSp.ClientAuthorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace zTest.Authorization
{
    [Route("api/Authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpPost]
        [Route("GetOriginalAuthorization")] //api/Authorization/GetOriginalAuthorization
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<OriginalAPIAuthorizationResponse>> GetOriginalAuthorization([FromBody] OriginalApiAuthorizationRequest originalApiAuthorizationRequest)
        {
            Authorize authorize = new Authorize();
            var result = await authorize.GetOriginalAuthorization(originalApiAuthorizationRequest);
            result.AccessToken = null;
            result.RefreshToken = null;
            result.TokenExpirationTime = null;
            result.ClientProfileCodes.Clear();
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("RecheckCountries")] //api/Authorization/RecheckCountries
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<OriginalAPIAuthorizationResponse>> RecheckCountries([FromBody] CountryAuthorizationUpdateRequest countryAuthorizationUpdateRequest)
        {
            RecheckAllCountries recheck = new RecheckAllCountries();
            var result = await recheck.RecheckCountries(countryAuthorizationUpdateRequest);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }
    }
}