using AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.D4Api;
using AdTool.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CampaignExtra
{
    public class SimpleAdKeyword
    {

        //REMEMBER THIS IS ONLY DESIGNED TO SUPPORT A SINGLE KEYWORD.
        public async Task<string> AddThisKeyword(KeywordRequestRoot keywordRequestMaster, int CountryId, List<ClientProfileCodes> clientProfileCodes, string EndPoint, string MediaType, APIAuthorization Auth, List<NewAdGroupIds> InvlaidKeywords, List<string> newAdGroupIds, List<NewAdGroupIds> AdGroupReference)
        {
            ClientProfileCodes profileCode = clientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            string responseValue = "1";

            string serlializedJson = JsonSerializer.Serialize(keywordRequestMaster);

            //call api here
            await System.Threading.Tasks.Task.Delay(1000);

            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            KeywordResponseRoot keywordResponse = new KeywordResponseRoot();

            //new ad group id holder in case we need it
            List<NewAdGroupIds> newAdGroupIdsHolder = new List<NewAdGroupIds>();
            
            if (responseMessage.IsSuccessStatusCode)
            {
                keywordResponse = await JsonSerializer.DeserializeAsync<KeywordResponseRoot>(responseMessage.Content.ReadAsStream());

                if (keywordResponse.keywords.error != null && keywordResponse.keywords.error.Count > 0)
                {
                    KeywordRequestRoot keywordRequest2 = new KeywordRequestRoot();

                    //find all the keywords that fell out of range and add them to a new list
                    foreach (var invalidKeywordId in keywordResponse.keywords.error)
                    {
                        var quota = invalidKeywordId.errors.Where(x => x.errorType.ToLower() == "rangeerror").FirstOrDefault();
                        var alreadyExists = invalidKeywordId.errors.Where(x => x.errorType.ToLower() == "duplicatevalueerror").FirstOrDefault();

                        if (quota != null)
                        {
                            APIKeyword apiKeyword = keywordRequestMaster.keywords.ElementAt(invalidKeywordId.index);
                            //apiKeyword.adGroupId = newAdGroupId;
                            keywordRequest2.keywords.Add(apiKeyword);
                        }
                        else if (alreadyExists != null)
                        {
                            //nothing to do. We won't resend it and we won't reject it. It already exists.
                        }
                        else
                        {
                            var rejectedKeyword = keywordRequestMaster.keywords.ElementAt(invalidKeywordId.index);
                            NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                            invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                            invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                            invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                            InvlaidKeywords.Add(invalidKeyword);
                        }
                    }

                    if (keywordRequest2 == null || keywordRequest2.keywords.Count == 0)
                    {
                        return "1";
                    }

                    //get the distinct ad groups that failed
                    newAdGroupIdsHolder =  (from t in keywordRequest2.keywords
                                                      group t by new { t.campaignId, t.adGroupId, t.matchType, t.bid } into grp
                                                      select new NewAdGroupIds
                                                      {
                                                          CampaignId = grp.Key.campaignId,
                                                          OldAdGroupId = grp.Key.adGroupId,
                                                          MatchType = grp.Key.matchType,
                                                          Bid = grp.Key.bid
                                                      }).ToList(); 

                    //remake ad groups
                    foreach(var failedAdGroup in newAdGroupIdsHolder)
                    {
                        string newAdGroupName = "";
                        string newAdGroupId = "";

                        //get the existing ad group name, see if the last string value after the last space is a number and increase by 1 or add " 2"
                        AdGroupUtils adGroupUtils = new AdGroupUtils();
                        newAdGroupName = await adGroupUtils.GetNewAdGroupName(failedAdGroup.CampaignId, CountryId, Auth.ClientId, failedAdGroup.OldAdGroupId);

                        //set the ad group usage type
                        int adGroupUsageTypeHere = 0;

                        if (failedAdGroup.MatchType.ToLower() == "broad")
                            adGroupUsageTypeHere = 1;
                        else if (failedAdGroup.MatchType.ToLower() == "phrase")
                            adGroupUsageTypeHere = 2;
                        else if (failedAdGroup.MatchType.ToLower() == "exact")
                            adGroupUsageTypeHere = 3;

                        int productId = AdGroupReference.Where(x => x.CampaignId == failedAdGroup.CampaignId && x.OldAdGroupId == failedAdGroup.OldAdGroupId).FirstOrDefault().ProductId;

                        //make another ad group and resend
                        ProcessAdditionalAdGroups processAdditionalAdGroups = new ProcessAdditionalAdGroups();
                        newAdGroupId = await processAdditionalAdGroups.CreateAdGroup(failedAdGroup.CampaignId, failedAdGroup.OldAdGroupId, adGroupUsageTypeHere, CountryId, failedAdGroup.CampaignId, newAdGroupName, clientProfileCodes, failedAdGroup.Bid, Auth, productId);

                        if (string.IsNullOrEmpty(newAdGroupId))
                        {
                            foreach(var adGroupRef in AdGroupReference)
                            {
                                if (adGroupRef.OldAdGroupId == failedAdGroup.OldAdGroupId)
                                {
                                    adGroupRef.NewAdGroupId = newAdGroupId;
                                    break;
                                }
                            }
                            //remove these from resending
                            keywordRequest2.keywords.RemoveAll(x => x.adGroupId == failedAdGroup.OldAdGroupId);

                            //failed to make ad group
                            foreach (var invalidKeywordId in keywordResponse.keywords.error)
                            {
                                var rejectedKeyword = keywordRequestMaster.keywords.ElementAt(invalidKeywordId.index);
                                NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                                invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                                invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                                InvlaidKeywords.Add(invalidKeyword);
                            }
                        }
                        else
                        {
                            //update ad groups
                            foreach(var keywordRequest in keywordRequest2.keywords)
                            {
                                if (keywordRequest.adGroupId == failedAdGroup.OldAdGroupId)
                                {
                                    keywordRequest.adGroupId = newAdGroupId;
                                }
                            }

                            //add this so I know to refresh the list on ProcessReportLogic
                            newAdGroupIds.Add(newAdGroupId);
                        }
                    }
                   
                    //resend request
                    string serlializedJson2 = JsonSerializer.Serialize(keywordRequest2);

                    //call api here
                    await System.Threading.Tasks.Task.Delay(1000);

                    HttpResponseMessage responseMessage2 = new HttpResponseMessage();

                    if (keywordRequest2 != null && keywordRequest2.keywords != null && keywordRequest2.keywords.Count > 0)
                    {
                        responseMessage2 = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson2);
                    }
                    else
                    {
                        responseMessage2.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    }

                    KeywordResponseRoot keywordResponse2 = new KeywordResponseRoot();
                    if (!responseMessage2.IsSuccessStatusCode)
                    {
                        //all of these failed
                        foreach (var invalidKeywordId in keywordRequest2.keywords)
                        {
                            NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                            invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == invalidKeywordId.adGroupId).FirstOrDefault().OldAdGroupId;
                            invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                            invalidKeyword.KeywordText = invalidKeywordId.keywordText;
                            InvlaidKeywords.Add(invalidKeyword);
                        }

                        return "0";
                    }
                    else
                    {
                        keywordResponse2 = await JsonSerializer.DeserializeAsync<KeywordResponseRoot>(responseMessage2.Content.ReadAsStream());

                        if (keywordResponse2 != null)
                        {
                            if (keywordResponse2.keywords != null && keywordResponse2.keywords.error != null && keywordResponse2.keywords.error.Count > 0)
                            {
                                foreach (var invalidKeywordId in keywordResponse2.keywords.error)
                                {
                                    var rejectedKeyword = keywordRequest2.keywords.ElementAt(invalidKeywordId.index);
                                    NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                    invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == rejectedKeyword.adGroupId).FirstOrDefault().OldAdGroupId;
                                    invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                                    invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                                    InvlaidKeywords.Add(invalidKeyword);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                if (responseMessage.IsSuccessStatusCode)
                {
                    keywordResponse = await JsonSerializer.DeserializeAsync<KeywordResponseRoot>(responseMessage.Content.ReadAsStream());

                    if (keywordResponse.keywords.error != null && keywordResponse.keywords.error.Count > 0)
                    {
                        KeywordRequestRoot keywordRequest2 = new KeywordRequestRoot();

                        //find all the keywords that fell out of range and add them to a new list
                        foreach (var invalidKeywordId in keywordResponse.keywords.error)
                        {
                            var quota = invalidKeywordId.errors.Where(x => x.errorType.ToLower() == "rangeerror").FirstOrDefault();
                            var alreadyExists = invalidKeywordId.errors.Where(x => x.errorType.ToLower() == "duplicatevalueerror").FirstOrDefault();

                            if (quota != null)
                            {
                                APIKeyword apiKeyword = keywordRequestMaster.keywords.ElementAt(invalidKeywordId.index);
                                //apiKeyword.adGroupId = newAdGroupId;
                                keywordRequest2.keywords.Add(apiKeyword);
                            }
                            else if (alreadyExists != null)
                            {
                                //nothing to do. We won't resend it and we won't reject it. It already exists.
                            }
                            else
                            {
                                var rejectedKeyword = keywordRequestMaster.keywords.ElementAt(invalidKeywordId.index);
                                NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                                invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                                invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                                InvlaidKeywords.Add(invalidKeyword);
                            }
                        }

                        if (keywordRequest2 == null || keywordRequest2.keywords.Count == 0)
                        {
                            return "1";
                        }

                        //get the distinct ad groups that failed
                        newAdGroupIdsHolder = (from t in keywordRequest2.keywords
                                               group t by new { t.campaignId, t.adGroupId, t.matchType, t.bid } into grp
                                               select new NewAdGroupIds
                                               {
                                                   CampaignId = grp.Key.campaignId,
                                                   OldAdGroupId = grp.Key.adGroupId,
                                                   MatchType = grp.Key.matchType,
                                                   Bid = grp.Key.bid
                                               }).ToList();

                        //remake ad groups
                        foreach (var failedAdGroup in newAdGroupIdsHolder)
                        {
                            string newAdGroupName = "";
                            string newAdGroupId = "";

                            //get the existing ad group name, see if the last string value after the last space is a number and increase by 1 or add " 2"
                            AdGroupUtils adGroupUtils = new AdGroupUtils();
                            newAdGroupName = await adGroupUtils.GetNewAdGroupName(failedAdGroup.CampaignId, CountryId, Auth.ClientId, failedAdGroup.OldAdGroupId);

                            //set the ad group usage type
                            int adGroupUsageTypeHere = 0;

                            if (failedAdGroup.MatchType.ToLower() == "broad")
                                adGroupUsageTypeHere = 1;
                            else if (failedAdGroup.MatchType.ToLower() == "phrase")
                                adGroupUsageTypeHere = 2;
                            else if (failedAdGroup.MatchType.ToLower() == "exact")
                                adGroupUsageTypeHere = 3;

                            int productId = AdGroupReference.Where(x => x.CampaignId == failedAdGroup.CampaignId && x.OldAdGroupId == failedAdGroup.OldAdGroupId).FirstOrDefault().ProductId;

                            //make another ad group and resend
                            ProcessAdditionalAdGroups processAdditionalAdGroups = new ProcessAdditionalAdGroups();
                            newAdGroupId = await processAdditionalAdGroups.CreateAdGroup(failedAdGroup.CampaignId, failedAdGroup.OldAdGroupId, adGroupUsageTypeHere, CountryId, failedAdGroup.CampaignId, newAdGroupName, clientProfileCodes, failedAdGroup.Bid, Auth, productId);

                            if (string.IsNullOrEmpty(newAdGroupId))
                            {
                                foreach (var adGroupRef in AdGroupReference)
                                {
                                    if (adGroupRef.OldAdGroupId == failedAdGroup.OldAdGroupId)
                                    {
                                        adGroupRef.NewAdGroupId = newAdGroupId;
                                        break;
                                    }
                                }

                                //remove these from resending
                                keywordRequest2.keywords.RemoveAll(x => x.adGroupId == failedAdGroup.OldAdGroupId);

                                //failed to make ad group
                                foreach (var invalidKeywordId in keywordResponse.keywords.error)
                                {
                                    var rejectedKeyword = keywordRequestMaster.keywords.ElementAt(invalidKeywordId.index);
                                    NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                    invalidKeyword.OldAdGroupId = rejectedKeyword.adGroupId;
                                    invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                                    invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                                    InvlaidKeywords.Add(invalidKeyword);
                                }
                            }
                            else
                            {
                                //update ad groups
                                foreach (var keywordRequest in keywordRequest2.keywords)
                                {
                                    if (keywordRequest.adGroupId == failedAdGroup.OldAdGroupId)
                                    {
                                        keywordRequest.adGroupId = newAdGroupId;
                                    }
                                }

                                //add this so I know to refresh the list on ProcessReportLogic
                                newAdGroupIds.Add(newAdGroupId);
                            }
                        }

                        //resend request
                        string serlializedJson2 = JsonSerializer.Serialize(keywordRequest2);

                        //call api here
                        await System.Threading.Tasks.Task.Delay(1000);

                        HttpResponseMessage responseMessage2 = new HttpResponseMessage();

                        if (keywordRequest2 != null && keywordRequest2.keywords != null && keywordRequest2.keywords.Count > 0)
                        {
                            responseMessage2 = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson2);
                        }
                        else
                        {
                            responseMessage2.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        }

                        KeywordResponseRoot keywordResponse2 = new KeywordResponseRoot();
                        if (!responseMessage2.IsSuccessStatusCode)
                        {
                            //all of these failed
                            foreach (var invalidKeywordId in keywordRequest2.keywords)
                            {
                                NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                invalidKeyword.OldAdGroupId = invalidKeywordId.adGroupId;
                                invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                                invalidKeyword.KeywordText = invalidKeywordId.keywordText;
                                InvlaidKeywords.Add(invalidKeyword);
                            }

                            return "0";
                        }
                        else
                        {
                            keywordResponse2 = await JsonSerializer.DeserializeAsync<KeywordResponseRoot>(responseMessage2.Content.ReadAsStream());

                            if (keywordResponse2 != null)
                            {
                                if (keywordResponse2.keywords != null && keywordResponse2.keywords.error != null && keywordResponse2.keywords.error.Count > 0)
                                {
                                    foreach (var invalidKeywordId in keywordResponse2.keywords.error)
                                    {
                                        var rejectedKeyword = keywordRequest2.keywords.ElementAt(invalidKeywordId.index);
                                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                        invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == rejectedKeyword.adGroupId).FirstOrDefault().OldAdGroupId;
                                        invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                                        invalidKeyword.KeywordText = rejectedKeyword.keywordText;
                                        InvlaidKeywords.Add(invalidKeyword);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var invalidKeywordId in keywordRequestMaster.keywords)
                    {
                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                        invalidKeyword.OldAdGroupId = invalidKeywordId.adGroupId;
                        invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                        invalidKeyword.KeywordText = invalidKeywordId.keywordText;
                        InvlaidKeywords.Add(invalidKeyword);
                    }

                    return "0";
                }
            }

            return responseValue;
        }
    }
}
