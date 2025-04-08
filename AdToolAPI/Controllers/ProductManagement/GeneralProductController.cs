using AdTool.AzSponsoredProducts.BusinessLogic.ProductManagement;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
using AdTool.Entities.AzSpApi.ProductManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdToolApi.Controllers.ProductManagement
{
    [Route("api/GeneralProduct")]
    [ApiController]
    public class GeneralProductController : ControllerBase
    {
        [HttpPost]
        [Route("GetProductInfo")] //api/GeneralProduct/GetProductInfo
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<ProductResponse>> GetProductInfo([FromBody] ProductRequest productRequest)
        {
            GeneralProductManagement generalProductManagement = new GeneralProductManagement();
            var result = await generalProductManagement.GetProductInfo(productRequest);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("GetPortfolios")] //api/GeneralProduct/GetPortfolios
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<PortfolioListResponse>> GetPortfolios([FromBody] PortfolioRequest portfolioRequest)
        {
            GetPortfoliosLogic gpl = new GetPortfoliosLogic();
            var result = await gpl.GetPortfolioList(portfolioRequest);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("CreatePortfolio")] //api/GeneralProduct/CreatePortfolio
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<PortfolioResponse>> CreatePortfolio([FromBody] CreatePortfolioRequest createPortfolioRequest)
        {
            CreatePortfoliosLogic cp = new CreatePortfoliosLogic();
            var result = await cp.CreatePortfolio(createPortfolioRequest);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("ProductData")] //api/GeneralProduct/GetProductData
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<GetProductResponseAPI>> GetProductData([FromBody] GetCampaignRequestApi request)
        {
            GetProductLogic getProductLogic = new GetProductLogic();
            var result = await getProductLogic.GetProducts(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

    }
}