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

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class ApplyNegativesOneOff
    {
        public async Task<SimpleResponse> ApplyNegative(NegativeOneOffKeyword negativeOneOff)
        {
            SimpleResponse simpleResponse = new SimpleResponse();

            try
            {
                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(negativeOneOff.Authorization);


                //get profile codes
                RetrieveReportData rrdCodes = new RetrieveReportData();
                negativeOneOff.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(negativeOneOff.Authorization.ClientId);

                //handle if token fails
                if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
                {
                    simpleResponse.APIAuthorization.AccessToken = "";
                    simpleResponse.APIAuthorization.ErrorMessage = "Token Failed";
                    return simpleResponse;
                }
                else
                {
                    simpleResponse.APIAuthorization = auth;
                }

                negativeOneOff.Authorization.AccessToken = auth.AccessToken;
                negativeOneOff.Authorization.TokenExpirationTime = auth.TokenExpirationTime;


                CampaignRequest request = new CampaignRequest();
                request.Authorization = negativeOneOff.Authorization;

         
                if (negativeOneOff.SimpleKeywordType.ToLower() == "producttarget")
                {
                    SaveSummaryReportAction negativesToApply = new SaveSummaryReportAction();
                    negativesToApply.SearchTerm = negativeOneOff.SearchTerm;
                    negativesToApply.AzCampaignId = negativeOneOff.AzCampaignId;
                    negativesToApply.AdGroup = negativeOneOff.AdGroup;

                    string negativeProductEndpoint = "/sp/negativeTargets";
                    string mediaTypeNegativeProducts = "application/vnd.spNegativeTargetingClause.v3+json";

                    AddNegativeProd addNegativeProd = new AddNegativeProd();
                    var responseNegative = await addNegativeProd.SetNegativeProduct(negativeOneOff.CountryId, request, negativeProductEndpoint, mediaTypeNegativeProducts, auth, negativesToApply);

                    if (responseNegative == "1")
                    {
                        SaveSummaryReportAction action = new SaveSummaryReportAction();
                        action.keywordType = negativeOneOff.KeywordType;
                        action.AzCampaignId = negativeOneOff.AzCampaignId;
                        action.Negative = true;
                        action.CountryId = negativeOneOff.CountryId;
                        action.SearchTerm = negativeOneOff.SearchTerm;
                        action.AdGroup = negativeOneOff.AdGroup;
                        action.Product = true;
                        action.KeywordId = negativeOneOff.KeywordId;

                        KeywordFunctions keywordFunctions = new KeywordFunctions();
                        action.keyword = await keywordFunctions.GetKeywordText(negativeOneOff.KeywordId, negativeOneOff.CountryId, negativeOneOff.Authorization.ClientId);

                        var functionResponse = await keywordFunctions.ApplyNegativeOneOff(action, negativeOneOff.Authorization.ClientId);

                        CountrySuccess countrySucces = new CountrySuccess();
                        countrySucces.CountryId = negativeOneOff.CountryId;
                        countrySucces.Success = true;

                        simpleResponse.CountrySuccess.Add(countrySucces);
                        return simpleResponse;
                    }
                    else
                    {
                        CountrySuccess countrySucces = new CountrySuccess();
                        countrySucces.CountryId = negativeOneOff.CountryId;
                        countrySucces.Success = false;

                        simpleResponse.CountrySuccess.Add(countrySucces);
                        return simpleResponse;
                    }
                }
                else
                {
                    NegativeKeywordsNewCampaign negativeKeywordsNewCampaign = new NegativeKeywordsNewCampaign();
                    negativeKeywordsNewCampaign.BlockType = "EXACT";
                    negativeKeywordsNewCampaign.NegativeKeyword = negativeOneOff.SearchTerm;
                    request.NegativeKeywordsNewCampaigns.Add(negativeKeywordsNewCampaign);

                    string negativeKeywordsEndpoint = "sp/negativeKeywords";
                    string mediaTypeNegativeKeywords = "application/vnd.spNegativeKeyword.v3+json";

                    AddNegativeKeywords addNegativeKeywords = new AddNegativeKeywords();

                    List<string> AdGroups = new List<string>();
                    AdGroups.Add(negativeOneOff.AdGroup);
                    var response = await addNegativeKeywords.AddTheseNegativeKeywords(negativeOneOff.CountryId, AdGroups, negativeOneOff.AzCampaignId, request, negativeKeywordsEndpoint, mediaTypeNegativeKeywords, auth);

                    if (response == "1")
                    {
                        SaveSummaryReportAction action = new SaveSummaryReportAction();
                        action.KeywordId = negativeOneOff.KeywordId;
                        action.keywordType = negativeOneOff.KeywordType;
                        action.AzCampaignId = negativeOneOff.AzCampaignId;
                        action.Negative = true;
                        action.CountryId = negativeOneOff.CountryId;
                        action.SearchTerm = negativeOneOff.SearchTerm;
                        action.AdGroup = negativeOneOff.AdGroup;
                        action.Product = false;

                        KeywordFunctions keywordFunctions = new KeywordFunctions();
                        action.keyword = await keywordFunctions.GetKeywordText(negativeOneOff.KeywordId, negativeOneOff.CountryId, negativeOneOff.Authorization.ClientId);

                        var functionResponse = await keywordFunctions.ApplyNegativeOneOff(action, negativeOneOff.Authorization.ClientId);

                        CountrySuccess countrySucces = new CountrySuccess();
                        countrySucces.CountryId = negativeOneOff.CountryId;
                        countrySucces.Success = true;

                        simpleResponse.CountrySuccess.Add(countrySucces);
                        return simpleResponse;
                    }
                    else
                    {
                        CountrySuccess countrySucces = new CountrySuccess();
                        countrySucces.CountryId = negativeOneOff.CountryId;
                        countrySucces.Success = false;

                        simpleResponse.CountrySuccess.Add(countrySucces);
                        return simpleResponse;
                    }
                }
            }
            catch(Exception ex)
            {
                simpleResponse.APIAuthorization.ErrorMessage = "Negative failed";
                return simpleResponse;
            }
         
        }
    }
}
