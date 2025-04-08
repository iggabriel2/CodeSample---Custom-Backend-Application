using AdTool.AzSponsoredProducts.BusinessLogic.Keywords;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.CampaignManagement;
using AdTool.Entities.AzSpApi.Keywords;
using AdTool.Entities.D4Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace AdToolApi.Controllers.Keywords
{
    [Route("api/Keyword")]
    [ApiController]
    public class KeywordController : ControllerBase
    {
        [HttpPost]
        [Route("GetRelatedKeywords")] //api/Keyword/GetRelatedKeywords
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<KeywordResponse>> GetRelatedKeywords([FromBody] KeywordRequest d4KeywordRequest)
        {
            GetKeywordsLogic getKeywordsLogic = new GetKeywordsLogic();
            var result = await getKeywordsLogic.GetKeywords(d4KeywordRequest, true);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("ApplyNegativeKeyword")] //api/Keyword/ApplyNegativeKeyword
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<SimpleResponse>> ApplyNegativeKeyword([FromBody] NegativeOneOffKeyword negativeOneOff)
        {
            ApplyNegativesOneOff applyNegativesOneOff = new ApplyNegativesOneOff();
            var result = await applyNegativesOneOff.ApplyNegative(negativeOneOff);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("ApplySearchTermReviewed")] //api/Keyword/ApplySearchTermReviewed
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<SimpleResponse>> ApplySearchTermReviewed([FromBody] NegativeOneOffKeyword reviewed)
        {
            ApplySearchTermReviewed applySearchTermReviewed = new ApplySearchTermReviewed();
            var result = await applySearchTermReviewed.ApplyReviewed(reviewed);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("RetrievePerfromanceKeywords")] //api/Keyword/RetrievePerfromanceKeywords
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<KeywordPerformanceResponse>> RetrievePerfromanceKeywords([FromBody] KeywordPerformanceRequest request)
        {
            RetrievePerfromanceKeywords retrievePerfromanceKeywords = new RetrievePerfromanceKeywords();
            var result = await retrievePerfromanceKeywords.GetKeywords(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("RetrieveKeywordsByAdGroup")] //api/Keyword/RetrieveKeywordsByAdGroup
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<KeywordResponseByAdGroup>> RetrieveKeywordsByAdGroup([FromBody] KeywordRequestByAdGroup request)
        {
            RetrieveAllKeywords retrieveAllKeywords = new RetrieveAllKeywords();
            var result = await retrieveAllKeywords.GetKeywords(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("RetrievePerfromanceSearchTerms")] //api/Keyword/RetrievePerfromanceSearchTerms
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<SearchTermPerformanceResponse>> RetrievePerfromanceSearchTerms([FromBody] SearchTermPerformanceRequest request)
        {
            RetrievePerformanceSearchTerms retrievePerfromanceSearchTerms = new RetrievePerformanceSearchTerms();
            var result = await retrievePerfromanceSearchTerms.GetSearchTerms(request);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("UpdateKeyword")] //api/Keyword/UpdateKeyword
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<SimpleResponse>> UpdateKeyword([FromBody] KeywordChangeRequest keywordChangeRequest)
        {
            //expression and expression type can be null or excluded when calling a keyword instead of a product target.
            //CampaignId may be populated or excluded. It is not requred for keywords but is for product targets.
            //state is "ENABLED" or "PAUSED"
            UpdateKeywordLogic updateKeywordLogic = new UpdateKeywordLogic();
            UpdateProductTargetLogic updateProductTargetLogic = new UpdateProductTargetLogic();

            SimpleResponse result = new SimpleResponse();
            if (keywordChangeRequest.KeywordType.ToUpper() == "KEYWORD") 
            {
                result = await updateKeywordLogic.Update(keywordChangeRequest);
            }
            else
            {
                result = await updateProductTargetLogic.Update(keywordChangeRequest);
            }

            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }

        //special internal use only
        [HttpPost]
        [Route("BidUpdater")] //api/Keyword/BidUpdater
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimpleResponse>> BidUpdater([FromBody] BidChangeRequest bidChangeRequest)
        {
            BidUpdaterLogic bidUpdaterLogic = new BidUpdaterLogic();
            var result = await bidUpdaterLogic.UpdateBids(bidChangeRequest);
            return result != null ? Ok(result) : NoContent();
        }

        //special internal use only
        [HttpPost]
        [Route("GetSpecialKeywords")] //api/Keyword/GetSpecialKeywords
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<ProductResponse>> GetSpecialKeywords([FromBody] KeywordRequest d4KeywordRequest)
        {
            CustomKeywords getKeywordsLogic = new CustomKeywords();
            var result = await getKeywordsLogic.GetKeywords(d4KeywordRequest);
            return result != null ? Ok(result) : NoContent();
        }

        //special internal use only
        [HttpPost]
        [Route("GetUserDefinedKeywords")] //api/Keyword/GetUserDefinedKeywords
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<GetUserDefinedKeywordsResponse>> GetUserDefinedKeywords([FromBody] GetUserDefinedKeywordsRequest request)
        {
            GetUserDefinedKeywordsLogic getKeywordsLogic = new GetUserDefinedKeywordsLogic();
            var result = await getKeywordsLogic.GetUserDefinedKeywords(request);
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("UpdateUserDefinedKeywords")] //api/Keyword/UpdateUserDefinedKeywords
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<UpdateUserDefinedKeywordsResponse>> UpdateUserDefinedKeywords([FromBody] UpdateUserDefinedKeywordsRequest request)
        {
            UpdateUserDefinedKeywordsLogic getKeywordsLogic = new UpdateUserDefinedKeywordsLogic();
            var result = await getKeywordsLogic.UpdateUserDefinedKeywords(request);
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("GetMatchingPrintForSingleAuthor")] //api/Keyword/GetMatchingPrintForSingleAuthor
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<SimpleResponse>> GetMatchingPrintForSingleAuthor([FromBody] ASINMatchRequest AsinMatchRequest)
        {
            GetMatchinASINs getMatchinASINs = new GetMatchinASINs();
            var result = await getMatchinASINs.GetmatchingAsins(AsinMatchRequest);
            result.APIAuthorization.AccessToken = null;
            result.APIAuthorization.TokenExpirationTime = null;
            return result != null ? Ok(result) : NoContent();
        }
    }
}
