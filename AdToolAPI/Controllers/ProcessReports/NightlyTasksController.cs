using AdTool.AzSponsoredProducts.BusinessLogic.Keywords;
using AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.D4Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace zTest.ProcessReports
{
    [Route("api/NightlyTasks")]
    [ApiController]
    public class NightlyTasksController : ControllerBase
    {
        [HttpGet]
        [Route("ProcessReportsNow")] //api/ProcessReports/ProcessReportsNow
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimpleResponse>> ProcessReportsNow()
        {
            ProcessReportsLogic processReportsLogic = new ProcessReportsLogic();
            var response = await processReportsLogic.ProcessReportsLogicNow();
            return Ok();
        }

        [HttpGet]
        [Route("RefreshKeywordsNow")] //api/ProcessReports/RefreshKeywordsNow
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimpleResponse>> RefreshKeywordsNow()
        {
            RefreshKeywordLogic refreshKeywordLogic = new RefreshKeywordLogic();
            var response = await refreshKeywordLogic.RefreshKeywordsNow();
            return Ok();
        }
    }
}
