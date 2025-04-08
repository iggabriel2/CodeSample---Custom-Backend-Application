using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using Azure;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Save;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update;
using Configuration;
using Microsoft.Azure.Cosmos;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign
{
    public class CreateCampaign
    {
        public async Task<CampaignResponse> Create(CampaignRequest request)
        {
            //instantiate save class
            SaveData sd = new SaveData();

            //get token
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(request.Authorization);

            //get profile codes
            RetrieveReportData rrdCodes = new RetrieveReportData();
            request.Authorization.ClientProfileCodes = await rrdCodes.GetProfileCodes(request.Authorization.ClientId);

            //this holds our response
            CampaignResponse myResponse = new CampaignResponse();

            //handle if token fails or set to use
            if (auth.AccessToken == "Invalid" || auth.AccessToken == "Failed")
            {
                myResponse.APIAuthorization.ErrorMessage = "Token Failed";
                return myResponse;
            }
            else
            {
                request.Authorization.AccessToken = auth.AccessToken;
                request.Authorization.TokenExpirationTime = auth.TokenExpirationTime;
            }

            //set authorization in response
            myResponse.APIAuthorization = auth;


            //make campaigns in all countries
            foreach (CountrySpecificRules countryToCreate in request.CountryRules)
            {
                //store ad group ids
                List<string> autoAdGroupIds = new List<string>();
                List<string> broadAdGroupIds = new List<string>();
                List<string> phraseAdGroupIds = new List<string>();
                List<string> exactAdGroupIds = new List<string>();
                List<string> ProductAdGroups = new List<string>();
                List<string> allAdGroups = new List<string>();
                List<string> InvlaidKeywords = new List<string>();
                List<string> DuplicateKeywords = new List<string>();
                List<AdGroupPairs> adGroupPairs = new List<AdGroupPairs>();

                List<string> InvalidAsins = new List<string>();



                CountrySuccessOnCampaigns countrySuccess = new CountrySuccessOnCampaigns();

                //get clientProfileCode so I can make sure user is authorized
                ClientProfileCodes cpCode = new ClientProfileCodes();
                cpCode= request.Authorization.ClientProfileCodes.Where(x => x.CountryId == countryToCreate.CountryId).FirstOrDefault();

                if (cpCode != null && cpCode.CountryId != 0)
                {
                    try
                    {

                        //each section will have its own file under CreateCampaign


                        //create campaign
                        //for each endpoint
                        //basic api setup - CUSTOMIZE VALUES
                        string mediaTypeCreateCampaign = "application/vnd.spCampaign.v3+json";
                        string endPointCreateCampaign = "sp/campaigns";
                        CampaignCreation campaignCreation = new CampaignCreation();
                        var campaignSuccess = await campaignCreation.CreateThisCampaign(countryToCreate.CountryId, request, countryToCreate, endPointCreateCampaign, mediaTypeCreateCampaign, auth);
                        if (campaignSuccess == "2")
                        {
                            myResponse.APIAuthorization.ErrorMessage = "Campaign already exists.";

                            countrySuccess.CountryId = countryToCreate.CountryId;
                            countrySuccess.Success = false;
                        }
                        else if (campaignSuccess == "0")
                        {
                            LogError(request, countryToCreate.CountryId, "Failed to create campaign on line 72");
                            myResponse.APIAuthorization.ErrorMessage = "Failed to create at least one campaign.";

                            countrySuccess.CountryId = countryToCreate.CountryId;
                            countrySuccess.Success = false;
                        }
                        else
                        {
                            // make sure if I make the same call more than once, it won't duplicate
                            //campaigns - these are good. they will not duplicate.


                            //separate keyword lists and figure out how many ad groups we need by total keywords
                            double totalAdGroups = new double();
                            if (request.CampaignType == 1)
                            {
                                List<string> distinctKeywords = request.Keywords.Distinct().ToList();
                                totalAdGroups = Math.Ceiling((double)distinctKeywords.Count() / 1000);
                            }

                            if (request.CampaignUsageType > 1 && totalAdGroups == 0)
                            {
                                totalAdGroups = 1;
                            }


                            //create keyword and product ad groups
                            bool createAdGroupSucces = await CreateAdGroups(request, totalAdGroups, autoAdGroupIds, broadAdGroupIds, phraseAdGroupIds, exactAdGroupIds, allAdGroups, adGroupPairs, countryToCreate, campaignSuccess, auth, ProductAdGroups);
                            if (createAdGroupSucces == false)
                            {
                                LogError(request, countryToCreate.CountryId, "Failed to create ad groups on line 118");
                                myResponse.APIAuthorization.ErrorMessage += "| Failed to create at least one ad group";

                                countrySuccess.CountryId = countryToCreate.CountryId;
                                countrySuccess.Success = false;
                            }
                            else
                            {
                                //add product to all ad groups
                                string productEndpoint = "sp/productAds";
                                string mediaType = "application/vnd.spproductAd.v3+json";
                                AddProduct addProduct = new AddProduct();
                                string productAdded = await addProduct.AddThisProduct(countryToCreate.CountryId, campaignSuccess, allAdGroups, request, countryToCreate, productEndpoint, mediaType, auth);
                                if (productAdded == "0")
                                {
                                    LogError(request, countryToCreate.CountryId, "Failed to add products on line 134");
                                    myResponse.APIAuthorization.ErrorMessage += "| Failed to add at least one product";

                                    countrySuccess.CountryId = countryToCreate.CountryId;
                                    countrySuccess.Success = false;
                                }
                                else
                                {
                                    //add keywords to keyword ad group
                                    string keywordsAdded = "1";
                                    if (request.CampaignType == 1 && request.Keywords != null && request.Keywords.Count > 0)
                                    {
                                        string keywordsEndpoint = "sp/keywords";
                                        string mediaTypeKeywords = "application/vnd.spKeyword.v3+json";
                                        AddKeywords addKeywords = new AddKeywords();
                                        keywordsAdded = await addKeywords.AddTheseKeywords(countryToCreate.CountryId, campaignSuccess, broadAdGroupIds, phraseAdGroupIds, exactAdGroupIds, request, countryToCreate, keywordsEndpoint, mediaTypeKeywords, auth, InvlaidKeywords, DuplicateKeywords);
                                    }


                                    if (keywordsAdded == "0")
                                    {
                                        LogError(request, countryToCreate.CountryId, "Failed to add keywords on line 144");
                                        myResponse.APIAuthorization.ErrorMessage += "| Failed to add keywords";

                                        countrySuccess.CountryId = countryToCreate.CountryId;
                                        countrySuccess.Success = false;
                                    }
                                    else
                                    {
                                        string negativesAdded = "1";
                                        if (request.NegativeKeywordsNewCampaigns != null && request.NegativeKeywordsNewCampaigns.Count > 0)
                                        {
                                            //add negative keywords
                                            string negativeKeywordsEndpoint = "sp/negativeKeywords";
                                            string mediaTypeNegativeKeywords = "application/vnd.spNegativeKeyword.v3+json";
                                            AddNegativeKeywords addNegativeKeywords = new AddNegativeKeywords();

                                            List<string> CombinedAdGroupsForNegatives = new List<string>();
                                            CombinedAdGroupsForNegatives.AddRange(autoAdGroupIds);
                                            CombinedAdGroupsForNegatives.AddRange(broadAdGroupIds);
                                            CombinedAdGroupsForNegatives.AddRange(phraseAdGroupIds);
                                            CombinedAdGroupsForNegatives.AddRange(exactAdGroupIds);

                                            negativesAdded = await addNegativeKeywords.AddTheseNegativeKeywords(countryToCreate.CountryId, CombinedAdGroupsForNegatives, campaignSuccess, request, negativeKeywordsEndpoint, mediaTypeNegativeKeywords, auth);

                                        }

                                        if (negativesAdded == "0")
                                        {
                                            LogError(request, countryToCreate.CountryId, "Failed to add negatives on line 159");
                                            myResponse.APIAuthorization.ErrorMessage += "| Failed to add negatives";

                                            countrySuccess.CountryId = countryToCreate.CountryId;
                                            countrySuccess.Success = false;
                                        }
                                        else
                                        {
                                            //add product to sell to product targeting ad group
                                            string asinsAdded = "1";
                                            if (request.CampaignType == 1 && request.Asins != null && request.Asins.Count > 0)
                                            {
                                                //add products
                                                string asinsKeywordsEndpoint = "sp/targets";
                                                string mediaTypeAsins = "application/vnd.spTargetingClause.v3+json";
                                                AddAsins addAsins = new AddAsins();
                                                asinsAdded = await addAsins.AddTheseAsins(countryToCreate.CountryId, campaignSuccess, request, countryToCreate, ProductAdGroups[0], asinsKeywordsEndpoint, mediaTypeAsins, auth, InvalidAsins);
                                            }

                                            if (asinsAdded == "0")
                                            {
                                                LogError(request, countryToCreate.CountryId, "Failed to add asins to product target campaign on line 192");
                                                myResponse.APIAuthorization.ErrorMessage += "| Failed to add asins to product target campaign";

                                                countrySuccess.CountryId = countryToCreate.CountryId;
                                                countrySuccess.Success = false;
                                            }
                                            else
                                            {
                                                CampaignSave campaignToSave = new CampaignSave();
                                                campaignToSave.ProductId = request.ProductAsinsAndCampaignNames[0].ProductId;
                                                campaignToSave.CampaignName = request.ProductAsinsAndCampaignNames[0].CampaignName;
                                                campaignToSave.AZCampaignId = campaignSuccess;
                                                campaignToSave.CountryId = countryToCreate.CountryId;
                                                campaignToSave.Active = true;
                                                campaignToSave.AzClientId = request.Authorization.ClientId;
                                                campaignToSave.AzPortfolioId = countryToCreate.AzPortfolioId;
                                                campaignToSave.AzSpCampaignUsageType = request.CampaignUsageType;
                                                campaignToSave.GeneratedByUs = true;
                                                campaignToSave.Budget = countryToCreate.Budget;

                                                if (request.CountryRules[0].BiddingStrategy.ToLower() == "down")
                                                {
                                                    campaignToSave.DynamicBiddingStrategy = 1;
                                                }
                                                else if (request.CountryRules[0].BiddingStrategy.ToLower() == "updown")
                                                {
                                                    campaignToSave.DynamicBiddingStrategy = 2;
                                                }
                                                else
                                                {
                                                    campaignToSave.DynamicBiddingStrategy = 3;
                                                }

                                                if (request.CampaignType == 1)
                                                {
                                                    campaignToSave.TargetingType = "MANUAL";
                                                }
                                                else
                                                {
                                                    campaignToSave.TargetingType = "AUTO";
                                                }

                                                if (request.CampaignUsageType == 1)
                                                {
                                                    campaignToSave.AzSpPrimaryInUsageType = false;
                                                }
                                                else
                                                {
                                                    campaignToSave.AzSpPrimaryInUsageType = true;
                                                }

                                                var campaignSaved = await sd.SaveCampaign(campaignToSave);

                                                Database database = await Cosmos.cosmosInstance.CreateDatabaseIfNotExistsAsync(Cosmos.CosmosAzDb);
                                                Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(Cosmos.CosmosAdGroups, "/partitionKey");

                                                foreach (var adgroupPair in adGroupPairs)
                                                {
                                                    //this is saved here for batch processing. We only save t1 and performance for efficency.
                                                    if (campaignToSave.AzSpPrimaryInUsageType)
                                                    {
                                                        var adGroupSaved = await sd.SaveAdGroup(adgroupPair, request.Authorization.ClientId);
                                                    }

                                                    //this is saved here for use in other places
                                                    Entities.CampaignsAdGroups.AdGroupSnapshot item = new Entities.CampaignsAdGroups.AdGroupSnapshot();
                                                    item.adGroupId = Convert.ToInt64(adgroupPair.AzAdGroupId);
                                                    item.name = adgroupPair.AzAdGroupName;
                                                    item.campaignId = Convert.ToInt64(adgroupPair.AzSpCampaignId);
                                                    item.defaultBid = request.CountryRules[0].Bid;
                                                    item.state = "enabled";
                                                    item.ClientId = request.Authorization.ClientId.ToString();
                                                    item.CountryId = adgroupPair.CountryId;
                                                    item.partitionKey = request.Authorization.ClientId.ToString();
                                                    item.id = request.Authorization.ClientId.ToString() + "." + adgroupPair.CountryId.ToString() + "." + adgroupPair.AzSpCampaignId.ToString() + "." + adgroupPair.AzAdGroupId.ToString();

                                                    await container.UpsertItemAsync<Entities.CampaignsAdGroups.AdGroupSnapshot>(item, new PartitionKey(item.partitionKey));
                                                }


                                                //note success
                                                countrySuccess.CountryId = countryToCreate.CountryId;
                                                countrySuccess.Success = true;
                                                countrySuccess.RejectedKeywords = InvlaidKeywords;
                                                countrySuccess.DuplicateKeywords = DuplicateKeywords.Count();
                                                countrySuccess.InvalidAsins = InvalidAsins;
                                                countrySuccess.QapId = campaignSaved;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        myResponse.CountrySuccess.Add(countrySuccess);
                    }
                    catch (Exception ex)
                    {
                        LogError(request, countryToCreate.CountryId, ex.ToString());

                        //note success
                        countrySuccess.CountryId = countryToCreate.CountryId;
                        countrySuccess.Success = false;

                        myResponse.CountrySuccess.Add(countrySuccess);
                    }

                }
            }

            return myResponse;
        }

        private void LogError(CampaignRequest request, int CountryId, string ex)
        {
            Logging logging = new Logging();
            LogError logError = new LogError();
            logError.ErrorMessage = ex;
            logError.FailureMethod = "CreateCampaign";
            logError.ClientId = request.Authorization.ClientId;
            logError.Parameters = JsonSerializer.Serialize(request);
            logging.WriteToLog(logError);
        }

        private async Task<bool> CreateAdGroups(CampaignRequest request, double totalAdGroups, List<string> autoAdGroupIds, List<string> broadAdGroupIds, List<string> phraseAdGroupIds, List<string> exactAdGroupIds, List<string> allAdGroups, List<AdGroupPairs> adGroupPairs, CountrySpecificRules countryToCreate, string CampaignId, APIAuthorization auth, List<string> ProductAdGroups)
        {
            string endPoint = "sp/adGroups";
            string mediaType = "application/vnd.spAdGroup.v3+json";

            AdGroupCreation adGroupCreation = new AdGroupCreation();
            if (request.CampaignType == 1)
            {
                var broadPresent = request.KeywordTypes.Where(x => x.ToLower() == "broad").FirstOrDefault();
                if (!string.IsNullOrEmpty(broadPresent))
                {
                    //create broad ad groups
                    for (var x = 0; x < totalAdGroups; x++)
                    {
                        string adGroupNameKey;
                        if (totalAdGroups > 1)
                        {
                            adGroupNameKey = request.ProductAsinsAndCampaignNames[0].CampaignName + " (Keywords Broad Group " + (x + 1) + ")";
                        }
                        else
                        {
                            adGroupNameKey = request.ProductAsinsAndCampaignNames[0].CampaignName + " (Keywords Broad Group)";
                        }
                       
                        string adGroupId = await adGroupCreation.CreateThisAdGroup(countryToCreate.CountryId, CampaignId, adGroupNameKey, request, countryToCreate, endPoint, mediaType, auth);
                        
                        if (adGroupId != "0")
                        {
                            broadAdGroupIds.Add(adGroupId);
                            allAdGroups.Add(adGroupId);

                            AdGroupPairs adGroupPair = new AdGroupPairs();
                            adGroupPair.AzAdGroupName = adGroupNameKey;
                            adGroupPair.AzAdGroupId = adGroupId;
                            adGroupPair.AzSpCampaignId = CampaignId;
                            adGroupPair.AzAdGroupUsageType = 1;
                            adGroupPair.CountryId = countryToCreate.CountryId;
                            adGroupPair.ClientId = auth.ClientId;
                            adGroupPairs.Add(adGroupPair);
                        }
                      
                    }

                    if (broadAdGroupIds.Count() < totalAdGroups)
                    {
                        return false;
                    }
                }

                var phrasePresent = request.KeywordTypes.Where(x => x.ToLower() == "phrase").FirstOrDefault();
                if (!string.IsNullOrEmpty(phrasePresent))
                {
                    //create phrase ad groups
                    for (var x = 0; x < totalAdGroups; x++)
                    {
                        string adGroupNameKey;
                        if (totalAdGroups > 1)
                        {
                            adGroupNameKey = request.ProductAsinsAndCampaignNames[0].CampaignName + " (Keywords Phrase Group " + (x + 1) + ")";
                        }
                        else
                        {
                            adGroupNameKey = request.ProductAsinsAndCampaignNames[0].CampaignName + " (Keywords Phrase Group)";
                        }
                        string adGroupId = await adGroupCreation.CreateThisAdGroup(countryToCreate.CountryId, CampaignId, adGroupNameKey, request, countryToCreate, endPoint, mediaType, auth);

                        if (adGroupId != "0")
                        {
                            phraseAdGroupIds.Add(adGroupId);
                            allAdGroups.Add(adGroupId);

                            AdGroupPairs adGroupPair = new AdGroupPairs();
                            adGroupPair.AzAdGroupName = adGroupNameKey;
                            adGroupPair.AzAdGroupId = adGroupId;
                            adGroupPair.AzSpCampaignId = CampaignId;
                            adGroupPair.AzAdGroupUsageType = 2;
                            adGroupPair.CountryId = countryToCreate.CountryId;
                            adGroupPair.ClientId = auth.ClientId;
                            adGroupPairs.Add(adGroupPair);
                        }
                     
                    }

                    if (phraseAdGroupIds.Count() < totalAdGroups)
                    {
                        return false;
                    }
                }

                var exactPresent = request.KeywordTypes.Where(x => x.ToLower() == "exact").FirstOrDefault();
                if (!string.IsNullOrEmpty(exactPresent))
                {
                    //create exact ad groups
                    for (var x = 0; x < totalAdGroups; x++)
                    {
                        string adGroupNameKey;
                        if (totalAdGroups > 1)
                        {
                            adGroupNameKey = request.ProductAsinsAndCampaignNames[0].CampaignName + " (Keywords Exact Group " + (x + 1) + ")";
                        }
                        else
                        {
                            adGroupNameKey = request.ProductAsinsAndCampaignNames[0].CampaignName + " (Keywords Exact Group)";
                        }
                        string adGroupId = await adGroupCreation.CreateThisAdGroup(countryToCreate.CountryId, CampaignId, adGroupNameKey, request, countryToCreate, endPoint, mediaType, auth);

                        if (adGroupId != "0")
                        {
                            exactAdGroupIds.Add(adGroupId);
                            allAdGroups.Add(adGroupId);

                            AdGroupPairs adGroupPair = new AdGroupPairs();
                            adGroupPair.AzAdGroupName = adGroupNameKey;
                            adGroupPair.AzAdGroupId = adGroupId;
                            adGroupPair.AzSpCampaignId = CampaignId;
                            adGroupPair.AzAdGroupUsageType = 3;
                            adGroupPair.CountryId = countryToCreate.CountryId;
                            adGroupPair.ClientId = auth.ClientId;
                            adGroupPairs.Add(adGroupPair);
                        }
                       
                    }

                    if (exactAdGroupIds.Count() < totalAdGroups)
                    {
                        return false;
                    }
                }

                //add product ad group
                if ((request.Asins != null && request.Asins.Count > 0) || request.CampaignUsageType == 2 || request.CampaignUsageType == 3)
                {
                    string adGroupNameKeyProducts = request.ProductAsinsAndCampaignNames[0].CampaignName + " (Products)";
                    string adGroupIdProducts = await adGroupCreation.CreateThisAdGroup(countryToCreate.CountryId, CampaignId, adGroupNameKeyProducts, request, countryToCreate, endPoint, mediaType, auth);

                    if (adGroupIdProducts != "0")
                    {
                        ProductAdGroups.Add(adGroupIdProducts);
                        allAdGroups.Add(adGroupIdProducts);

                        AdGroupPairs adGroupPair = new AdGroupPairs();
                        adGroupPair.AzAdGroupName = adGroupNameKeyProducts;
                        adGroupPair.AzAdGroupId = adGroupIdProducts;
                        adGroupPair.AzSpCampaignId = CampaignId;
                        adGroupPair.AzAdGroupUsageType = 4;
                        adGroupPair.CountryId = countryToCreate.CountryId;
                        adGroupPair.ClientId = auth.ClientId;
                        adGroupPairs.Add(adGroupPair);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                string adGroupNameKey = request.ProductAsinsAndCampaignNames[0].CampaignName + "_Auto_Group";
                string adGroupId = await adGroupCreation.CreateThisAdGroup(countryToCreate.CountryId, CampaignId, adGroupNameKey, request, countryToCreate, endPoint, mediaType, auth);

                if (adGroupId != "0")
                {
                    autoAdGroupIds.Add(adGroupId);
                    allAdGroups.Add(adGroupId);

                    AdGroupPairs adGroupPair = new AdGroupPairs();
                    adGroupPair.AzAdGroupName = adGroupNameKey;
                    adGroupPair.AzAdGroupId = adGroupId;
                    adGroupPair.AzSpCampaignId = CampaignId;
                    adGroupPair.AzAdGroupUsageType = 5;
                    adGroupPair.CountryId = countryToCreate.CountryId;
                    adGroupPair.ClientId = auth.ClientId;
                    adGroupPairs.Add(adGroupPair);
                }
               

                if (adGroupId == "0" || string.IsNullOrEmpty(adGroupId))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
