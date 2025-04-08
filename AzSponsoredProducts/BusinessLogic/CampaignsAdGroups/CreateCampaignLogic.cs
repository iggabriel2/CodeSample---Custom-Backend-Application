using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign;
using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.Data;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Campaigns
{
    public class CreateCampaignLogic
    {
        public async Task<CampaignResponse> CreateCampaign(CampaignRequest myRequest)
        {
            CampaignResponse campaignResponse = new CampaignResponse();

            try
            {
                

                if (myRequest.ProductAsinsAndCampaignNames[0].CampaignName.ToLower().Contains("|split_k|"))
                {
                    //pull out keywords
                    List<string> pulledKeywords = new List<string>();
                    pulledKeywords.AddRange(myRequest.Keywords);
                    myRequest.Keywords.Clear();


                    string splitCount = myRequest.ProductAsinsAndCampaignNames[0].CampaignName.ToLower().Substring(myRequest.ProductAsinsAndCampaignNames[0].CampaignName.ToLower().IndexOf("|split_k|") + 9).Trim();
                    int finalSplitCount = 5000;

                    try
                    {
                        finalSplitCount = Convert.ToInt32(splitCount);
                    }
                    catch(Exception ex)
                    {
                        finalSplitCount = 5000;
                    }

                    int keywordCount = pulledKeywords.Count();

                    //max keywords per campaign
                    double totalCampaigns = Math.Ceiling((double)keywordCount / finalSplitCount);

                    string mainCampaignName = myRequest.ProductAsinsAndCampaignNames[0].CampaignName;


                    for (var x = 0; x < totalCampaigns; x++)
                    {
                        int recordsToSkip = x * finalSplitCount;
                        List<string> keywordsToAdd = pulledKeywords.Skip(recordsToSkip).Take(finalSplitCount).ToList();
                        myRequest.Keywords.AddRange(keywordsToAdd);
                        myRequest.ProductAsinsAndCampaignNames[0].CampaignName = mainCampaignName + " - " + (x + 1).ToString();

                        campaignResponse = await ActualCampaignCreate(myRequest);

                        myRequest.Keywords.Clear();
                        await System.Threading.Tasks.Task.Delay(1000);
                    }
                }
                else if (myRequest.ProductAsinsAndCampaignNames[0].CampaignName.ToLower().Contains("|split_a|"))
                {
                    //pull out keywords
                    List<string> pullledAsins = new List<string>();
                    pullledAsins.AddRange(myRequest.Asins);
                    myRequest.Asins.Clear();


                    string splitCount = myRequest.ProductAsinsAndCampaignNames[0].CampaignName.ToLower().Substring(myRequest.ProductAsinsAndCampaignNames[0].CampaignName.ToLower().IndexOf("|split_a|") + 9).Trim();
                    int finalSplitCount = 5000;

                    try
                    {
                        finalSplitCount = Convert.ToInt32(splitCount);
                    }
                    catch (Exception ex)
                    {
                        finalSplitCount = 5000;
                    }

                    int asinCount = pullledAsins.Count();

                    //max keywords per campaign
                    double totalCampaigns = Math.Ceiling((double)asinCount / finalSplitCount);

                    string mainCampaignName = myRequest.ProductAsinsAndCampaignNames[0].CampaignName;


                    for (var x = 0; x < totalCampaigns; x++)
                    {
                        int recordsToSkip = x * finalSplitCount;
                        List<string> asinstoadd = pullledAsins.Skip(recordsToSkip).Take(finalSplitCount).ToList();
                        myRequest.Asins.AddRange(asinstoadd);
                        myRequest.ProductAsinsAndCampaignNames[0].CampaignName = mainCampaignName + " - " + (x + 1).ToString();

                        campaignResponse = await ActualCampaignCreate(myRequest);

                        myRequest.Asins.Clear();
                        await System.Threading.Tasks.Task.Delay(1000);
                    }
                }
                else
                {
                    campaignResponse = await ActualCampaignCreate(myRequest);
                }
             


                return campaignResponse;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreateCampaignLogic";
                logError.ClientId = myRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(myRequest);
                await logging.WriteToLog(logError);

                campaignResponse.APIAuthorization.ErrorMessage = "Errored on CreateCampaign";
                return campaignResponse;
            }
        }

        public async Task<CampaignResponse> ActualCampaignCreate(CampaignRequest myRequest)
        {
            try
            {
                CreateCampaign createCampaign = new CreateCampaign();
                CampaignResponse campaignResponse = new CampaignResponse();

                campaignResponse = await createCampaign.Create(myRequest);

                //if we get an error, clear the token and try once more
                if (!string.IsNullOrEmpty(campaignResponse.APIAuthorization.ErrorMessage))
                {
                    myRequest.Authorization.AccessToken = "";

                    //clear token and try again
                    campaignResponse = await createCampaign.Create(myRequest);
                }

                return campaignResponse;
            }
            catch(Exception ex)
            {
                throw;
            }
          
        }
    }
}
