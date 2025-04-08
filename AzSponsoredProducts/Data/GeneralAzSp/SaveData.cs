using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Save;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.D4Api;
using AdTool.Entities.Edit;
using AdTool.Entities.Edit.Auth;
using AdTool.Entities.Logging;
using Azure.Core;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;

namespace AdTool.AzSponsoredProducts.Data
{
    public class SaveData
    {
        public async Task<bool> UpdateAccessToken(AllAccessTokens accessTokenFromDB)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateAccessToken", new { @AccessToken = accessTokenFromDB.AccessToken, @TokenExpirationTime = accessTokenFromDB.TokenExpirationTime, @ClientId = accessTokenFromDB.ClientId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateAccessToken";
                logError.ClientId = accessTokenFromDB.ClientId;
                logError.Parameters = JsonSerializer.Serialize(accessTokenFromDB);
                await logging.WriteToLog(logError);
            }
            return true;
        }

        public async Task<Guid> CreateRefreshTokenAndProfileCodes(APIAuthorizationRequest aPIAuthorizationRequest)
        {
            try
            {
                Guid responseClientId;
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    responseClientId = (await connection.QueryAsync<Guid>("EditOrUpdateAzSpClient", new { @TokenExpirationTime = aPIAuthorizationRequest.TokenExpirationTime, @AccessToken = aPIAuthorizationRequest.AccessToken, @RefreshToken = aPIAuthorizationRequest.RefreshToken, @AppUserId = aPIAuthorizationRequest.AppUserId, @ClientId = aPIAuthorizationRequest.ClientId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                
                    foreach(var profileCode in aPIAuthorizationRequest.ClientProfileCodes)
                    {
                        var affectedRows = await connection.ExecuteAsync("CreateRefreshTokenAndProfileCodes", new { ProfileCode = profileCode.ProfileCode, CountryId = profileCode.CountryId, ClientId = responseClientId, @TimeZone = profileCode.TimeZone }, commandType: CommandType.StoredProcedure);
                    }
                }
                return responseClientId;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CreateRefreshTokenAndProfileCodes";
                logError.ClientId = aPIAuthorizationRequest.ClientId;
                logError.Parameters = JsonSerializer.Serialize(aPIAuthorizationRequest);
                await logging.WriteToLog(logError);

                return Guid.Empty;
            }
        }

        public async Task<int> RecreateProfileCodes(APIAuthorizationRequest aPIAuthorizationRequest)
        {
            try
            {
                int responseClientId = 0;
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    foreach (var profileCode in aPIAuthorizationRequest.ClientProfileCodes)
                    {
                        var affectedRows = await connection.ExecuteAsync("RecreateProfileCodes", new { ProfileCode = profileCode.ProfileCode, CountryId = profileCode.CountryId, ClientId = aPIAuthorizationRequest.ClientId, @TimeZone = profileCode.TimeZone }, commandType: CommandType.StoredProcedure);
                    }
                }
                return responseClientId;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "RecreateProfileCodes";
                logError.ClientId = aPIAuthorizationRequest.ClientId;
                logError.Parameters = JsonSerializer.Serialize(aPIAuthorizationRequest);
                await logging.WriteToLog(logError);

                return 0;
            }
        }

        public async Task<int> SavePortfolios(AzPortfolio azPortfolio)
        {
            int responseId = 0;
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                   responseId = (await connection.QueryAsync<int>("EditOrUpdatePortfolios", azPortfolio, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SavePortfolios";
                logError.ClientId = azPortfolio.ClientId;
                logError.Parameters = JsonSerializer.Serialize(azPortfolio);
                await logging.WriteToLog(logError);
            }

            return responseId;
        }

        public async Task<int> SaveCampaign(CampaignSave campaign)
        {
            int responseId = 0;
            try
            {
                //if this is Tier 1 or Performance, set existing Tier 1 or Performance so that it is not primary
                if (campaign.AzSpCampaignUsageType > 1)
                {
                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        var affectedRows = await connection.ExecuteAsync("SaveCampaign", new { @ProductId = campaign.ProductId, @CountryId = campaign.CountryId, @AzSpCampaignUsageType = campaign.AzSpCampaignUsageType, @clientId = campaign.AzClientId }, commandType: CommandType.StoredProcedure);
                    }
                }

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    responseId = (await connection.QueryAsync<int>("EditOrUpdateCampaigns", campaign, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveCampaign";
                logError.ClientId = campaign.AzClientId;
                logError.Parameters = JsonSerializer.Serialize(campaign);
                await logging.WriteToLog(logError);
            }
            return responseId;
        }

        public async Task<bool> SaveAdGroup(AdGroupPairs AdGroup, Guid ClientId)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("EditOrUpdateAdGroups", AdGroup, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveAdGroup";
                logError.ClientId = ClientId;
                logError.Parameters = JsonSerializer.Serialize(AdGroup);
                await logging.WriteToLog(logError);
            }
            return true;
        }

        public async Task<bool> DeactiveOldAdGroup(string AzAdGroupId, string AzSpCampaignId, int CountryId, Guid ClientId)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("DeactiveOldAdGroup", new { @AzAdGroupId = AzAdGroupId, @AzSpCampaignId = AzSpCampaignId, @CountryId = CountryId, @ClientId = ClientId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "DeactiveOldAdGroup";
                logError.ClientId = ClientId;
                logError.Parameters = AzAdGroupId + " " + AzSpCampaignId + " " + CountryId + " " + ClientId.ToString();
                await logging.WriteToLog(logError);
            }
            return true;
        }

        public async Task<bool> SaveKeywords(List<KeywordUpdate> keywordUpdate, Guid clientId, SearchTermRefresh searchTerm, bool books = false)
        {
            try
            {
                Guid searchKey = Guid.NewGuid();

                SqlMapper.Settings.CommandTimeout = 240;

                //make name caps
                GeneralUtils generalUtils = new GeneralUtils();
                searchTerm.FriendlyName = await generalUtils.CapName(searchTerm.FriendlyName);

                int responseSearchTermId = 0;
                if (!books)
                {
                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        responseSearchTermId = (await connection.QueryAsync<int>("EditOrUpdateKeywordsSearchTerms", new { @searchTerm = searchTerm.SearchTerm, @regularSearchTerm = searchTerm.FriendlyName.Trim().Replace(".", "") }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    }
                }
                else
                {
                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        responseSearchTermId = (await connection.QueryAsync<int>("EditOrUpdateKeywordsSearchTermsForBooks", new { @searchTerm = searchTerm.SearchTerm, @regularSearchTerm = searchTerm.FriendlyName.Trim().Replace(".", "") }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    }
                }


                if (keywordUpdate != null && keywordUpdate.Count > 0)
                {
                    keywordUpdate.ForEach(s => s.KeywordSearchTermId = responseSearchTermId);
                    keywordUpdate.ForEach(s => s.SearchKey = searchKey);
                    LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                    DataTable dt = linqToDataTableUtil.LinqToDataTable<KeywordUpdate>(keywordUpdate);

                    using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                    {
                        bcopy.DestinationTableName = "KeywordsLocatedTemp";
                        SqlBulkCopyColumnMapping mapping = new SqlBulkCopyColumnMapping("KeywordSearchTermId", "KeywordSearchTermId");
                        SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("Keyword", "Keyword");
                        SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("TypeId", "TypeId");
                        SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("SourceId", "SourceId");
                        SqlBulkCopyColumnMapping mapping5 = new SqlBulkCopyColumnMapping("SearchKey", "SearchKey");
                        bcopy.ColumnMappings.Add(mapping);
                        bcopy.ColumnMappings.Add(mapping2);
                        bcopy.ColumnMappings.Add(mapping3);
                        bcopy.ColumnMappings.Add(mapping4);
                        bcopy.ColumnMappings.Add(mapping5);
                        bcopy.WriteToServer(dt);
                    }

                    //reconcile keywords
                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        var affectedRows = await connection.ExecuteAsync("ReconcileKeywords", new { @searchKey = searchKey }, commandType: CommandType.StoredProcedure);
                    }

                    //get types for this search term
                    List<int> searchTermTypes = new List<int>();

                    using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                    {
                        searchTermTypes = (await connection.QueryAsync<int>("GetKeywordsBySearchTermId", new { @id = responseSearchTermId }, commandType: CommandType.StoredProcedure)).ToList();
                    }

                    //update if search term type exists
                    if (searchTermTypes.Contains(1) || searchTermTypes.Contains(3))
                    {
                        using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                        {
                            var affectedRows = await connection.ExecuteAsync("UpdateKeywordsSearchTermsForHasKeywords", new { @id = responseSearchTermId }, commandType: CommandType.StoredProcedure);
                        }
                    }

                    if (searchTermTypes.Contains(2) || searchTermTypes.Contains(6) || searchTermTypes.Contains(7))
                    {
                        using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                        {
                            var affectedRows = await connection.ExecuteAsync("UpdateKeywordsSearchTermsForHasAsins", new { @id = responseSearchTermId }, commandType: CommandType.StoredProcedure);
                        }
                    }

                }

                return true;
                
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveKeywords";
                logError.ClientId = clientId;
                logError.Parameters = JsonSerializer.Serialize(keywordUpdate);
                await logging.WriteToLog(logError);

                return false;
            }
        }

        public async Task<bool> SaveCampaignsBatch(List<CampaignSaveBatch> campaigns, Guid clientId, int countryId)
        {
            try
            {

                //clear temp table as a precaution
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("SaveCampaignsBatch", new { @clientid = clientId, @country = countryId }, commandType: CommandType.StoredProcedure);
                }


                Guid bulkId = Guid.NewGuid();


                campaigns.ForEach(s => s.BulkId = bulkId);
                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<CampaignSaveBatch>(campaigns);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.DestinationTableName = "AzSpCampaignsBatchTemp";
                    SqlBulkCopyColumnMapping mapping = new SqlBulkCopyColumnMapping("BulkId", "BulkId");
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("AZCampaignId", "AZCampaignId");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("AzPortfolioId", "AzPortfolioId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("ProductId", "ProductId");
                    SqlBulkCopyColumnMapping mapping5 = new SqlBulkCopyColumnMapping("CampaignName", "CampaignName");
                    SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("CountryId", "CountryId");
                    SqlBulkCopyColumnMapping mapping7 = new SqlBulkCopyColumnMapping("Active", "Active");
                    SqlBulkCopyColumnMapping mapping8 = new SqlBulkCopyColumnMapping("AzClientId", "azClientId");
                    SqlBulkCopyColumnMapping mapping9 = new SqlBulkCopyColumnMapping("AzSpCampaignUsageType", "AzSpCampaignUsageType");
                    SqlBulkCopyColumnMapping mapping10 = new SqlBulkCopyColumnMapping("AzSpPrimaryInUsageType", "AzSpPrimaryInUsageType");
                    SqlBulkCopyColumnMapping mapping11 = new SqlBulkCopyColumnMapping("GeneratedByUs", "GeneratedByUs");
                    SqlBulkCopyColumnMapping mapping12 = new SqlBulkCopyColumnMapping("Budget", "Budget");
                    SqlBulkCopyColumnMapping mapping13 = new SqlBulkCopyColumnMapping("DynamicBiddingStrategy", "DynamicBiddingStrategy");
                    SqlBulkCopyColumnMapping mapping14 = new SqlBulkCopyColumnMapping("State", "State");
                    SqlBulkCopyColumnMapping mapping15 = new SqlBulkCopyColumnMapping("TargetingType", "TargetingType");
                    bcopy.ColumnMappings.Add(mapping);
                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping5);
                    bcopy.ColumnMappings.Add(mapping6);
                    bcopy.ColumnMappings.Add(mapping7);
                    bcopy.ColumnMappings.Add(mapping8);
                    bcopy.ColumnMappings.Add(mapping9);
                    bcopy.ColumnMappings.Add(mapping10);
                    bcopy.ColumnMappings.Add(mapping11);
                    bcopy.ColumnMappings.Add(mapping12);
                    bcopy.ColumnMappings.Add(mapping13);
                    bcopy.ColumnMappings.Add(mapping14);
                    bcopy.ColumnMappings.Add(mapping15);
                    bcopy.WriteToServer(dt);
                }

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("EditOrUpdateCampaignsNightly", new { @ClientId = campaigns[0].AzClientId }, commandType: CommandType.StoredProcedure);
                }

                return true;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "SaveCampaignsBatch";
                logError.ClientId = campaigns[0].AzClientId;
                logError.Parameters = JsonSerializer.Serialize(campaigns);
                await logging.WriteToLog(logError);

                return false;
            }
        }

        public async Task<bool> UpdateCampaign(CampaignUpdateDbObject campaignUpdate)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateCampaign", campaignUpdate, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateCampaign";
                logError.ClientId = campaignUpdate.ClientId;
                logError.Parameters = JsonSerializer.Serialize(campaignUpdate);
                await logging.WriteToLog(logError);
            }
            return true;
        }

        public async Task<bool> UpdateBiddingStrategyOnMultipleCampaigns(CampaignUpdateDbObject campaignUpdate)
        {
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("UpdateBiddingStrategyOnMultipleCampaigns", campaignUpdate, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateBiddingStrategyOnMultipleCampaigns";
                logError.ClientId = campaignUpdate.ClientId;
                logError.Parameters = JsonSerializer.Serialize(campaignUpdate);
                await logging.WriteToLog(logError);
            }
            return true;
        }

        public async Task<bool> UpdateMultipleCampaigns(List<CampaignUpdateDbObject> campaigns)
        {
            try
            {
                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<CampaignUpdateDbObject>(campaigns);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.DestinationTableName = "AzSpCampaigns_BulkUpdate_Temp";
                    SqlBulkCopyColumnMapping mapping = new SqlBulkCopyColumnMapping("CampaignId", "AZCampaignId");
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("DynamicBiddingStrategy", "DynamicBiddingStrategy");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("ClientId", "ClientId");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("CountryId", "CountryId");
                    bcopy.ColumnMappings.Add(mapping);
                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.WriteToServer(dt);
                }

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("BulkUpdateCampaigns", new { @ClientId = campaigns[0].ClientId}, commandType: CommandType.StoredProcedure);
                }

                return true;

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "UpdateMultipleCampaigns";
                logError.ClientId = campaigns[0].ClientId;
                logError.Parameters = JsonSerializer.Serialize(campaigns);
                await logging.WriteToLog(logError);

                return false;
            }
        }

    }
}
