using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.ProductAdListResponse;

namespace AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create
{
    public class AddProduct
    {
        public async Task<string> AddThisProduct(int CountryId, string CampaignID, List<string> allAdGroups, CampaignRequest request, CountrySpecificRules CountryToCreate, string EndPoint, string MediaType, APIAuthorization Auth)
        {

            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            //make object
            string serlializedJson = await MakeObjectToSend(request, CountryToCreate, CampaignID, allAdGroups, CountryId);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            ProductAdsResponseRoot productResposne = new ProductAdsResponseRoot();
            if (responseMessage.IsSuccessStatusCode)
            {
                productResposne = await JsonSerializer.DeserializeAsync<ProductAdsResponseRoot>(responseMessage.Content.ReadAsStream());
            }

            if (responseMessage.IsSuccessStatusCode && productResposne.productAds.error.Count == 0)
            {
                return "1";
            }
            else
            {
                //make sure it doesn't exist in the list
                string serlializedJson2 = await MakeObjectToSendToVerify(request, CountryToCreate, CampaignID, allAdGroups, CountryId);
                HttpResponseMessage responseMessageValidation = await azAPIUtils.CallAmazonPostApi(EndPoint + "/list", MediaType, Auth, profileCode, serlializedJson2);

                if (responseMessageValidation.IsSuccessStatusCode)
                {
                    ProductResponseRoot productAdsResponseRoot = new ProductResponseRoot();
                    productAdsResponseRoot = await JsonSerializer.DeserializeAsync<ProductResponseRoot>(responseMessageValidation.Content.ReadAsStream());

                    if (productAdsResponseRoot.totalResults == 0)
                    {
                        //we have nothing. keep going.
                    }
                    else
                    {
                        foreach(var adgroupFound in productAdsResponseRoot.productAds)
                        {
                            if (adgroupFound.asin == request.ProductAsinsAndCampaignNames[0].Asin)
                            {
                                allAdGroups.Remove(adgroupFound.adGroupId);
                            }
                        }

                        if (allAdGroups.Count == 0)
                        {
                            return "1";
                        }
                    }
                }



                //continue creating if we did not find the product or failed to get it
                string serlializedJson3 = await MakeObjectToSend(request, CountryToCreate, CampaignID, allAdGroups, CountryId);
                responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson3);

                if (responseMessage.IsSuccessStatusCode)
                {
                    productResposne = await JsonSerializer.DeserializeAsync<ProductAdsResponseRoot>(responseMessage.Content.ReadAsStream());
                }

                if (responseMessage.IsSuccessStatusCode && productResposne.productAds.error.Count == 0)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }

            }
        }

        private async Task<string> MakeObjectToSend(CampaignRequest request, CountrySpecificRules CountryToCreate, string CampaignID, List<string> allAdGroups, int CountryId)
        {
            APIProductsRequestRoot productRequestRoot = new APIProductsRequestRoot();
            List<APIProductsRequest> productRequestList = new List<APIProductsRequest>(); //this holds your post parameters

            foreach (var adgroup in allAdGroups)
            {
                APIProductsRequest productRequest = new APIProductsRequest();
                productRequest.adGroupId = adgroup;
                productRequest.asin = request.ProductAsinsAndCampaignNames[0].Asin;
                productRequest.campaignId = CampaignID;
                productRequest.state = "ENABLED";

                if (CountryId == 1 && !string.IsNullOrEmpty(CountryToCreate.SalesText))
                {
                    productRequest.customText = CountryToCreate.SalesText;
                }
                productRequestList.Add(productRequest);
            }

            productRequestRoot.productAds = productRequestList;

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(productRequestRoot);

            return serlializedJson;
        }

        private async Task<string> MakeObjectToSendToVerify(CampaignRequest request, CountrySpecificRules CountryToCreate, string CampaignID, List<string> allAdGroups, int CountryId)
        {
            AdGroupRequestObject productRequestRoot = new AdGroupRequestObject();
            productRequestRoot.campaignIdFilter.include.Add(CampaignID);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(productRequestRoot);

            return serlializedJson;
        }
    }
}
