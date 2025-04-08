using AdTool.AzSponsoredProducts.AmazonAPI.General;
using AdTool.Entities.Payments;
using AdTool.PaymentProcessor.BusinessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static AdTool.AzSponsoredProducts.AmazonAPI.General.LocationFetcher;

namespace AdToolApi.Controllers.General
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpGet]
        [Route("GetCountryOfOrigin")] //api/Location/GetCountryOfOrigin
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<CountryResponse>> GetCountryOfOrigin(string ipAddress)
        {
            LocationFetcher fetcher = new LocationFetcher();
            var result = await fetcher.GetCountry(ipAddress);
            return result != null ? Ok(result) : NoContent();
        }
    }
}
