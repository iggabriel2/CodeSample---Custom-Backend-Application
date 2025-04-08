using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Logging;
using Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdTool.Entities.AzSp.General;
using Azure;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.Entities.AzSpApi.CampaignCreations;
using AdTool.Entities.AzSpApi.ProductManagement;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.CampaignsAdGroups;
using AdTool.Entities.AzSpApi.CampaignsAdGroups;
using AdTool.Entities.AzSpApi.CampaignManagement;

namespace AdTool.BusinessLogic.BusinessLogic.AzSp
{
    public class AzSpBackendAPI
    {
        #region Product
        public async Task<ProductResponse> GetProductInformation(ProductRequest productRequest)
        {
            ProductResponse? myResponse = new ProductResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(productRequest);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/GeneralProduct/GetProductInfo", productRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<ProductResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetProductInformation - Api Call";
                    logError.ClientId = productRequest.Authorization.ClientId;
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get product.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetProductInformation";
                logError.ClientId = productRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(productRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get product.";

            }
            return myResponse;
        }

        public async Task<GetProductResponseAPI> GetProductData(GetCampaignRequestApi productRequest)
        {
            GetProductResponseAPI? myResponse = new GetProductResponseAPI();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(productRequest);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/GeneralProduct/ProductData", productRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<GetProductResponseAPI>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetProductData - Api Call";
                    logError.ClientId = productRequest.Authorization.ClientId;
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get product list.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetProductData";
                logError.ClientId = productRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(productRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get product list.";

            }
            return myResponse;
        }
        #endregion

        #region Portfolio
        public async Task<PortfolioResponse> CreatePortfolio(CreatePortfolioRequest portfolioRequest)
        {
            PortfolioResponse? myResponse = new PortfolioResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(portfolioRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/GeneralProduct/CreatePortfolio", portfolioRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<PortfolioResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "CreatePortfolio - Api Call";
                    logError.ClientId = portfolioRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(portfolioRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to create portfolio.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreatePortfolio";
                logError.ClientId = portfolioRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(portfolioRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to create portfolio.";

            }
            return myResponse;
        }

        public async Task<PortfolioListResponse> GetPortfolios(PortfolioRequest portfolioRequest)
        {
            PortfolioListResponse? myResponse = new PortfolioListResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(portfolioRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/GeneralProduct/GetPortfolios", portfolioRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<PortfolioListResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;

                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetPortfolios - Api Call";
                    logError.ClientId = portfolioRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(portfolioRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get portfolio list.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetPortfolios";
                logError.ClientId = portfolioRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(portfolioRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get portfolio list.";

            }
            return myResponse;
        }
        #endregion

        #region Campaigns
        public async Task<GetCampaignResponseApi> GetCampaigns(GetCampaignRequestApi request)
        {
            GetCampaignResponseApi? myResponse = new GetCampaignResponseApi();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(request);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Campaign/GetCampaigns", request.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<GetCampaignResponseApi>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetCampaigns - Api Call";
                    logError.ClientId = request.Authorization.ClientId;
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get Campaign information.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCampaigns";
                logError.ClientId = request.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(request);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get Campaign information.";

            }
            return myResponse;
        }
        public async Task<CampaignResponse> CreateCampaign(CampaignRequest campaignsRequest)
        {
            CampaignResponse? myResponse = new CampaignResponse();
            try
            {

                string serlializedJson = JsonSerializer.Serialize(campaignsRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Campaign/Create", campaignsRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<CampaignResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;

                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "CreateCampaign - Api Call";
                    logError.ClientId = campaignsRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(campaignsRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to create campaign.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreateCampaign";
                logError.ClientId = campaignsRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(campaignsRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to create campaign.";

            }
            return myResponse;
        }

        public async Task<SimpleResponse> GetCampaignName(CampaignNameRequest campaignsRequest)
        {
            SimpleResponse? myResponse = new SimpleResponse();
            try
            {

                string serlializedJson = JsonSerializer.Serialize(campaignsRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Campaign/GetCampaignName", campaignsRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimpleResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;

                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetCampaignName - Api Call";
                    logError.ClientId = campaignsRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(campaignsRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get campaign name.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetCampaignName";
                logError.ClientId = campaignsRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(campaignsRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get campaign name.";

            }
            return myResponse;
        }

        public async Task<SimpleResponse> UpdateCampaign(CampaignUpdateRequest campaignsRequest)
        {
            SimpleResponse? myResponse = new SimpleResponse();
            try
            {

                string serlializedJson = JsonSerializer.Serialize(campaignsRequest);
                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Campaign/UpdateCampaign", campaignsRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimpleResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;

                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "UpdateCampaign - Api Call";
                    logError.ClientId = campaignsRequest.Authorization.ClientId;
                    logError.Parameters = JsonSerializer.Serialize(campaignsRequest);
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to update campaign.";
                    return myResponse;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateCampaign";
                logError.ClientId = campaignsRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(campaignsRequest);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to update campaign.";

            }
            return myResponse;
        }



        #endregion

        #region Countries

        public async Task<OriginalAPIAuthorizationResponse> RecheckCountries(CountryAuthorizationUpdateRequest countryRequest)
        {
            OriginalAPIAuthorizationResponse? myResponse = new OriginalAPIAuthorizationResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(countryRequest);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/Authorization/RecheckCountries", countryRequest.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<OriginalAPIAuthorizationResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "RecheckCountries - Api Call";
                    logError.ClientId = countryRequest.Authorization.ClientId;
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);

                    myResponse.ErrorMessage  = "Failed to get countries.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "RecheckCountries";
                logError.ClientId = countryRequest.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(countryRequest);
                await logging.WriteToLog(logError);

                myResponse.ErrorMessage = "Failed to get countries.";

            }
            return myResponse;
        }

        #endregion Countries

        #region AdGroup
        public async Task<GetAdGroupsResponseAPI> GetByCampaign(GetAdGroupsRequest request)
        {
            GetAdGroupsResponseAPI? myResponse = new GetAdGroupsResponseAPI();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(request);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/AdGroups/GetByCampaign", request.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<GetAdGroupsResponseAPI>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "GetByCampaign - Api Call";
                    logError.ClientId = request.Authorization.ClientId;
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to get ad group information.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetByCampaign";
                logError.ClientId = request.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(request);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to get ad group information.";

            }
            return myResponse;
        }

        public async Task<SimpleResponse> Update(UpdateAdGroupRequest request)
        {
            SimpleResponse? myResponse = new SimpleResponse();
            try
            {
                string serlializedJson = JsonSerializer.Serialize(request);

                BackendAPIUtilities utils = new BackendAPIUtilities();
                HttpResponseMessage response = await utils.CallPostApi(serlializedJson, "api/AdGroups/Update", request.Authorization.ClientId);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    myResponse = await JsonSerializer.DeserializeAsync<SimpleResponse>(await response.Content.ReadAsStreamAsync(), options);
                    return myResponse;
                }
                else
                {
                    Logging logging = new Logging();
                    LogError logError = new LogError();
                    logError.ErrorMessage = response.Content.ToString();
                    logError.FailureMethod = "Update ad group - Api Call";
                    logError.ClientId = request.Authorization.ClientId;
                    logError.Parameters = serlializedJson;
                    await logging.WriteToLog(logError);

                    myResponse.APIAuthorization.ErrorMessage = "Failed to update ad group.";
                    return myResponse;

                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "Update ad group ";
                logError.ClientId = request.Authorization.ClientId;
                logError.Parameters = JsonSerializer.Serialize(request);
                await logging.WriteToLog(logError);

                myResponse.APIAuthorization.ErrorMessage = "Failed to update ad group.";

            }
            return myResponse;
        }
        #endregion AdGroup
    }
}

