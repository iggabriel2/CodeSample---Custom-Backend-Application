using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.AmazonAPI.ExtraKeywordPromoManagement;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.AzSpApi.CampaignManagement;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class ApplySearchTermReviewed
    {
        public async Task<SimpleResponse> ApplyReviewed(NegativeOneOffKeyword negativeOneOff)
        {
            SimpleResponse simpleResponse = new SimpleResponse();

            try
            {
                if (negativeOneOff.SimpleKeywordType.ToLower() == "producttarget")
                {
                    SaveSummaryReportAction action = new SaveSummaryReportAction();
                    action.keywordType = negativeOneOff.KeywordType;
                    action.AzCampaignId = negativeOneOff.AzCampaignId;
                    action.CountryId = negativeOneOff.CountryId;
                    action.SearchTerm = negativeOneOff.SearchTerm;
                    action.AdGroup = negativeOneOff.AdGroup;
                    action.Product = true;
                    action.KeywordId = negativeOneOff.KeywordId;

                    KeywordFunctions keywordFunctions = new KeywordFunctions();
                    action.keyword = await keywordFunctions.GetKeywordText(negativeOneOff.KeywordId, negativeOneOff.CountryId, negativeOneOff.Authorization.ClientId);

                    var functionResponse = await keywordFunctions.MarkSearchTermReviewed(action, negativeOneOff.Authorization.ClientId);

                    CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = negativeOneOff.CountryId;
                    countrySucces.Success = true;

                    simpleResponse.CountrySuccess.Add(countrySucces);
                    return simpleResponse;
                }
                else
                {
                    SaveSummaryReportAction action = new SaveSummaryReportAction();
                    action.KeywordId = negativeOneOff.KeywordId;
                    action.keywordType = negativeOneOff.KeywordType;
                    action.AzCampaignId = negativeOneOff.AzCampaignId;
                    action.CountryId = negativeOneOff.CountryId;
                    action.SearchTerm = negativeOneOff.SearchTerm;
                    action.AdGroup = negativeOneOff.AdGroup;
                    action.Product = false;

                    KeywordFunctions keywordFunctions = new KeywordFunctions();
                    action.keyword = await keywordFunctions.GetKeywordText(negativeOneOff.KeywordId, negativeOneOff.CountryId, negativeOneOff.Authorization.ClientId);

                    var functionResponse = await keywordFunctions.MarkSearchTermReviewed(action, negativeOneOff.Authorization.ClientId);

                    CountrySuccess countrySucces = new CountrySuccess();
                    countrySucces.CountryId = negativeOneOff.CountryId;
                    countrySucces.Success = true;

                    simpleResponse.CountrySuccess.Add(countrySucces);
                    return simpleResponse;
                }
            }
            catch(Exception ex)
            {
                simpleResponse.APIAuthorization.ErrorMessage = "Review failed";
                return simpleResponse;
            }
        }
    }
}
