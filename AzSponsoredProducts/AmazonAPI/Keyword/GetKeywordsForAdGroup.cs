using AdTool.AzSponsoredProducts.BusinessObjects.AdGroups;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignManagement;
using AdTool.Entities.CampaignsAdGroups;
using AdTool.Entities.D4Api;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.AmazonAPI.Keyword
{
    public class GetKeywordsForAdGroup
    {
        public async Task<KeywordListResponse> GetKeywords(int CountryId, APIAuthorizationRequest authRequest, string adGroupId, string EndPoint, string MediaType, APIAuthorization Auth, List<string> keywordIds = null, bool GetByKeywordIds = false)
        {

            ClientProfileCodes profileCode = authRequest.ClientProfileCodes.Where(x => x.CountryId == CountryId).FirstOrDefault();

            string serlializedJson = "";

            //make object
            if (!GetByKeywordIds)
            {
                serlializedJson = await MakeObjectToSend(adGroupId);
            }
            else
            {
                serlializedJson = await MakeObjectToSendForKeywordList(keywordIds);
            }

            //call api here
            AzAPIUtils azAPIUtils = new AzAPIUtils();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = await azAPIUtils.CallAmazonPostApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

            if (responseMessage.IsSuccessStatusCode)
            {
                KeywordListResponse keywordResponse = new KeywordListResponse();
                keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<KeywordListResponse>(responseMessage.Content.ReadAsStream());

                if (keywordResponse != null && keywordResponse.keywords != null && keywordResponse.keywords.Count > 0)
                {
                    return keywordResponse;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                responseMessage = await azAPIUtils.CallAmazonPutApi(EndPoint, MediaType, Auth, profileCode, serlializedJson);

                if (responseMessage.IsSuccessStatusCode)
                {
                    KeywordListResponse keywordResponse = new KeywordListResponse();
                    keywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<KeywordListResponse>(responseMessage.Content.ReadAsStream());

                    if (keywordResponse != null && keywordResponse.keywords != null && keywordResponse.keywords.Count > 0)
                    {
                        return keywordResponse;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<string> MakeObjectToSend(string adGroupId)
        {
            //make object to send
            KeywordListRequest responseAz = new KeywordListRequest();

            responseAz.adGroupIdFilter.include.Add(adGroupId);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(responseAz);

            return serlializedJson;
        }

        public async Task<string> MakeObjectToSendForKeywordList(List<string> keywordIds)
        {
            //make object to send
            KeywordListRequestByKeywordIds responseAz = new KeywordListRequestByKeywordIds();

            responseAz.keywordIdFilter.include = keywordIds;

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(responseAz);

            return serlializedJson;
        }
    }
}
