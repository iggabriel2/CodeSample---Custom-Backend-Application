using AdTool.AzSponsoredProducts.BusinessObjects.AdGroups;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.Special;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.CampaignsAdGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.AdGroups
{
    public class UpdateAzAdGroup
    {
        public async Task<string> Update(int CountryId, UpdateAdGroupRequest request, string EndPoint, string MediaType, APIAuthorization Auth)
        {

            ClientProfileCodes profileCode = request.Authorization.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            //make object
            string serlializedJson = await MakeObjectToSend(request);

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPutApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                UpdateAdGroupResponseAz updateAdGroupResponseAz = new UpdateAdGroupResponseAz();
                updateAdGroupResponseAz = await System.Text.Json.JsonSerializer.DeserializeAsync<UpdateAdGroupResponseAz>(responseMessage.Content.ReadAsStream());

                if (updateAdGroupResponseAz.adGroups.error != null && updateAdGroupResponseAz.adGroups.error.Count > 0)
                {
                    return "0";
                }
                else
                {
                    return "1";
                }
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPutApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                if (responseMessage.IsSuccessStatusCode)
                {
                    UpdateAdGroupResponseAz updateAdGroupResponseAz = new UpdateAdGroupResponseAz();
                    updateAdGroupResponseAz = await System.Text.Json.JsonSerializer.DeserializeAsync<UpdateAdGroupResponseAz>(responseMessage.Content.ReadAsStream());

                    if (updateAdGroupResponseAz.adGroups.error != null && updateAdGroupResponseAz.adGroups.error.Count > 0)
                    {
                        return "0";
                    }
                    else
                    {
                        return "1";
                    }
                }
                else
                {
                    return "0";
                }
            }
        }

        public async Task<string> MakeObjectToSend(UpdateAdGroupRequest request)
        {
            //make object to send
            UpdateAdGroupRequestAz responseAz = new UpdateAdGroupRequestAz();

            AdGroup adGroup = new AdGroup();
            adGroup.adGroupId = request.adGroupId;
            adGroup.defaultBid = request.defaultBid;
            adGroup.state = request.state;
            adGroup.name = request.name;
            responseAz.adGroups.Add(adGroup);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(responseAz);

            return serlializedJson;
        }
    }
}
