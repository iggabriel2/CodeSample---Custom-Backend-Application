using AdTool.AzSponsoredProducts.BusinessObjects.AdGroups;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignManagement;
using AdTool.Entities.CampaignsAdGroups;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.Keyword
{
    public class UpdateKeyword
    {
        public async Task<string> Update(int CountryId,  KeywordChangeRequest request, string EndPoint, string MediaType, APIAuthorization Auth)
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
                KeywordUpdateResponse updateKeywordResponse = new KeywordUpdateResponse();
                updateKeywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<KeywordUpdateResponse>(responseMessage.Content.ReadAsStream());

                if (updateKeywordResponse.keywords.error != null && updateKeywordResponse.keywords.error.Count > 0)
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
                    KeywordUpdateResponse updateKeywordResponse = new KeywordUpdateResponse();
                    updateKeywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<KeywordUpdateResponse>(responseMessage.Content.ReadAsStream());

                    if (updateKeywordResponse.keywords.error != null && updateKeywordResponse.keywords.error.Count > 0)
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

        public async Task<string> MakeObjectToSend(KeywordChangeRequest request)
        {
            //make object to send
            KeywordUpdateRequest responseAz = new KeywordUpdateRequest();

            BusinessObjects.Keyword.Keyword keyword = new BusinessObjects.Keyword.Keyword();
            keyword.keywordId = request.keywordId;
            keyword.state = request.state.ToUpper();
            keyword.bid = request.bid;
            responseAz.keywords.Add(keyword);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(responseAz);

            return serlializedJson;
        }
    }
}
