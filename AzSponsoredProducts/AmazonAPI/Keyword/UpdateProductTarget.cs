using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword.ProductTargets;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.Keyword
{
    public class UpdateProductTarget
    {
        public async Task<string> Update(int CountryId, KeywordChangeRequest request, string EndPoint, string MediaType, APIAuthorization Auth)
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
                ProductTargetUpdateResponse updateKeywordResponse = new ProductTargetUpdateResponse();
                updateKeywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetUpdateResponse>(responseMessage.Content.ReadAsStream());

                if (updateKeywordResponse.targetingClauses.error != null && updateKeywordResponse.targetingClauses.error.Count > 0)
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
                    ProductTargetUpdateResponse updateKeywordResponse = new ProductTargetUpdateResponse();
                    updateKeywordResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<ProductTargetUpdateResponse>(responseMessage.Content.ReadAsStream());

                    if (updateKeywordResponse.targetingClauses.error != null && updateKeywordResponse.targetingClauses.error.Count > 0)
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
            ProdutTargetUpdateRequest responseAz = new ProdutTargetUpdateRequest();

            BusinessObjects.Keyword.TargetingClauseUpdate target = new BusinessObjects.Keyword.TargetingClauseUpdate();
            target.targetId = request.keywordId;
            target.state = request.state.ToUpper();
            target.bid = request.bid;
            //target.expressionType = request.expressionType;

            //foreach(var expression in request.expression)
            //{
            //    BusinessObjects.Keyword.ProductTargetExpression productTargetExpression = new BusinessObjects.Keyword.ProductTargetExpression();
            //    productTargetExpression.type = await MakeType(expression.type);
            //    productTargetExpression.value = expression.value;
            //    target.expression.Add(productTargetExpression);
            //}

            responseAz.targetingClauses.Add(target);

            //serialize object to send
            string serlializedJson = JsonSerializer.Serialize(responseAz);

            return serlializedJson;
        }

        public async Task<string> MakeType(string rawType)
        {
            if (rawType.ToUpper() == "ASINAGERANGESAMEAS")
            {
                return "ASIN_AGE_RANGE_SAME_AS";
            }
            else if (rawType.ToUpper() == "ASINSAMEAS")
            {
                return "ASIN_SAME_AS";
            }
            else if (rawType.ToUpper() == "ASINREVIEWRATINGLESSTHAN")
            {
                return "ASIN_REVIEW_RATING_LESS_THAN";
            }
            else if (rawType.ToUpper() == "ASINPRICEGREATERTHAN")
            {
                return "ASIN_PRICE_GREATER_THAN";
            }
            else if (rawType.ToUpper() == "ASINREVIEWRATINGBETWEEN")
            {
                return "ASIN_REVIEW_RATING_BETWEEN";
            }
            else if (rawType.ToUpper() == "ASINGENRESAMEAS")
            {
                return "ASIN_GENRE_SAME_AS";
            }
            else if (rawType.ToUpper() == "QUERYHIGHRELMATCHES")
            {
                return "QUERY_HIGH_REL_MATCHES";
            }
            else if (rawType.ToUpper() == "ASINEXPANDEDFROM")
            {
                return "ASIN_EXPANDED_FROM";
            }
            else if (rawType.ToUpper() == "ASINREVIEWRATINGGREATERTHAN")
            {
                return "ASIN_REVIEW_RATING_GREATER_THAN";
            }
            else if (rawType.ToUpper() == "ASINPRICELESSTHAN")
            {
                return "ASIN_PRICE_LESS_THAN";
            }
            else if (rawType.ToUpper() == "ASINPRICEBETWEEN")
            {
                return "ASIN_PRICE_BETWEEN";
            }
            else if (rawType.ToUpper() == "ASINBRANDSAMEAS")
            {
                return "ASIN_BRAND_SAME_AS";
            }
            else if (rawType.ToUpper() == "ASINSUBSTITUTERELATED")
            {
                return "ASIN_SUBSTITUTE_RELATED";
            }
            else if (rawType.ToUpper() == "ASINCATEGORYSAMEAS")
            {
                return "ASIN_CATEGORY_SAME_AS";
            }
            else if (rawType.ToUpper() == "ASINACCESSORYRELATED")
            {
                return "ASIN_ACCESSORY_RELATED";
            }
            else if (rawType.ToUpper() == "QUERYBROADRELMATCHES")
            {
                return "QUERY_BROAD_REL_MATCHES";
            }
            else if (rawType.ToUpper() == "ASINISPRIMESHIPPINGELIGIBLE")
            {
                return "ASIN_IS_PRIME_SHIPPING_ELIGIBLE";
            }
            else
            {
                return rawType;
            }
        }
    }
}
