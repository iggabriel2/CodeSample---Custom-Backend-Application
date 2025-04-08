using AdTool.AzSponsoredProducts.AmazonAPI.CampaignExtra;
using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign;
using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.Data;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class ProcessAdditionalAdGroups
    {
        public async Task<string> CreateAdGroup(string ExistingCampaignId, string ExistingAdGroupId, int CampaignUsageType, int CountryId, string CampaignID, string AdGroupName, List<ClientProfileCodes> ClientProfileCodes, decimal Bid, APIAuthorization Auth, int ProductId)
        {
            try
            {
                //make items ad group expects and populate only those values needed
                CampaignRequest campaignRequest = new CampaignRequest();
                campaignRequest.Authorization.ClientProfileCodes = ClientProfileCodes;

                CountrySpecificRules countryToCreate = new CountrySpecificRules();
                countryToCreate.Bid = Bid;

                //make new ad group
                CreateSimpleAdGroup createSimpleAdGroup = new CreateSimpleAdGroup();
                var adgroupId = await createSimpleAdGroup.CreateAdGroup(CountryId, CampaignID, AdGroupName, campaignRequest, countryToCreate, Auth);

                if (!string.IsNullOrEmpty(adgroupId))
                {
                    AdGroupPairs adGroupPair = new AdGroupPairs();
                    adGroupPair.AzAdGroupName = AdGroupName;
                    adGroupPair.AzAdGroupId = adgroupId;
                    adGroupPair.AzSpCampaignId = CampaignID;
                    adGroupPair.AzAdGroupUsageType = CampaignUsageType;
                    adGroupPair.CountryId = CountryId;
                    adGroupPair.ClientId = Auth.ClientId;

                    //update db to add ad group and make this primary in campaign
                    SaveData sd = new SaveData();
                    var success = await sd.DeactiveOldAdGroup(ExistingAdGroupId, ExistingCampaignId, CountryId, Auth.ClientId);
                    var success2 = await sd.SaveAdGroup(adGroupPair, Auth.ClientId);


                    List<string> adGroups = new List<string>();
                    adGroups.Add(adgroupId);

                    CampaignRequest request = new CampaignRequest();

                    //add asin to ProductAsinsAndCampaignNames
                    RetrieveData rd = new RetrieveData();
                    string asin = await rd.GetProductAsin(ProductId);
                    string? addProductResponse = "";

                    if (!string.IsNullOrEmpty (asin))
                    {
                        ProductAsinAndCampaignName p = new ProductAsinAndCampaignName();
                        p.Asin = asin;

                        request.ProductAsinsAndCampaignNames.Add(p);
                        request.Authorization.ClientProfileCodes = ClientProfileCodes;

                        CountrySpecificRules CountryToCreate = new CountrySpecificRules();

                        string productEndpoint = "sp/productAds";
                        string productMediaType = "application/vnd.spproductAd.v3+json";

                        AddProduct addProduct = new AddProduct();
                        addProductResponse = await addProduct.AddThisProduct(CountryId, CampaignID, adGroups, request, CountryToCreate, productEndpoint, productMediaType, Auth);
                    }

                    if (addProductResponse == "0" || string.IsNullOrEmpty(asin)) 
                    {
                        return null;
                    }
                    
                }

                return adgroupId;
            }
            catch(Exception ex)
            {
                return null;
            }

        }
    }
}
