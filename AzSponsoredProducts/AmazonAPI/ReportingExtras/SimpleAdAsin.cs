using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.AsinError;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdTool.Entities.D4Api;
using AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords;
using System.Security.Cryptography;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using System.Text.Json;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CampaignExtra
{
    public class SimpleAdAsin
    {
        //REMEMBER THIS IS ONLY DESIGNED TO SUPPORT A SINGLE ASIN.
        public async Task<string> AddThisAsin(ProductTargetRequestRoot productTargetRequestRoot, int CountryId, List<ClientProfileCodes> clientProfileCodes, string EndPoint, string MediaType, APIAuthorization Auth, List<NewAdGroupIds> InvlaidKeywords, List<string> newAdGroupIds, List<NewAdGroupIds> AdGroupReference)
        {
            ClientProfileCodes profileCode = clientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            //make object
            string serlializedJson = JsonSerializer.Serialize(productTargetRequestRoot);

            ProductTargetResponseRoot myResponse = new ProductTargetResponseRoot();

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            //new ad group id holder in case we need it
            List<NewAdGroupIds> newAdGroupIdsHolder = new List<NewAdGroupIds>();

            if (responseMessage.IsSuccessStatusCode)
            {
                myResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetResponseRoot>(responseMessage.Content.ReadAsStream());

                if (myResponse.targetingClauses.error != null && myResponse.targetingClauses.error.Count > 0)
                {
                    ProductTargetRequestRoot productTargetRequestRoot2 = new ProductTargetRequestRoot();
                    List<TargetingClause> targetingList = new List<TargetingClause>();

                    foreach (AsinErrorRoot invalidAsin in myResponse.targetingClauses.error)
                    {
                        var quota = invalidAsin.errors.Where(x => x.errorType.ToLower() == "rangeerror").FirstOrDefault();
                        var alreadyExists = invalidAsin.errors.Where(x => x.errorType.ToLower() == "duplicatevalueerror").FirstOrDefault();

                        if (quota != null)
                        {
                            TargetingClause targetingClause = productTargetRequestRoot.targetingClauses.ElementAt(invalidAsin.index);
                            targetingList.Add(targetingClause);
                        }
                        else if (alreadyExists != null)
                        {
                            //nothing to do. We won't resend it and we won't reject it. It already exists.
                        }
                        else
                        {
                            var rejectedAsin = productTargetRequestRoot.targetingClauses.ElementAt(invalidAsin.index);
                            NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                            invalidKeyword.OldAdGroupId = rejectedAsin.adGroupId;
                            invalidKeyword.CampaignId = rejectedAsin.campaignId;
                            invalidKeyword.KeywordText = rejectedAsin.expression[0].value;
                            InvlaidKeywords.Add(invalidKeyword);
                        }

                    }

                    if (targetingList == null  || targetingList.Count == 0)
                    {
                        return "1";
                    }

                    productTargetRequestRoot2.targetingClauses = targetingList;

                    //get the distinct ad groups that failed
                    newAdGroupIdsHolder = (from t in productTargetRequestRoot2.targetingClauses
                                           group t by new { t.campaignId, t.adGroupId, t.bid } into grp
                                           select new NewAdGroupIds
                                           {
                                               CampaignId = grp.Key.campaignId,
                                               OldAdGroupId = grp.Key.adGroupId,
                                               Bid = grp.Key.bid
                                           }).ToList();

                    foreach (var failedAdGroup in newAdGroupIdsHolder)
                    {
                        string newAdGroupName = "";
                        string newAdGroupId = "";

                        //get the existing ad group name, see if the last string value after the last space is a number and increase by 1 or add " 2"
                        AdGroupUtils adGroupUtils = new AdGroupUtils();
                        newAdGroupName = await adGroupUtils.GetNewAdGroupName(failedAdGroup.CampaignId, CountryId, Auth.ClientId, failedAdGroup.OldAdGroupId);

                        //set the ad group usage type
                        int adGroupUsageTypeHere = 4;

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
                            productTargetRequestRoot2.targetingClauses.RemoveAll(x => x.adGroupId == failedAdGroup.OldAdGroupId);

                            //failed to make ad group
                            foreach (var invalidId in myResponse.targetingClauses.error)
                            {
                                var rejectedAsin = productTargetRequestRoot.targetingClauses.ElementAt(invalidId.index);
                                NewAdGroupIds invalidAsin = new NewAdGroupIds();
                                invalidAsin.OldAdGroupId = rejectedAsin.adGroupId;
                                invalidAsin.CampaignId = rejectedAsin.campaignId;
                                invalidAsin.KeywordText = rejectedAsin.expression[0].value;
                                InvlaidKeywords.Add(invalidAsin);
                            }
                        }
                        else
                        {
                            //update ad groups
                            foreach (var targetingClause in productTargetRequestRoot2.targetingClauses)
                            {
                                if (targetingClause.adGroupId == failedAdGroup.OldAdGroupId)
                                {
                                    targetingClause.adGroupId = newAdGroupId;
                                }
                            }

                            //add this so I know to refresh the list on ProcessReportLogic
                            newAdGroupIds.Add(newAdGroupId);
                        }
                    }

                    //resend request
                    string serlializedJson2 = JsonSerializer.Serialize(productTargetRequestRoot2);

                    //call api here
                    await System.Threading.Tasks.Task.Delay(1000);

                    HttpResponseMessage responseMessage2 = new HttpResponseMessage();


                    if (productTargetRequestRoot2 != null && productTargetRequestRoot2.targetingClauses != null && productTargetRequestRoot2.targetingClauses.Count > 0)
                    {
                        responseMessage2 = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson2);
                    }
                    else
                    {
                        responseMessage2.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    }

                    ProductTargetResponseRoot productTargetResponseRoot2 = new ProductTargetResponseRoot();
                    if (!responseMessage2.IsSuccessStatusCode)
                    {
                        //all of these failed
                        foreach (var invalidKeywordId in productTargetRequestRoot2.targetingClauses)
                        {
                            NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                            invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == invalidKeywordId.adGroupId).FirstOrDefault().OldAdGroupId;
                            invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                            invalidKeyword.KeywordText = invalidKeywordId.expression[0].value;
                            InvlaidKeywords.Add(invalidKeyword);
                        }

                        return "0";
                    }
                    else
                    {
                        myResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetResponseRoot>(responseMessage2.Content.ReadAsStream());

                        if (myResponse != null)
                        {
                            if (myResponse.targetingClauses != null && myResponse.targetingClauses.error != null && myResponse.targetingClauses.error.Count > 0)
                            {
                                foreach (var invalidKeywordId in myResponse.targetingClauses.error)
                                {
                                    var rejectedKeyword = productTargetRequestRoot2.targetingClauses.ElementAt(invalidKeywordId.index);
                                    NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                    invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == rejectedKeyword.adGroupId).FirstOrDefault().OldAdGroupId;
                                    invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                                    invalidKeyword.KeywordText = rejectedKeyword.expression[0].value;
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
                    myResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetResponseRoot>(responseMessage.Content.ReadAsStream());

                    if (myResponse.targetingClauses.error != null && myResponse.targetingClauses.error.Count > 0)
                    {
                        ProductTargetRequestRoot productTargetRequestRoot2 = new ProductTargetRequestRoot();
                        List<TargetingClause> targetingList = new List<TargetingClause>();

                        foreach (AsinErrorRoot invalidAsin in myResponse.targetingClauses.error)
                        {
                            var quota = invalidAsin.errors.Where(x => x.errorType.ToLower() == "rangeerror").FirstOrDefault();
                            var alreadyExists = invalidAsin.errors.Where(x => x.errorType.ToLower() == "duplicatevalueerror").FirstOrDefault();

                            if (quota != null)
                            {
                                TargetingClause targetingClause = productTargetRequestRoot.targetingClauses.ElementAt(invalidAsin.index);
                                targetingList.Add(targetingClause);
                            }
                            else if (alreadyExists != null)
                            {
                                //nothing to do. We won't resend it and we won't reject it. It already exists.
                            }
                            else
                            {
                                var rejectedAsin = productTargetRequestRoot2.targetingClauses.ElementAt(invalidAsin.index);
                                NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == rejectedAsin.adGroupId).FirstOrDefault().OldAdGroupId;
                                invalidKeyword.CampaignId = rejectedAsin.campaignId;
                                invalidKeyword.KeywordText = rejectedAsin.expression[0].value;
                                InvlaidKeywords.Add(invalidKeyword);
                            }

                        }

                        if (targetingList == null || targetingList.Count == 0)
                        {
                            return "1";
                        }

                        productTargetRequestRoot2.targetingClauses = targetingList;

                        //get the distinct ad groups that failed
                        newAdGroupIdsHolder = (from t in productTargetRequestRoot.targetingClauses
                                               group t by new { t.campaignId, t.adGroupId, t.bid } into grp
                                               select new NewAdGroupIds
                                               {
                                                   CampaignId = grp.Key.campaignId,
                                                   OldAdGroupId = grp.Key.adGroupId,
                                                   Bid = grp.Key.bid
                                               }).ToList();

                        foreach (var failedAdGroup in newAdGroupIdsHolder)
                        {
                            string newAdGroupName = "";
                            string newAdGroupId = "";

                            //get the existing ad group name, see if the last string value after the last space is a number and increase by 1 or add " 2"
                            AdGroupUtils adGroupUtils = new AdGroupUtils();
                            newAdGroupName = await adGroupUtils.GetNewAdGroupName(failedAdGroup.CampaignId, CountryId, Auth.ClientId, failedAdGroup.OldAdGroupId);

                            //set the ad group usage type
                            int adGroupUsageTypeHere = 4;

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
                                productTargetRequestRoot2.targetingClauses.RemoveAll(x => x.adGroupId == failedAdGroup.OldAdGroupId);

                                //failed to make ad group
                                foreach (var invalidId in myResponse.targetingClauses.error)
                                {
                                    var rejectedAsin = productTargetRequestRoot.targetingClauses.ElementAt(invalidId.index);
                                    NewAdGroupIds invalidAsin = new NewAdGroupIds();
                                    invalidAsin.OldAdGroupId = rejectedAsin.adGroupId;
                                    invalidAsin.CampaignId = rejectedAsin.campaignId;
                                    invalidAsin.KeywordText = rejectedAsin.expression[0].value;
                                    InvlaidKeywords.Add(invalidAsin);
                                }
                            }
                            else
                            {
                                //update ad groups
                                foreach (var targetingClause in productTargetRequestRoot2.targetingClauses)
                                {
                                    if (targetingClause.adGroupId == failedAdGroup.OldAdGroupId)
                                    {
                                        targetingClause.adGroupId = newAdGroupId;
                                    }
                                }

                                //add this so I know to refresh the list on ProcessReportLogic
                                newAdGroupIds.Add(newAdGroupId);
                            }
                        }

                        //resend request
                        string serlializedJson2 = JsonSerializer.Serialize(productTargetRequestRoot2);

                        //call api here
                        await System.Threading.Tasks.Task.Delay(1000);

                        HttpResponseMessage responseMessage2 = new HttpResponseMessage();

                        if (productTargetRequestRoot2 != null && productTargetRequestRoot2.targetingClauses != null && productTargetRequestRoot2.targetingClauses.Count > 0)
                        {
                            responseMessage2 = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson2);
                        }
                        else
                        {
                            responseMessage2.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        }
                       

                        ProductTargetResponseRoot productTargetResponseRoot2 = new ProductTargetResponseRoot();
                        if (!responseMessage2.IsSuccessStatusCode)
                        {
                            //all of these failed
                            foreach (var invalidKeywordId in productTargetRequestRoot2.targetingClauses)
                            {
                                NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == invalidKeywordId.adGroupId).FirstOrDefault().OldAdGroupId;
                                invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                                invalidKeyword.KeywordText = invalidKeywordId.expression[0].value;
                                InvlaidKeywords.Add(invalidKeyword);
                            }

                            return "0";
                        }
                        else
                        {
                            myResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetResponseRoot>(responseMessage2.Content.ReadAsStream());

                            if (myResponse != null)
                            {
                                if (myResponse.targetingClauses != null && myResponse.targetingClauses.error != null && myResponse.targetingClauses.error.Count > 0)
                                {
                                    foreach (var invalidKeywordId in myResponse.targetingClauses.error)
                                    {
                                        var rejectedKeyword = productTargetRequestRoot2.targetingClauses.ElementAt(invalidKeywordId.index);
                                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                                        invalidKeyword.OldAdGroupId = AdGroupReference.Where(x => x.NewAdGroupId == rejectedKeyword.adGroupId).FirstOrDefault().OldAdGroupId;
                                        invalidKeyword.CampaignId = rejectedKeyword.campaignId;
                                        invalidKeyword.KeywordText = rejectedKeyword.expression[0].value;
                                        InvlaidKeywords.Add(invalidKeyword);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var invalidKeywordId in productTargetRequestRoot.targetingClauses)
                    {
                        NewAdGroupIds invalidKeyword = new NewAdGroupIds();
                        invalidKeyword.OldAdGroupId = invalidKeywordId.adGroupId;
                        invalidKeyword.CampaignId = invalidKeywordId.campaignId;
                        invalidKeyword.KeywordText = invalidKeywordId.expression[0].value;
                        InvlaidKeywords.Add(invalidKeyword);
                    }

                    return "0";
                }
            }

            return "1";

        }
    }
}
