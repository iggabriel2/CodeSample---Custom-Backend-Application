using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessLogic.Keywords;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords;
using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSpApi.CampaignCreations;
using Azure;
using Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using AdTool.AzSponsoredProducts.Data.KeywordManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using System.Security.Cryptography.X509Certificates;
using AdTool.Entities.Edit;
using AdTool.Entities.AzSp.CampaignCreations;
using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign.Create;
using static System.Collections.Specialized.BitVector32;
using AdTool.AzSponsoredProducts.AmazonAPI.ExtraKeywordPromoManagement;
using System.Linq.Expressions;
using AdTool.Entities.Logging;
using AdTool.BusinessLogic.DataAccess;
using System.Transactions;
using AdTool.AzSponsoredProducts.AmazonAPI.ProductManagement;
using AdTool.Entities.AzSpApi.ProductManagement;
using AdTool.Entities.AzSp.ProductManagement;
using System.Text.RegularExpressions;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.Utils;
using static Google.Rpc.Context.AttributeContext.Types;
using AdTool.AzSponsoredProducts.BusinessLogic.Authorization;
using AdTool.AzSponsoredProducts.AmazonAPI.CampaignExtra;
using Google.Protobuf.WellKnownTypes;
using System.Security.Cryptography;
using AdTool.AzSponsoredProducts.AmazonAPI.CreateCampaign;
using Azure.Core;
using AdTool.AzSponsoredProducts.TestData;
using AdTool.BusinessLogic.Utilities;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class ProcessReportsLogic
    {
        private static readonly object ReportLock = new object();
        private static readonly object SaveActionsRequiredLock = new object();
        private static readonly object SaveKeywordHistoryLock = new object();
        private static readonly object SaveKeywordActionsLock = new object();
        private static readonly object FinalSaveHistory = new object();
        private static readonly object FinalSaveActions = new object();
        private static readonly object FinalSaveActionRequred = new object();

        int endDateDaysToSubtractForRegineratedReports = 0;

        public async Task<bool> ProcessReportsLogicNow(Guid? ClientId = null)
        {
            try
            {

                SaveReportData srd = new SaveReportData();

                int recordTracking = 0;

                //skip when new user
                if (ClientId == null)
                {
                    recordTracking = await srd.SaveReportProcessingStart();
                }
                else
                {
                    recordTracking = 1;
                }

                if (recordTracking != 0)
                {
                    List<ReportUser> reportUsersRaw = new List<ReportUser>();
                    List<ReportUser> reportUsers = new List<ReportUser>();

                    RetrieveReportData rrd = new RetrieveReportData();

                    AdTool.PaymentProcessor.Data.SaveData paymentSaveData = new AdTool.PaymentProcessor.Data.SaveData();
                    var updatedCancellationStatus = await paymentSaveData.DeactivateExpiredAppUsers();

                    if (ClientId != null && ClientId != Guid.Empty)
                    {
                        reportUsers = await rrd.GetSpecificReportUser(ClientId);
                        reportUsersRaw = reportUsers.ToList();
                    }
                    else
                    {
                        reportUsers = await rrd.GetAllReportUsers();
                        reportUsersRaw = reportUsers.ToList();
                    }

                    foreach (ReportUser reportUser in reportUsersRaw)
                    {
                        APIAuthorizationRequest aPIAuthorizationRequest = new APIAuthorizationRequest();
                        aPIAuthorizationRequest.ClientId = reportUser.ClientId;
                        aPIAuthorizationRequest.RefreshToken = reportUser.RefreshToken;
                        aPIAuthorizationRequest.AccessToken = reportUser.AccessToken;
                        aPIAuthorizationRequest.TokenExpirationTime = reportUser.TokenExpirationTime;

                        //make sure user is still active
                        APIAuthorization aPIAuthtorization = new APIAuthorization();
                        aPIAuthtorization.ClientId = reportUser.ClientId;

                        APITokenCreation aPITokenCreation = new APITokenCreation();
                        var tokenCreated = await aPITokenCreation.MakeANewToken(aPIAuthtorization, aPIAuthorizationRequest);

                        aPIAuthorizationRequest.AccessToken = aPIAuthtorization.AccessToken;
                        aPIAuthorizationRequest.TokenExpirationTime = aPIAuthtorization.TokenExpirationTime;

                        //user is no longer validated to connect to Amazon
                        if (string.IsNullOrEmpty(aPIAuthorizationRequest.AccessToken.ToLower()) || aPIAuthorizationRequest.AccessToken.ToLower() == "invalid")
                        {
                            reportUsers.Remove(reportUser);
                        }
                        else
                        {
                            RecheckAllCountries recheckAllCountries = new RecheckAllCountries();
                            CountryAuthorizationUpdateRequest countryAuthorizationUpdateRequest = new CountryAuthorizationUpdateRequest();
                            countryAuthorizationUpdateRequest.Authorization = aPIAuthorizationRequest;

                            var response = await recheckAllCountries.RecheckCountries(countryAuthorizationUpdateRequest);
                            RetrieveReportData rrdCodes = new RetrieveReportData();
                            aPIAuthorizationRequest.ClientProfileCodes = await rrdCodes.GetProfileCodes(reportUser.ClientId);

                            var thisReportUser = reportUsers.Where(x => x.ClientId == reportUser.ClientId).FirstOrDefault();
                            thisReportUser.aPIAuthorizationRequest = aPIAuthorizationRequest;

                            if (aPIAuthorizationRequest.ClientProfileCodes == null || aPIAuthorizationRequest.ClientProfileCodes.Count() < 1)
                            {
                                reportUsers.Remove(reportUser);
                            }
                        }
                    }

                    MonthlyReportSettings monthlyReportSettings = new MonthlyReportSettings();
                    monthlyReportSettings = await rrd.GetDaysInMonthToChekLastMonthlyReport();
                    string cancellationToken = "";

                    ParallelOptions parallelOptions = new()
                    {
                        MaxDegreeOfParallelism = monthlyReportSettings.ParallelSettings
                    };

                    await Parallel.ForEachAsync(reportUsers, parallelOptions, async (reportUser, cancellationToken) => {

                        var reportProcessed = await ProcessReportsLogicFirst(reportUser, monthlyReportSettings, ClientId);

                        //skip when new user
                        if (ClientId == null)
                        {
                            var reportProcessed2 = await ProcessReportsLogicSecond(reportUser, monthlyReportSettings);     
                        }
                    });

                    //skip when new user
                    if (ClientId == null)
                    {
                        await srd.SaveReportProcessingEnd(recordTracking);
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ProcessReportsLogic";
                logError.ClientId = Guid.Empty;
                logError.Parameters = "None. Nightly process for " + DateTime.Now.Date.ToString();
                await logging.WriteToLog(logError);

                return false;
            }
        }

        private async Task<bool> ProcessReportsLogicFirst(ReportUser reportUser, MonthlyReportSettings monthlyReportSettings, Guid? ClientId = null)
        {
            try
            {
                RetrieveReportData getSpecialReportData = new RetrieveReportData();
                List<ReportLoggingByClient> allReportLogging = await getSpecialReportData.GetReportLoggingByClient(reportUser.ClientId);

                //reset authorization
                //get token
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = new APIAuthorization();
                auth = await aPITokenCreation.ReturnRequestTokens(reportUser.aPIAuthorizationRequest);

                //this client's dates
                List<ReportLoggingByClient> reportLogging = new List<ReportLoggingByClient>();
                if (allReportLogging != null && allReportLogging.Count > 0)
                {
                    reportLogging = allReportLogging.Where(x => x.AzClientId == reportUser.ClientId).ToList();
                }

                //add any report logging items that are missing
                foreach (var profileCodeHere in reportUser.aPIAuthorizationRequest.ClientProfileCodes)
                {
                    //string timeZone = reportTimeZones.Where(x => x.CountryId == profileCodeHere.CountryId).FirstOrDefault().TimeZone;
                    TimeZoneUtils timeZoneUtils = new TimeZoneUtils();
                    DateTime currentDateForThisReport = await timeZoneUtils.GetProfileTimeZoneEndDate(profileCodeHere.TimeZone);

                    ReportLoggingByClient thisReportLogging = reportLogging.Where(x => x.CountryId == profileCodeHere.CountryId).FirstOrDefault();


                    if (thisReportLogging == null || thisReportLogging.Id < 1)
                    {
                        ReportLoggingByClient reportLoggingThisClient = new ReportLoggingByClient();
                        reportLoggingThisClient.StartDate = currentDateForThisReport.Date.AddDays(-30);
                        reportLoggingThisClient.LastRunDate = currentDateForThisReport.Date.AddDays(-30);
                        reportLoggingThisClient.Today = currentDateForThisReport.Date;
                        reportLoggingThisClient.CountryId = profileCodeHere.CountryId;
                        reportLoggingThisClient.AzClientId = reportUser.ClientId;

                        //go ahead and put this in the db since there are no records
                        SaveReportData srd = new SaveReportData();
                        reportLoggingThisClient.Id = await srd.SaveReportDataByClientProfileCode(reportLoggingThisClient);

                        reportLogging.Add(reportLoggingThisClient);
                    }
                }

                //update lastrundate on any report items
                foreach(var thisReportItem in reportLogging)
                {
                    string timeZone = reportUser.aPIAuthorizationRequest.ClientProfileCodes.Where(x => x.CountryId == thisReportItem.CountryId).FirstOrDefault().TimeZone;
                    TimeZoneUtils timeZoneUtils = new TimeZoneUtils();
                    DateTime currentDateForThisReport = await timeZoneUtils.GetProfileTimeZoneEndDate(timeZone);

                    thisReportItem.Today = currentDateForThisReport.Date;
                }

                    

                reportUser.aPIAuthorizationRequest.AccessToken = auth.AccessToken;
                reportUser.aPIAuthorizationRequest.TokenExpirationTime = auth.TokenExpirationTime;

                //set up everything we need
                bool keepProccessing = true;

                //make sure we have campaigns and portfolios
                //update any portfolios
                PortfolioRequest portfolioRequest = new PortfolioRequest();
                GetPortfolios getPortfolios = new GetPortfolios();
                PortfolioListResponse portfolioListresponse = new PortfolioListResponse();

                portfolioRequest.Authorization = reportUser.aPIAuthorizationRequest;
                portfolioListresponse = await getPortfolios.GetPortfolioInfo(portfolioRequest);

                //update any campaign info
                GetAllCampaignsLogic getAllCampaignsLogic = new GetAllCampaignsLogic();

                //for each country
                foreach (var profileCode in reportUser.aPIAuthorizationRequest.ClientProfileCodes)
                {
                    var getCampaignsResponse = await getAllCampaignsLogic.GetAllCampaigns(reportUser.aPIAuthorizationRequest, profileCode);
                }


                List<ReportIds> allReportIds = new List<ReportIds>();
                foreach (var profileCode in reportUser.aPIAuthorizationRequest.ClientProfileCodes)
                {
                    ReportIds reportIds = new ReportIds();
                    reportIds.ClientProfileCode = profileCode;
                    allReportIds.Add(reportIds);
                }


                //date prep for reports
                DateTime reportRunDate = DateTime.Now.AddDays(-endDateDaysToSubtractForRegineratedReports).Date;

                //get the report ids
                foreach (var reportIdRecords in allReportIds)
                {

                    //get the report date info for this country
                    ReportLoggingByClient thisReportLogging = reportLogging.Where(x => x.CountryId == reportIdRecords.ClientProfileCode.CountryId).FirstOrDefault();

                    if (thisReportLogging != null && thisReportLogging.Id != 0)
                    {
                        CreateReportIds createReportIds = new CreateReportIds();

                        //get kewyords from snapshot
                        //reportIdRecords.KeywordSnapshotId = await createReportIds.CreateSnapshot(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, "keywords");

                        //get ad groups for snapshot
                        reportIdRecords.AdGroupSnapshotId = await createReportIds.CreateSnapshot(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, "adGroups");

                        //get getproductTargets for snapshot
                        reportIdRecords.ProductTargetSnapshotId = await createReportIds.CreateSnapshot(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, "targets");


                        //daily report - create id - always run to current
                        reportIdRecords.DailyReportId = await createReportIds.CreateDailyReportId(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing);


                        //monthly summary report for previous month - only process for first three days of month and not for new clients - if I need to expand the window to catch up, just change the 3 below to include more days
                        if (thisReportLogging.StartDate.Month != thisReportLogging.Today.Month && thisReportLogging.Today.Day <= monthlyReportSettings.DaysInMonthToProcess && (ClientId == null || ClientId == Guid.Empty))
                        {
                            reportIdRecords.LastMonthlyReportId = await createReportIds.CreateMonthlyReportId(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, true);
                            reportIdRecords.ProcessLastMonth = true;

                            reportIdRecords.KeywordLastMonthlyReportId = await createReportIds.CreateMonthlyReportIdForKeywords(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, true);
                        }

                        //monthly report id - always run to current
                        reportIdRecords.MonthlyReportId = await createReportIds.CreateMonthlyReportId(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing);

                        reportIdRecords.KeywordMonthlyReportId = await createReportIds.CreateMonthlyReportIdForKeywords(thisReportLogging, reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing);

                        await System.Threading.Tasks.Task.Delay(1000);
                    }
                }

                //get the report urls
                GetReports getReports = new GetReports();
                bool reportUrlComplete = await getReports.GetReportUrls(reportUser.aPIAuthorizationRequest, reportUser, allReportIds);

                //now process the reports
                foreach (var reportIdRecords in allReportIds)
                {
                    //create daily report
                    if (!string.IsNullOrEmpty(reportIdRecords.DailyReportUrl))
                    {
                        //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                        await System.Threading.Tasks.Task.Delay(1000);

                        DailyReportLogic dailyReportLogic = new DailyReportLogic();
                        var summarySuccess = await dailyReportLogic.ProccessReport(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.DailyReportUrl);
                    }

                    //create last month report
                    if (!string.IsNullOrEmpty(reportIdRecords.LastMonthlyReportUrl))
                    {
                        //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                        await System.Threading.Tasks.Task.Delay(1000);

                        MonthlySummaryReportLogic monthlySummaryReportLogic = new MonthlySummaryReportLogic();
                        var summarySuccess = await monthlySummaryReportLogic.ProccessReport(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.LastMonthlyReportUrl, true);
                    }

                    //create monthly report
                    if (!string.IsNullOrEmpty(reportIdRecords.MonthlyReportUrl))
                    {
                        //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                        await System.Threading.Tasks.Task.Delay(1000);

                        MonthlySummaryReportLogic monthlyReportLogic = new MonthlySummaryReportLogic();
                        var summarySuccess = await monthlyReportLogic.ProccessReport(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.MonthlyReportUrl);
                    }

                    //create last month report for keywords
                    if (!string.IsNullOrEmpty(reportIdRecords.KeywordLastMonthlyReportUrl))
                    {
                        //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                        await System.Threading.Tasks.Task.Delay(1000);

                        MonthlySummaryReportLogicForKeywords monthlySummaryReportLogic = new MonthlySummaryReportLogicForKeywords();
                        var summarySuccess = await monthlySummaryReportLogic.ProccessReport(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.KeywordLastMonthlyReportUrl, true);
                    }

                    //create monthly report for keywords
                    if (!string.IsNullOrEmpty(reportIdRecords.KeywordMonthlyReportUrl))
                    {
                        //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                        await System.Threading.Tasks.Task.Delay(1000);

                        MonthlySummaryReportLogicForKeywords monthlyReportLogic = new MonthlySummaryReportLogicForKeywords();
                        var summarySuccess = await monthlyReportLogic.ProccessReport(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.KeywordMonthlyReportUrl);
                    }

                    //process keyword snapshot - no longer save locally
                    //if (!string.IsNullOrEmpty(reportIdRecords.KeywordSnapshotUrl))
                    //{
                    //    KeywordSnapshotLogic keywordSnapshotLogic = new KeywordSnapshotLogic();
                    //    var keywordSnapshotSuccess = await keywordSnapshotLogic.ProccessSnapshot(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.KeywordSnapshotUrl);
                    //}

                    if (!string.IsNullOrEmpty(reportIdRecords.ProductTargetSnapshotUrl))
                    {
                        //process product target snapshot
                        ProductTargetSnapshotLogic productTargetSnapshotLogic = new ProductTargetSnapshotLogic();
                        var productTargetSnapshotSuccess = await productTargetSnapshotLogic.ProccessSnapshot(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.ProductTargetSnapshotUrl);
                    }

                    if (!string.IsNullOrEmpty(reportIdRecords.AdGroupSnapshotUrl))
                    {
                        //process ad group snapshot
                        AdGroupSnapshotLogic adGroupSnapshotLogic = new AdGroupSnapshotLogic();
                        var adgroupSnapshotSuccess = await adGroupSnapshotLogic.ProccessSnapshot(reportUser.aPIAuthorizationRequest, reportUser, reportIdRecords.ClientProfileCode, keepProccessing, reportIdRecords.AdGroupSnapshotUrl);
                    }


                    //save reportdatesbyprofile
                    ReportLoggingByClient reportLoggingThisClient = reportLogging.Where(x => x.CountryId == reportIdRecords.ClientProfileCode.CountryId).FirstOrDefault();
                    SaveReportData srd = new SaveReportData();
                    var reconcileSuccess = await srd.UpdateLastProcessingReportDate(reportLoggingThisClient);
                }

               
                //make sure tier 1 and performance campaigns are active and report if they aren't
                var response = await MakeSureT1AndPActive(reportUser);

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ProcessReportsLogicFirst";
                logError.ClientId = reportUser.ClientId;
                logError.Parameters = "None. Nightly process for " + DateTime.Now.Date.ToString();
                await logging.WriteToLog(logError);
            }
            
            return true;

           
        }

        //make sure t1 and performance campaigns are active
        public async Task<bool> MakeSureT1AndPActive(ReportUser reportUser)
        {
            try
            {
                RetrieveNightlyExtras retrieveNightlyExtras = new RetrieveNightlyExtras();
                List<DisabledCampaign> disabledCampaigns = new List<DisabledCampaign>();

                disabledCampaigns = await retrieveNightlyExtras.GetDisabledPerformanceCampaigns(reportUser.ClientId);

                if (disabledCampaigns != null && disabledCampaigns.Count > 0)
                {
                    SaveKeywordManagementData saveKeywordManagementData = new SaveKeywordManagementData();

                    List<SaveActionRequired> saveActionsRequired = new List<SaveActionRequired>();
                    foreach (var campaign in disabledCampaigns)
                    {
                        SaveActionRequired saveActionRequired = new SaveActionRequired();
                        saveActionRequired.ActionId = 2;
                        saveActionRequired.AzCampaignId = campaign.CampaignId;
                        saveActionRequired.Resolved = false;
                        saveActionRequired.ClientId = reportUser.ClientId;
                        saveActionRequired.CountryId = campaign.CountryId;
                        saveActionRequired.Description = "Promotions for the related product will not work when the Tier 1 or Performance campaign is disabled.";
                        saveActionsRequired.Add(saveActionRequired);
                    }

                    var actionsRequiredSaved = await saveKeywordManagementData.SaveActionsRequired(saveActionsRequired, reportUser);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "MakeSureT1AndPActive";
                logError.ClientId = Guid.Empty;
                logError.Parameters = System.Text.Json.JsonSerializer.Serialize(reportUser);
                await logging.WriteToLog(logError);

                return false;
            }
        }


        private async Task<bool> ProcessReportsLogicSecond(ReportUser reportUser, MonthlyReportSettings monthlyReportSettings)
        {
            try
            {
                //reconcile cosmos
                ReconcileProductsOnCosmos reconcileProductsOnCosmos = new ReconcileProductsOnCosmos();
                await reconcileProductsOnCosmos.Reconcile(reportUser.ClientId);

                //calculate billing
                var overageResult = await ProcessOverage(reportUser.ClientId);

                Regex regexSearch = new Regex(@"^(b[\da-z]{9}|\d{9}(X|\d))$");

                //make response items to handle later
                List<SaveActionRequired> saveActionRequireds = new List<SaveActionRequired>();
                List<SaveSummaryReportAction> saveSummaryReportActions = new List<SaveSummaryReportAction>();
                List<SaveKeywordHistory> saveKeywordHistories = new List<SaveKeywordHistory>();

                //add any campaigns without a product to actions required
                RetrieveNightlyExtras retrieveNightlyExtras = new RetrieveNightlyExtras();
                var campaignsToAssign = await retrieveNightlyExtras.GetUnassignedCampaigns(reportUser.ClientId);

                foreach (var st in campaignsToAssign)
                {
                    SaveActionRequired saveActionRequired = new SaveActionRequired();
                    saveActionRequired.ActionId = 1;
                    saveActionRequired.AzCampaignId = st.CampaignId;
                    saveActionRequired.CountryId = st.CountryId;
                    saveActionRequired.Description = "Campaign With Activity Missing Product Assignment";
                    saveActionRequired.ClientId = reportUser.ClientId;

                    saveActionRequireds.Add(saveActionRequired);

                }

                //make all items needed for processing
                RetrieveKeywordManagementData retrieveKeywordManagementData = new RetrieveKeywordManagementData();

                //all search terms for updating
                List<int> CountriesToUpdateForSearchTerms = new List<int>();
                foreach(var id in reportUser.aPIAuthorizationRequest.ClientProfileCodes)
                {
                    CountriesToUpdateForSearchTerms.Add(id.CountryId);
                }

                DateTime CreationDate = DateTime.Now.AddYears(-2);
                List<AllSearchTerms> allSearchTermsRaw = await retrieveKeywordManagementData.GetAllSearchTerms(reportUser.ClientId, CreationDate);
                List<AllSearchTerms> allSearchTerms = allSearchTermsRaw.Where(x => CountriesToUpdateForSearchTerms.Contains(x.Country)).ToList();

                //get campaign, product, and ad group relationships
                List<CampaignProductRelationships> campaignProductRelationships = await retrieveKeywordManagementData.GetCampaignProductRelationships(reportUser.ClientId);

                //get campaign active statuses
                List<CampaignActiveStatus> campaignActiveStatuses = new List<CampaignActiveStatus>();

                foreach (var productRelationship in campaignProductRelationships)
                {
                    //see if promotion campaigns are active
                    CampaignActiveStatus campaignActiveStatus = new CampaignActiveStatus();
                    campaignActiveStatus.AzCampaignId = productRelationship.azspcampaignid;
                    campaignActiveStatus.ClientId = productRelationship.ClientId;
                    campaignActiveStatus.Active = await retrieveKeywordManagementData.GetCampaignActiveStatus(reportUser.ClientId, productRelationship.azspcampaignid, productRelationship.CountryId);

                    campaignActiveStatuses.Add(campaignActiveStatus);
                }

                //get promotion and negative rules by country
                List<PromoNegativeRules> promoNegativeRules = await retrieveKeywordManagementData.GetPromoNegativeRules(reportUser.ClientId);

                //adjust any bids - disabled, turn this back on if I want to keep it
                //BidAdjustment bidAdjustment = new BidAdjustment();
                //List<SaveKeywordHistory> bidHistory = await bidAdjustment.AdjustBid(reportUser, promoNegativeRules);
                //if (bidHistory != null && bidHistory.Count > 0)
                //    saveKeywordHistories = saveKeywordHistories.Union(bidHistory).ToList();


                //response actions
                List<SaveSummaryReportAction> amazonActions = new List<SaveSummaryReportAction>();
                List<SummedSearchTerms> summedSearchTermsHere = new List<SummedSearchTerms>();
                List<AllSearchTerms> allSearchTermsHere = new List<AllSearchTerms>();
                List<SummedSearchTerms> summedSearchTermsWithoutProductHere = new List<SummedSearchTerms>();

                //payment plan 1 is the basic tier. We will manage all campaigns on all other plans
                if (reportUser.PaymentPlan > 1)
                {
                    allSearchTermsHere = allSearchTerms;

                    summedSearchTermsHere = (from t in allSearchTerms
                                                group t by new { t.SearchTerm, t.ProductId, t.ClientId, t.Country } into grp
                                                select new SummedSearchTerms
                                                {
                                                    SearchTerm = grp.Key.SearchTerm,
                                                    ProductId = grp.Key.ProductId,
                                                    ClientId = grp.Key.ClientId,
                                                    Country = grp.Key.Country,
                                                    Clicks = grp.Sum(t => t.Clicks),
                                                    Orders = grp.Sum(t => t.Orders),
                                                    Pages = grp.Sum(t => t.Pages),
                                                }).ToList();

                    summedSearchTermsWithoutProductHere = (from t in allSearchTerms
                                                            group t by new { t.SearchTerm, t.ClientId, t.Country } into grp
                                                            select new SummedSearchTerms
                                                            {
                                                                SearchTerm = grp.Key.SearchTerm,
                                                                ClientId = grp.Key.ClientId,
                                                                Country = grp.Key.Country,
                                                                Clicks = grp.Sum(t => t.Clicks),
                                                                Orders = grp.Sum(t => t.Orders),
                                                                Pages = grp.Sum(t => t.Pages),
                                                            }).ToList();

                }
                else
                {
                    allSearchTermsHere = allSearchTerms.Where(x => x.GeneratedByUs == true).ToList();

                    summedSearchTermsHere = (from t in allSearchTerms.Where(x => x.GeneratedByUs)
                                                group t by new { t.SearchTerm, t.ProductId, t.ClientId, t.Country } into grp
                                                select new SummedSearchTerms
                                                {
                                                    SearchTerm = grp.Key.SearchTerm,
                                                    ProductId = grp.Key.ProductId,
                                                    ClientId = grp.Key.ClientId,
                                                    Country = grp.Key.Country,
                                                    Clicks = grp.Sum(t => t.Clicks),
                                                    Orders = grp.Sum(t => t.Orders),
                                                    Pages = grp.Sum(t => t.Pages),
                                                }).ToList();

                    summedSearchTermsWithoutProductHere = (from t in allSearchTerms.Where( x =>  x.GeneratedByUs)
                                                            group t by new { t.SearchTerm, t.ClientId, t.Country } into grp
                                                            select new SummedSearchTerms
                                                            {
                                                                SearchTerm = grp.Key.SearchTerm,
                                                                ClientId = grp.Key.ClientId,
                                                                Country = grp.Key.Country,
                                                                Clicks = grp.Sum(t => t.Clicks),
                                                                Orders = grp.Sum(t => t.Orders),
                                                                Pages = grp.Sum(t => t.Pages),
                                                            }).ToList();
                }


                foreach (var searchTerm in summedSearchTermsWithoutProductHere)
                {
                    if (searchTerm.Pages > 0 || searchTerm.Orders > 0 || searchTerm.Clicks > 0)
                    {
                        //we won't consider any products for any search terms if any of those related campaigns aren't associated with a product, since we won't know how to handle them
                        List<AllSearchTerms> searchTermCampaignIdentification = allSearchTermsHere.Where(x => x.SearchTerm == searchTerm.SearchTerm && x.Country == searchTerm.Country).ToList();
                        List<AllSearchTerms> searchTermCampaignsWithoutProduct = searchTermCampaignIdentification.Where(x => x.ProductId == 0).ToList();

                        if (searchTermCampaignsWithoutProduct.Count > 0)
                        {
                            //at least one campaign with relevant activity is missing an assignment. Skip it.
                        }
                        else
                        {
                            //work through all matching country/product/search terms groupings
                            var relevantSummedSearchTerms = summedSearchTermsHere.Where(x => x.SearchTerm == searchTerm.SearchTerm && x.Country == searchTerm.Country);

                            foreach(var secondSearchTerm in relevantSummedSearchTerms)
                            {
                                //each one of these search terms is grouped by country and product
                                //lock around anything I add to list outside of parallel
                                //make sure I'm working with "here" versions of keywords

                                //relevant rules for this search term in this country
                                var relevantRules = promoNegativeRules.Where(x => x.CountryID == secondSearchTerm.Country && x.QAPProductID == secondSearchTerm.ProductId && x.ClientId == secondSearchTerm.ClientId).FirstOrDefault();


                                if (relevantRules != null)
                                {
                                    //bool markedAsNegative = false;

                                    //I've broken these out to make them easier to read
                                    decimal expectedConversion = 0;
                                    if (relevantRules.ConversionGoal > 0)
                                    {
                                        expectedConversion = ((decimal)100.00 / (decimal)relevantRules.ConversionGoal);
                                    }
                                    bool excludeAudibleFromNegative = relevantRules.ExcludeAudibleKeywordsFromNegative;
                                    bool applyNegative = relevantRules.ApplyNegative;

                                    //negatives
                                    if (expectedConversion > 0 && applyNegative)
                                    {
                                        //do negatives with audible exclusion based on user preferences
                                        if ((!secondSearchTerm.SearchTerm.ToLower().Contains("audible")) || ((secondSearchTerm.SearchTerm.ToLower().Contains("audible")) && !excludeAudibleFromNegative))
                                        {

                                            decimal conversionGoal = (decimal)relevantRules.ConversionGoal;

                                            //we've reached twice the allowed click count, but not three times
                                            if ((secondSearchTerm.Clicks > (expectedConversion * 2)) && (secondSearchTerm.Clicks < (expectedConversion * 3)))
                                            {

                                                // if not at least one sale - get all campaign ids with this search term as well as all record ids and create an object summaryreportid, azcampaignid, setnegative
                                                // if not at least one sale - create keywordhistory object with dateprocessed, countryid, searchterm, productid, action (id), reason (text), clientid
                                                if (secondSearchTerm.Orders < 1)
                                                {
                                                    //markedAsNegative = true;

                                                    SaveKeywordHistory saveKeywordHistory = new SaveKeywordHistory();
                                                    saveKeywordHistory.CountryId = secondSearchTerm.Country;
                                                    saveKeywordHistory.SearchTerm = secondSearchTerm.SearchTerm;
                                                    saveKeywordHistory.ProductId = secondSearchTerm.ProductId;
                                                    saveKeywordHistory.Action = 2;
                                                    saveKeywordHistory.Reason = "Keyword/ASIN set as negative as it is performing below conversion threshold.";
                                                    saveKeywordHistory.ClientId = secondSearchTerm.ClientId;
                                                    saveKeywordHistory.DateProcessed = DateTime.Now.Date;
                                                    saveKeywordHistories.Add(saveKeywordHistory);

                                                    //get all records to update
                                                    List<AllSearchTerms> allSearchTermsToUpdate = allSearchTermsHere.Where(x => x.SearchTerm == secondSearchTerm.SearchTerm && x.Country == secondSearchTerm.Country && x.ProductId == secondSearchTerm.ProductId).ToList();

                                                    foreach (var SearchTermToUpdate in allSearchTermsToUpdate)
                                                    {
                                                        ////commmented out for negative
                                                        //SaveSummaryReportAction action = new SaveSummaryReportAction();
                                                        //action.keyword = SearchTermToUpdate.Keyword;
                                                        //action.keywordType = SearchTermToUpdate.KeywordType;
                                                        //action.AzCampaignId = SearchTermToUpdate.CampaignId;
                                                        //action.Negative = true;
                                                        //action.CountryId = SearchTermToUpdate.Country;
                                                        //action.SearchTerm = secondSearchTerm.SearchTerm;
                                                        //action.AdGroup = SearchTermToUpdate.AdGroupId;
                                                        //action.KeywordId = SearchTermToUpdate.Keyword;
                                                            
                                                        //if any of these items match the targeting expression or complement, then it is a product
                                                        AllSearchTerms searchTermIsAsinFirst2 = allSearchTermsToUpdate.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinSecond2 = allSearchTermsToUpdate.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.Keyword.ToLower() == "complements").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinThird2 = allSearchTermsToUpdate.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.SearchTerm.Length == 10 && !x.SearchTerm.Contains(" ")).FirstOrDefault();
                                                        string thisSearchTerm = allSearchTermsToUpdate.FirstOrDefault().SearchTerm;

                                                        //if (((searchTermIsAsinFirst2 != null && !string.IsNullOrEmpty(searchTermIsAsinFirst2.SearchTerm)) || (searchTermIsAsinSecond2 != null && !string.IsNullOrEmpty(searchTermIsAsinSecond2.SearchTerm)) || (searchTermIsAsinThird2 != null && !string.IsNullOrEmpty(searchTermIsAsinThird2.SearchTerm))) && regexSearch.Match(thisSearchTerm).Success)
                                                        //{
                                                        //    action.Product = true;
                                                        //}
                                                        //else
                                                        //{
                                                        //    action.Product = false;
                                                        //}



                                                        //saveSummaryReportActions.Add(action);
                                                        //amazonActions.Add(action);
                                                    }
                                                }
                                            }
                                            else if (secondSearchTerm.Clicks > (expectedConversion * 3))
                                            {

                                                // if not within one sale of the goal - get all campaign ids with this search term as well as all record ids and create an object summaryreportid, azcampaignid, setnegative
                                                // if not within one sale of the goal - create keywordhistory object with dateprocessed, countryid, searchterm, productid, action (id), reason (text), clientid
                                                int numberOfSales = Convert.ToInt32(Math.Floor(Convert.ToDecimal((decimal)secondSearchTerm.Clicks / (decimal)expectedConversion)));

                                                if (secondSearchTerm.Orders < (numberOfSales - 1))
                                                {
                                                    //markedAsNegative = true;

                                                    SaveKeywordHistory saveKeywordHistory = new SaveKeywordHistory();
                                                    saveKeywordHistory.CountryId = secondSearchTerm.Country;
                                                    saveKeywordHistory.SearchTerm = secondSearchTerm.SearchTerm;
                                                    saveKeywordHistory.ProductId = secondSearchTerm.ProductId;
                                                    saveKeywordHistory.Action = 2;
                                                    saveKeywordHistory.Reason = "Keyword/ASIN set as negative as it is performing below conversion threshold.";
                                                    saveKeywordHistory.ClientId = secondSearchTerm.ClientId;
                                                    saveKeywordHistory.DateProcessed = DateTime.Now.Date;
                                                    saveKeywordHistories.Add(saveKeywordHistory);

                                                    //get all records to update
                                                    List<AllSearchTerms> allSearchTermsToUpdate = allSearchTermsHere.Where(x => x.SearchTerm == secondSearchTerm.SearchTerm && x.Country == secondSearchTerm.Country && x.ProductId == secondSearchTerm.ProductId).ToList();

                                                    foreach (var SearchTermToUpdate in allSearchTermsToUpdate)
                                                    {
                                                        ////commmented out for negative
                                                        //SaveSummaryReportAction action = new SaveSummaryReportAction();
                                                        //action.keyword = SearchTermToUpdate.Keyword;
                                                        //action.keywordType = SearchTermToUpdate.KeywordType;
                                                        //action.AzCampaignId = SearchTermToUpdate.CampaignId;
                                                        //action.Negative = true;
                                                        //action.CountryId = SearchTermToUpdate.Country;
                                                        //action.SearchTerm = secondSearchTerm.SearchTerm;
                                                        //action.AdGroup = SearchTermToUpdate.AdGroupId;
                                                        //action.KeywordId = SearchTermToUpdate.KeywordId;
                                                            
                                                        //if any of these items match the targeting expression or complement, then it is a product
                                                        AllSearchTerms searchTermIsAsinFirst2 = allSearchTermsToUpdate.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinSecond2 = allSearchTermsToUpdate.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.Keyword.ToLower() == "complements").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinThird2 = allSearchTermsToUpdate.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.SearchTerm.Length == 10 && !x.SearchTerm.Contains(" ")).FirstOrDefault();
                                                        string thisSearchTerm = allSearchTermsToUpdate.FirstOrDefault().SearchTerm;

                                                        ////if (((searchTermIsAsinFirst2 != null && !string.IsNullOrEmpty(searchTermIsAsinFirst2.SearchTerm)) || (searchTermIsAsinSecond2 != null && !string.IsNullOrEmpty(searchTermIsAsinSecond2.SearchTerm)) || (searchTermIsAsinThird2 != null && !string.IsNullOrEmpty(searchTermIsAsinThird2.SearchTerm))) && regexSearch.Match(thisSearchTerm).Success)
                                                        ////{
                                                        ////    action.Product = true;
                                                        ////}
                                                        ////else
                                                        ////{
                                                        ////    action.Product = false;
                                                        ////}

                                                        ////saveSummaryReportActions.Add(action);
                                                        ////amazonActions.Add(action);
                                                    }
                                                }
                                            }

                                        }

                                    }

                                    //positives
                                    int tier1TresholdSales = relevantRules.Tier1TresholdSales;
                                    int Tier1TresholdPageReads = relevantRules.Tier1TresholdPageReads;
                                    int performTresholdSales = relevantRules.PerformTresholdSales;
                                    int performTresholdPageReads = relevantRules.PerformTresholdPageReads;
                                    decimal tier1DefaultBid = (decimal)relevantRules.Tier1DefaultBid;
                                    decimal performanceDefBid = (decimal)relevantRules.PerformanceDefBid;

                                    CampaignProductRelationships tier1Campaign = campaignProductRelationships.Where(x => x.CountryId == secondSearchTerm.Country && x.ProductId == secondSearchTerm.ProductId && x.ClientId == secondSearchTerm.ClientId && x.CampaignUsageType == 2 && x.PrimaryInUsageType == true).FirstOrDefault();
                                    CampaignProductRelationships performanceCampaign = campaignProductRelationships.Where(x => x.CountryId == secondSearchTerm.Country && x.ProductId == secondSearchTerm.ProductId && x.ClientId == secondSearchTerm.ClientId && x.CampaignUsageType == 3 && x.PrimaryInUsageType == true).FirstOrDefault();

                                    CampaignActiveStatus tier1ActiveStatus = new CampaignActiveStatus();
                                    CampaignActiveStatus performanceActiveStatus = new CampaignActiveStatus();

                                    try
                                    {
                                        tier1ActiveStatus = campaignActiveStatuses.Where(x => x.AzCampaignId == tier1Campaign.azspcampaignid && x.ClientId == tier1Campaign.ClientId).FirstOrDefault();
    
                                    }
                                    catch(Exception ex)
                                    {
                                        tier1ActiveStatus.Active = false;
                                    }

                                    try
                                    {
                                        performanceActiveStatus = campaignActiveStatuses.Where(x => x.AzCampaignId == performanceCampaign.azspcampaignid && x.ClientId == performanceCampaign.ClientId).FirstOrDefault();

                                    }
                                    catch(Exception ex)
                                    {
                                        performanceActiveStatus.Active = false;
                                    }

                                    //removed since I'm not applying negatives - if (!markedAsNegative && performanceActiveStatus.Active && tier1ActiveStatus.Active)
                                    if (performanceActiveStatus.Active && tier1ActiveStatus.Active)
                                    {

                                        //if ((secondSearchTerm.Pages >= Tier1TresholdPageReads && Tier1TresholdPageReads != 0) || (secondSearchTerm.Pages >= performTresholdPageReads && performTresholdPageReads != 0) || (secondSearchTerm.Orders >= tier1TresholdSales && tier1TresholdSales != 0) || (secondSearchTerm.Orders >= performTresholdSales && performTresholdSales != 0))
                                        if (secondSearchTerm.Clicks >= 1)
                                        {
                                            List<AllSearchTerms> allSearchTermsToConsiderHere = allSearchTermsHere.Where(x => x.SearchTerm == secondSearchTerm.SearchTerm && x.Country == secondSearchTerm.Country && x.ProductId == secondSearchTerm.ProductId).ToList();

                                            //look for performance campaign

                                            //get all search terms where there are no performance campaigns
                                            List<string> perfromanceCampaigns = campaignProductRelationships.Where(x => x.CountryId == secondSearchTerm.Country && x.ProductId == secondSearchTerm.ProductId && x.ClientId == secondSearchTerm.ClientId && x.CampaignUsageType == 3).Select(c => c.azspcampaignid).ToList();
                                            List<AllSearchTerms> performanceCampaignAlreadyPromoted = allSearchTermsToConsiderHere.Where(x => perfromanceCampaigns.Contains(x.CampaignId)).ToList();

                                            //get all search terms are already in tier 1
                                            List<string> tier1Campaigns = campaignProductRelationships.Where(x => x.CountryId == secondSearchTerm.Country && x.ProductId == secondSearchTerm.ProductId && x.ClientId == secondSearchTerm.ClientId && x.CampaignUsageType == 2).Select(c => c.azspcampaignid).ToList();
                                            List<AllSearchTerms> tier1CampaignAlreadyPromoted = allSearchTermsToConsiderHere.Where(x => tier1Campaigns.Contains(x.CampaignId)).ToList();

                                            //get all search terms that are not in any performance campaign
                                            List<AllSearchTerms> allCampaignsButPerformance = allSearchTermsToConsiderHere.Where(x => !perfromanceCampaigns.Contains(x.CampaignId)).ToList();

                                            //get all search terms that are not in a performance or tier 1 campaign
                                            List<AllSearchTerms> allCampaignsButPerformanceOrTier1 = allSearchTermsToConsiderHere.Where(x => !tier1Campaigns.Contains(x.CampaignId) && !perfromanceCampaigns.Contains(x.CampaignId)).ToList();

                                            //if already in a performace campaign, add it as a negative to lower campaigns
                                            if (performanceCampaignAlreadyPromoted != null && performanceCampaignAlreadyPromoted.Count > 0)
                                            {
                                                if (allCampaignsButPerformance.Count > 0)
                                                {
                                                    SaveKeywordHistory saveKeywordHistory = new SaveKeywordHistory();
                                                    saveKeywordHistory.CountryId = secondSearchTerm.Country;
                                                    saveKeywordHistory.SearchTerm = secondSearchTerm.SearchTerm;
                                                    saveKeywordHistory.ProductId = secondSearchTerm.ProductId;
                                                    saveKeywordHistory.Action = 1;
                                                    saveKeywordHistory.Reason = "Keyword/ASIN located in performace campaign and negative applied to lower campaigns.";
                                                    saveKeywordHistory.ClientId = secondSearchTerm.ClientId;
                                                    saveKeywordHistory.DateProcessed = DateTime.Now.Date;
                                                    saveKeywordHistories.Add(saveKeywordHistory);

                                                    foreach (var SearchTermToUpdate in allCampaignsButPerformance)
                                                    {
                                                        ////commmented out for negative
                                                        //SaveSummaryReportAction action = new SaveSummaryReportAction();
                                                        //action.keyword = SearchTermToUpdate.Keyword;
                                                        //action.keywordType = SearchTermToUpdate.KeywordType;
                                                        //action.AzCampaignId = SearchTermToUpdate.CampaignId;
                                                        //action.Negative = true;
                                                        //action.CountryId = SearchTermToUpdate.Country;
                                                        //action.SearchTerm = secondSearchTerm.SearchTerm;
                                                        //action.AdGroup = SearchTermToUpdate.AdGroupId;
                                                        //action.KeywordId = SearchTermToUpdate.KeywordId;
                                                            
                                                        //if any of these items match the targeting expression or complement, then it is a product
                                                        AllSearchTerms searchTermIsAsinFirst = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinSecond = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.Keyword.ToLower() == "complements").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinThird = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.SearchTerm.Length == 10 && !x.SearchTerm.Contains(" ")).FirstOrDefault();
                                                        string thisSearchTerm = allSearchTermsToConsiderHere.FirstOrDefault().SearchTerm;

                                                        //if (((searchTermIsAsinFirst != null && !string.IsNullOrEmpty(searchTermIsAsinFirst.SearchTerm)) || (searchTermIsAsinSecond != null && !string.IsNullOrEmpty(searchTermIsAsinSecond.SearchTerm)) || (searchTermIsAsinThird != null && !string.IsNullOrEmpty(searchTermIsAsinThird.SearchTerm))) && regexSearch.Match(thisSearchTerm).Success)
                                                        //{
                                                        //    action.Product = true;
                                                        //}
                                                        //else
                                                        //{
                                                        //    action.Product = false;
                                                        //}

                                                        //saveSummaryReportActions.Add(action);
                                                        //amazonActions.Add(action);
                                                    }
                                                }

                                            }
                                            //if it is already in tier 1, check performance thresholds, add to peformance, and set as negative on any existing campaigns

                                            else if (tier1CampaignAlreadyPromoted != null && tier1CampaignAlreadyPromoted.Count > 0)
                                            {
                                                if ((secondSearchTerm.Pages >= performTresholdPageReads && performTresholdPageReads != 0) || (secondSearchTerm.Orders >= performTresholdSales && performTresholdSales != 0))
                                                //if (secondSearchTerm.Clicks >= 1)
                                                {
                                                    SaveKeywordHistory saveKeywordHistory = new SaveKeywordHistory();
                                                    saveKeywordHistory.CountryId = secondSearchTerm.Country;
                                                    saveKeywordHistory.SearchTerm = secondSearchTerm.SearchTerm;
                                                    saveKeywordHistory.ProductId = secondSearchTerm.ProductId;
                                                    saveKeywordHistory.Action = 1;
                                                    saveKeywordHistory.Reason = "Keyword/ASIN promoted to performace campaign.";
                                                    saveKeywordHistory.ClientId = secondSearchTerm.ClientId;
                                                    saveKeywordHistory.DateProcessed = DateTime.Now.Date;
                                                    saveKeywordHistories.Add(saveKeywordHistory);

                                                    //apply negatives for this promotion
                                                    foreach (var SearchTermToUpdate in allCampaignsButPerformance)
                                                    {
                                                        ////commmented out for negative
                                                        //SaveSummaryReportAction action = new SaveSummaryReportAction();
                                                        //action.keyword = SearchTermToUpdate.Keyword;
                                                        //action.keywordType = SearchTermToUpdate.KeywordType;
                                                        //action.AzCampaignId = SearchTermToUpdate.CampaignId;
                                                        //action.Negative = true;
                                                        //action.CountryId = SearchTermToUpdate.Country;
                                                        //action.SearchTerm = secondSearchTerm.SearchTerm;
                                                        //action.AdGroup = SearchTermToUpdate.AdGroupId;
                                                            
                                                        ////if any of these items match the targeting expression or complement, then it is a product
                                                        AllSearchTerms searchTermIsAsinFirst = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinSecond = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.Keyword.ToLower() == "complements").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinThird = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.SearchTerm.Length == 10 && !x.SearchTerm.Contains(" ")).FirstOrDefault();
                                                        string thisSearchTerm2 = allSearchTermsToConsiderHere.FirstOrDefault().SearchTerm;

                                                        //if (((searchTermIsAsinFirst != null && !string.IsNullOrEmpty(searchTermIsAsinFirst.SearchTerm)) || (searchTermIsAsinSecond != null && !string.IsNullOrEmpty(searchTermIsAsinSecond.SearchTerm)) || (searchTermIsAsinThird != null && !string.IsNullOrEmpty(searchTermIsAsinThird.SearchTerm))) && regexSearch.Match(thisSearchTerm2).Success)
                                                        //{
                                                        //    action.Product = true;
                                                        //}
                                                        //else
                                                        //{
                                                        //    action.Product = false;
                                                        //}

                                                        //amazonActions.Add(action);

                                                        SaveSummaryReportAction action3 = new SaveSummaryReportAction();
                                                        action3.keyword = SearchTermToUpdate.Keyword;
                                                        action3.keywordType = SearchTermToUpdate.KeywordType;
                                                        action3.AzCampaignId = SearchTermToUpdate.CampaignId;
                                                        action3.Promoted = true;
                                                        action3.CountryId = SearchTermToUpdate.Country;
                                                        action3.SearchTerm = secondSearchTerm.SearchTerm;
                                                        action3.AdGroup = SearchTermToUpdate.AdGroupId;
                                                        action3.KeywordId = SearchTermToUpdate.KeywordId;
                                                            
                                                        //if any of these items match the targeting expression or complement, then it is a product
                                                        if (((searchTermIsAsinFirst != null && !string.IsNullOrEmpty(searchTermIsAsinFirst.SearchTerm)) || (searchTermIsAsinSecond != null && !string.IsNullOrEmpty(searchTermIsAsinSecond.SearchTerm)) || (searchTermIsAsinThird != null && !string.IsNullOrEmpty(searchTermIsAsinThird.SearchTerm))) && regexSearch.Match(thisSearchTerm2).Success)
                                                        {
                                                            action3.Product = true;
                                                        }
                                                        else
                                                        {
                                                            action3.Product = false;
                                                        }

                                                        saveSummaryReportActions.Add(action3);
                                                    }

                                                    //apply performance promotion
                                                    SaveSummaryReportAction action2 = new SaveSummaryReportAction();
                                                    action2.AzCampaignId = performanceCampaign.azspcampaignid;
                                                    action2.Promoted = true;
                                                    action2.CountryId = secondSearchTerm.Country;
                                                    action2.SearchTerm = secondSearchTerm.SearchTerm;
                                                    action2.DefaultBid = performanceDefBid;
                                                        
                                                    //if any of these items match the targeting expression or complement, then it is a product
                                                    AllSearchTerms searchTermIsAsinFirst2 = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION").FirstOrDefault();
                                                    AllSearchTerms searchTermIsAsinSecond2 = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.Keyword.ToLower() == "complements").FirstOrDefault();
                                                    AllSearchTerms searchTermIsAsinThird2 = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.SearchTerm.Length == 10 && !x.SearchTerm.Contains(" ")).FirstOrDefault();
                                                    string thisSearchTerm = allSearchTermsToConsiderHere.FirstOrDefault().SearchTerm;

                                                    if (((searchTermIsAsinFirst2 != null && !string.IsNullOrEmpty(searchTermIsAsinFirst2.SearchTerm)) || (searchTermIsAsinSecond2 != null && !string.IsNullOrEmpty(searchTermIsAsinSecond2.SearchTerm)) || (searchTermIsAsinThird2 != null && !string.IsNullOrEmpty(searchTermIsAsinThird2.SearchTerm))) && regexSearch.Match(thisSearchTerm).Success)
                                                    {
                                                        action2.Product = true;
                                                    }
                                                    else
                                                    {
                                                        action2.Product = false;
                                                    }

                                                    amazonActions.Add(action2);
                                                }
                                            }
                                            else
                                            {
                                                //if ((secondSearchTerm.Pages >= Tier1TresholdPageReads && Tier1TresholdPageReads != 0) || (secondSearchTerm.Orders >= tier1TresholdSales && tier1TresholdSales != 0))
                                                if (secondSearchTerm.Clicks >= 1)
                                                {
                                                    SaveKeywordHistory saveKeywordHistory = new SaveKeywordHistory();
                                                    saveKeywordHistory.CountryId = secondSearchTerm.Country;
                                                    saveKeywordHistory.SearchTerm = secondSearchTerm.SearchTerm;
                                                    saveKeywordHistory.ProductId = secondSearchTerm.ProductId;
                                                    saveKeywordHistory.Action = 1;
                                                    saveKeywordHistory.Reason = "Keyword/ASIN promoted to tier 1 campaign.";
                                                    saveKeywordHistory.ClientId = secondSearchTerm.ClientId;
                                                    saveKeywordHistory.DateProcessed = DateTime.Now.Date;
                                                    saveKeywordHistories.Add(saveKeywordHistory);

                                                    //apply negatives for this promotion
                                                    foreach (var SearchTermToUpdate in allCampaignsButPerformanceOrTier1)
                                                    {
                                                        //if any of these items match the targeting expression or complement, then it is a product
                                                        AllSearchTerms searchTermIsAsinFirst2 = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinSecond2 = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.Keyword.ToLower() == "complements").FirstOrDefault();
                                                        AllSearchTerms searchTermIsAsinThird2 = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.SearchTerm.Length == 10 && !x.SearchTerm.Contains(" ")).FirstOrDefault();
                                                        string thisSearchTerm2 = allSearchTermsToConsiderHere.FirstOrDefault().SearchTerm;

                                                        ////commmented out for negative
                                                        //SaveSummaryReportAction action = new SaveSummaryReportAction();
                                                        //action.keyword = SearchTermToUpdate.Keyword;
                                                        //action.keywordType = SearchTermToUpdate.KeywordType;
                                                        //action.AzCampaignId = SearchTermToUpdate.CampaignId;
                                                        //action.Negative = true;
                                                        //action.CountryId = SearchTermToUpdate.Country;
                                                        //action.SearchTerm = secondSearchTerm.SearchTerm;
                                                        //action.AdGroup = SearchTermToUpdate.AdGroupId;
                                                            
                                                        //if (((searchTermIsAsinFirst2 != null && !string.IsNullOrEmpty(searchTermIsAsinFirst2.SearchTerm)) || (searchTermIsAsinSecond2 != null && !string.IsNullOrEmpty(searchTermIsAsinSecond2.SearchTerm)) || (searchTermIsAsinThird2 != null && !string.IsNullOrEmpty(searchTermIsAsinThird2.SearchTerm))) && regexSearch.Match(thisSearchTerm2).Success)
                                                        //{
                                                        //    action.Product = true;
                                                        //}
                                                        //else
                                                        //{
                                                        //    action.Product = false;
                                                        //}


                                                        //amazonActions.Add(action);

                                                        SaveSummaryReportAction action3 = new SaveSummaryReportAction();
                                                        action3.keyword = SearchTermToUpdate.Keyword;
                                                        action3.keywordType = SearchTermToUpdate.KeywordType;
                                                        action3.AzCampaignId = SearchTermToUpdate.CampaignId;
                                                        action3.Promoted = true;
                                                        action3.CountryId = SearchTermToUpdate.Country;
                                                        action3.SearchTerm = secondSearchTerm.SearchTerm;
                                                        action3.AdGroup = SearchTermToUpdate.AdGroupId;
                                                        action3.KeywordId = SearchTermToUpdate.KeywordId;

                                                        if (((searchTermIsAsinFirst2 != null && !string.IsNullOrEmpty(searchTermIsAsinFirst2.SearchTerm)) || (searchTermIsAsinSecond2 != null && !string.IsNullOrEmpty(searchTermIsAsinSecond2.SearchTerm)) || (searchTermIsAsinThird2 != null && !string.IsNullOrEmpty(searchTermIsAsinThird2.SearchTerm))) && regexSearch.Match(thisSearchTerm2).Success)
                                                        {
                                                            action3.Product = true;
                                                        }
                                                        else
                                                        {
                                                            action3.Product = false;
                                                        }

                                                        saveSummaryReportActions.Add(action3);
                                                    }

                                                    //apply tier 1 promotion
                                                    SaveSummaryReportAction action2 = new SaveSummaryReportAction();
                                                    action2.AzCampaignId = tier1Campaign.azspcampaignid;
                                                    action2.Promoted = true;
                                                    action2.CountryId = secondSearchTerm.Country;
                                                    action2.SearchTerm = secondSearchTerm.SearchTerm;
                                                    action2.DefaultBid = tier1DefaultBid;
                                                        
                                                    //if any of these items match the targeting expression or complement, then it is a product
                                                    AllSearchTerms searchTermIsAsinFirst = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION").FirstOrDefault();
                                                    AllSearchTerms searchTermIsAsinSecond = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.Keyword.ToLower() == "complements").FirstOrDefault();
                                                    AllSearchTerms searchTermIsAsinThird = allSearchTermsToConsiderHere.Where(x => x.KeywordType.ToUpper() == "TARGETING_EXPRESSION_PREDEFINED" && x.SearchTerm.Length == 10 && !x.SearchTerm.Contains(" ")).FirstOrDefault();
                                                    string thisSearchTerm = allSearchTermsToConsiderHere.FirstOrDefault().SearchTerm;

                                                    if (((searchTermIsAsinFirst != null && !string.IsNullOrEmpty(searchTermIsAsinFirst.SearchTerm)) || (searchTermIsAsinSecond != null && !string.IsNullOrEmpty(searchTermIsAsinSecond.SearchTerm)) || (searchTermIsAsinThird != null && !string.IsNullOrEmpty(searchTermIsAsinThird.SearchTerm))) && regexSearch.Match(thisSearchTerm).Success)
                                                    {
                                                        action2.Product = true;
                                                    }
                                                    else
                                                    {
                                                        action2.Product = false;
                                                    }

                                                    amazonActions.Add(action2);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }



                //clean up keywords before proceeding
                foreach (var item in saveKeywordHistories)
                {
                    var cleanSearchTerm = item.SearchTerm.Replace("#", " ").Replace(".", " ").Replace(",", " ").Replace("-", " ").Replace("&", " ").Replace(":", " ").Replace("/", " ").Replace("\"", " ");

                    RegexOptions options = RegexOptions.None;
                    Regex regex = new Regex("[ ]{2,}", options);
                    string cleanKeywordNoExtaSpace = regex.Replace(cleanSearchTerm, " ");

                    item.SearchTerm = cleanKeywordNoExtaSpace;

                }

                foreach (var item in amazonActions)
                {
                    var cleanSearchTerm = item.SearchTerm.Replace("#", " ").Replace(".", " ").Replace(",", " ").Replace("-", " ").Replace("&", " ").Replace(":", " ").Replace("/", " ").Replace("\"", " ");

                    RegexOptions options = RegexOptions.None;
                    Regex regex = new Regex("[ ]{2,}", options);
                    string cleanKeywordNoExtaSpace = regex.Replace(cleanSearchTerm, " ");

                    item.SearchTerm = cleanKeywordNoExtaSpace;

                }






                List<SaveSummaryReportAction> promotionsToApply = amazonActions.Where(x => x.Promoted == true).ToList();

                //assign products and bid to promotion
                foreach (var promotion in promotionsToApply)
                {
                    promotion.QapProductId = campaignProductRelationships.Where(x => x.CountryId == promotion.CountryId && x.ClientId == reportUser.ClientId && x.azspcampaignid == promotion.AzCampaignId).FirstOrDefault().ProductId;
                    
                    BidForSearchTerm bidForSearchTerm = new BidForSearchTerm();
                    decimal? currentBid = await bidForSearchTerm.GetBidForSearchTerm(promotion.SearchTerm, reportUser, promotion.CountryId);

                    if (currentBid != null)
                    {
                        promotion.DefaultBid = (decimal)currentBid;
                    }
                
                }

                if (promotionsToApply != null && promotionsToApply.Count > 0)
                {
                    List<int> countryCodesToApply = promotionsToApply.Select(o => o.CountryId).Distinct().ToList();

                    foreach (var countryCodeToApply in countryCodesToApply)
                    {
                        //seperate asins and products
                        List<SaveSummaryReportAction> asinPromotionsToApplyHere = promotionsToApply.Where(x => x.CountryId == countryCodeToApply && x.Product == false).ToList();
                        List<SaveSummaryReportAction> productPromotionsToApplyHere = promotionsToApply.Where(x => x.CountryId == countryCodeToApply && x.Product == true).ToList();

                        //identify unique campaigns
                        List<string> asinCampaignsToApply = asinPromotionsToApplyHere.Select(o => o.AzCampaignId).Distinct().ToList();
                        List<string> productCampaignsToApply = productPromotionsToApplyHere.Select(o => o.AzCampaignId).Distinct().ToList();


                        //brief pause
                        await System.Threading.Tasks.Task.Delay(2000);

                        //make keyword request and ad group reference
                        KeywordRequestRoot keywordRequest = new KeywordRequestRoot();
                        List<NewAdGroupIds> adGroupReference = new List<NewAdGroupIds>();


                        //do asins for this country
                        foreach (var asinCampaign in asinCampaignsToApply)
                        {
                            List<AdGroupGrouping> adGroups = new List<AdGroupGrouping>();

                            //broad, phrase, and exact ad group ids
                            List<CampaignProductRelationships> broadAdGroupIdsObjects = new List<CampaignProductRelationships>();
                            List<CampaignProductRelationships> phraseAdGroupIdsObjects = new List<CampaignProductRelationships>();
                            List<CampaignProductRelationships> exactAdGroupIdsObjects = new List<CampaignProductRelationships>();

                            broadAdGroupIdsObjects = campaignProductRelationships.Where(x => x.azspcampaignid == asinCampaign && x.CountryId == countryCodeToApply && x.azadgroupusagetype == 1 && x.ClientId == reportUser.ClientId && x.PrimaryAdGroup == true).ToList();
                            phraseAdGroupIdsObjects = campaignProductRelationships.Where(x => x.azspcampaignid == asinCampaign && x.CountryId == countryCodeToApply && x.azadgroupusagetype == 2 && x.ClientId == reportUser.ClientId && x.PrimaryAdGroup == true).ToList();
                            exactAdGroupIdsObjects = campaignProductRelationships.Where(x => x.azspcampaignid == asinCampaign && x.CountryId == countryCodeToApply && x.azadgroupusagetype == 3 && x.ClientId == reportUser.ClientId && x.PrimaryAdGroup == true).ToList();

                            if (broadAdGroupIdsObjects.Count > 0)
                            {
                                AdGroupGrouping adGroup = new AdGroupGrouping();
                                adGroup.AdGroupId = broadAdGroupIdsObjects[0].AzAdGroupId;
                                adGroup.AdGroupType = "Broad";
                                adGroup.AdGroupTypeId = 1;
                                adGroups.Add(adGroup);
                            }

                            if (phraseAdGroupIdsObjects.Count > 0)
                            {
                                AdGroupGrouping adGroup = new AdGroupGrouping();
                                adGroup.AdGroupId = phraseAdGroupIdsObjects[0].AzAdGroupId;
                                adGroup.AdGroupType = "Phrase";
                                adGroup.AdGroupTypeId = 2;
                                adGroups.Add(adGroup);
                            }

                            if (exactAdGroupIdsObjects.Count > 0)
                            {
                                AdGroupGrouping adGroup = new AdGroupGrouping();
                                adGroup.AdGroupId = exactAdGroupIdsObjects[0].AzAdGroupId;
                                adGroup.AdGroupType = "Exact";
                                adGroup.AdGroupTypeId = 3;
                                adGroups.Add(adGroup);
                            }

                            foreach (var adGroupToProcess in adGroups)
                            {


                                //get the list of asins with this campaign id
                                List<SaveSummaryReportAction> asinPromotionsToApplyforThisCampaign = asinPromotionsToApplyHere.Where(x => x.AzCampaignId == asinCampaign).ToList();

                                //build object
                                foreach (var asinHere in asinPromotionsToApplyforThisCampaign)
                                {
                                    RegexOptions options = RegexOptions.None;
                                    Regex regex = new Regex("[ ]{2,}", options);
                                    string rawKeywordNoExtaSpace = regex.Replace(asinHere.SearchTerm, " ");

                                    var cleanKeyword1 = rawKeywordNoExtaSpace.Replace("#", " ").Replace(".", "").Replace(",", "").Replace("-", " ").Replace("&", " ").Replace(":", "").Replace("/", "").Replace("\"", "");
                                    var cleanKeyword = regex.Replace(cleanKeyword1, " ");

                                    APIKeyword apiKeyword = new APIKeyword();
                                    apiKeyword.campaignId = asinCampaign;
                                    apiKeyword.bid = asinHere.DefaultBid;
                                    apiKeyword.keywordText = cleanKeyword;
                                    apiKeyword.matchType = adGroupToProcess.AdGroupType.ToUpper();
                                    apiKeyword.state = "ENABLED";
                                    apiKeyword.adGroupId = adGroupToProcess.AdGroupId;
                                    
                                    keywordRequest.keywords.Add(apiKeyword);

                                    NewAdGroupIds adGroupRef = new NewAdGroupIds();
                                    adGroupRef.CampaignId = asinCampaign;
                                    adGroupRef.OldAdGroupId = adGroupToProcess.AdGroupId;
                                    adGroupRef.ProductId = asinHere.QapProductId;
                                    adGroupReference.Add(adGroupRef);
                                }

                            }
                        }

                        if (keywordRequest.keywords != null && keywordRequest.keywords.Count > 0)
                        {
                            //send and handle keyword request
                            //get token
                            APITokenCreation aPITokenCreation = new APITokenCreation();
                            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(reportUser.aPIAuthorizationRequest);

                            reportUser.aPIAuthorizationRequest.AccessToken = auth.AccessToken;
                            reportUser.aPIAuthorizationRequest.TokenExpirationTime = auth.TokenExpirationTime;

                            CampaignRequest request = new CampaignRequest();
                            request.Authorization = reportUser.aPIAuthorizationRequest;

                            List<NewAdGroupIds> InvlaidKeywords = new List<NewAdGroupIds>();
                            List<string> newAdGroupIds = new List<string>();

                            string keywordsEndpoint = "sp/keywords";
                            string mediaTypeKeywords = "application/vnd.spKeyword.v3+json";


                            SimpleAdKeyword simpleAdKeyword = new SimpleAdKeyword();
                            string keywordsAdded = await simpleAdKeyword.AddThisKeyword(keywordRequest, countryCodeToApply, reportUser.aPIAuthorizationRequest.ClientProfileCodes, keywordsEndpoint, mediaTypeKeywords, auth, InvlaidKeywords, newAdGroupIds, adGroupReference);

                            //see if we had to make a new ad group. If we did, refersh campaignProductRelationships
                            if (newAdGroupIds.Count > 0)
                                campaignProductRelationships = await retrieveKeywordManagementData.GetCampaignProductRelationships(reportUser.ClientId);


                            if (InvlaidKeywords != null && InvlaidKeywords.Count > 0)
                            {
                                foreach (var invalidKeyword in InvlaidKeywords)
                                {
                                    saveSummaryReportActions.RemoveAll(x => x.Promoted == true && x.CountryId == countryCodeToApply && x.SearchTerm == invalidKeyword.KeywordText);
                                    saveKeywordHistories.RemoveAll(x => x.Action == 1 && x.ClientId == auth.ClientId && x.CountryId == countryCodeToApply && x.SearchTerm == invalidKeyword.KeywordText);
                                    amazonActions.RemoveAll(x => x.Negative == true && x.CountryId == countryCodeToApply && x.SearchTerm == invalidKeyword.KeywordText);

                                }
                            }
                        }
                          

                        //products for this country
                        ProductTargetRequestRoot productTargetRequestRoot = new ProductTargetRequestRoot(); //this holds your post parameters
                        List<BusinessObjects.CreateCampaign.Create.TargetingClause> targetingList = new List<BusinessObjects.CreateCampaign.Create.TargetingClause>();

                        List<NewAdGroupIds> adGroupReferenceAsins = new List<NewAdGroupIds>();

                        foreach (var productCampaign in productPromotionsToApplyHere)
                        {
                            List<CampaignProductRelationships> productAdGroupIdsObjects = campaignProductRelationships.Where(x => x.azspcampaignid == productCampaign.AzCampaignId && x.CountryId == countryCodeToApply && x.azadgroupusagetype == 4 && x.ClientId == reportUser.ClientId && x.PrimaryAdGroup == true).ToList();
                           
                            if (productAdGroupIdsObjects.Count > 0)
                            {
                                string asin = productCampaign.SearchTerm;
                                string campaignId = productCampaign.AzCampaignId;
                                string adGroupId = productAdGroupIdsObjects.FirstOrDefault().AzAdGroupId;
                                string bid = productCampaign.DefaultBid.ToString();

                                var target = await MakeAsinObjectToSend(asin, campaignId, adGroupId, bid);
                                targetingList.Add(target);

                                NewAdGroupIds adGroupRef = new NewAdGroupIds();
                                adGroupRef.CampaignId = productCampaign.AzCampaignId;
                                adGroupRef.OldAdGroupId = productAdGroupIdsObjects.FirstOrDefault().AzAdGroupId;
                                adGroupRef.ProductId = productCampaign.QapProductId;
                                adGroupReferenceAsins.Add(adGroupRef);
                            }
                        }

                        productTargetRequestRoot.targetingClauses = targetingList;

                        if (productTargetRequestRoot.targetingClauses != null && productTargetRequestRoot.targetingClauses.Count > 0)
                        {
                            APITokenCreation aPITokenCreation = new APITokenCreation();
                            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(reportUser.aPIAuthorizationRequest);

                            reportUser.aPIAuthorizationRequest.AccessToken = auth.AccessToken;
                            reportUser.aPIAuthorizationRequest.TokenExpirationTime = auth.TokenExpirationTime;

                            string asinsKeywordsEndpoint = "sp/targets";
                            string mediaTypeAsins = "application/vnd.spTargetingClause.v3+json";

                            List<NewAdGroupIds> InvalidAsins = new List<NewAdGroupIds>();
                            List<string> newAdGroupIdsforAsins = new List<string>();

                            SimpleAdAsin simpleAdAsin = new SimpleAdAsin();
                            string asinsAdded = await simpleAdAsin.AddThisAsin(productTargetRequestRoot, countryCodeToApply, reportUser.aPIAuthorizationRequest.ClientProfileCodes, asinsKeywordsEndpoint, mediaTypeAsins, auth, InvalidAsins, newAdGroupIdsforAsins, adGroupReferenceAsins);

                            //see if we had to make a new ad group. If we did, refersh campaignProductRelationships
                            if (newAdGroupIdsforAsins.Count > 0)
                                campaignProductRelationships = await retrieveKeywordManagementData.GetCampaignProductRelationships(reportUser.ClientId);

                            if (InvalidAsins != null && InvalidAsins.Count > 0)
                            {
                                foreach (var invalidAsin in InvalidAsins)
                                {
                                    saveSummaryReportActions.RemoveAll(x => x.Promoted == true && x.CountryId == countryCodeToApply && x.SearchTerm == invalidAsin.KeywordText);
                                    saveKeywordHistories.RemoveAll(x => x.Action == 1 && x.ClientId == auth.ClientId && x.CountryId == countryCodeToApply && x.SearchTerm == invalidAsin.KeywordText);
                                    amazonActions.RemoveAll(x => x.Negative == true && x.CountryId == countryCodeToApply && x.SearchTerm == invalidAsin.KeywordText);
                                }
                            }
                        }
                           
                    }
                }

                //apply negatives via Amazon
                //List<SaveSummaryReportAction> negativesToApply = amazonActions.Where(x => x.Negative == true).ToList();

                //if (negativesToApply != null && negativesToApply.Count > 0)
                //{
                //    List<int> countryCodesToApply = negativesToApply.Select(o => o.CountryId).Distinct().ToList();

                //    foreach (var countryCodeToApply in countryCodesToApply)
                //    {
                //        List<SaveSummaryReportAction> productNegativesToApplyHere = negativesToApply.Where(x => x.CountryId == countryCodeToApply && x.Product == true).ToList();
                //        List<SaveSummaryReportAction> asinNegativesToApplyHere = negativesToApply.Where(x => x.CountryId == countryCodeToApply && x.Product == false).ToList();

                //        //keywords
                //        NegativeQueryRoot negativeQueryRoot = new NegativeQueryRoot();

                //        foreach (var keywordNegative in asinNegativesToApplyHere)
                //        {
                //            NegativeKeywords negativeQueryItem = new NegativeKeywords();
                //            negativeQueryItem.campaignId = keywordNegative.AzCampaignId;
                //            negativeQueryItem.state = "ENABLED";
                //            negativeQueryItem.keywordText = keywordNegative.SearchTerm;
                //            negativeQueryItem.matchType = "NEGATIVE_EXACT";
                //            negativeQueryItem.adGroupId = keywordNegative.AdGroup;
                //            negativeQueryRoot.negativeKeywords.Add(negativeQueryItem);
                //        }

                //        //brief pause
                //        await System.Threading.Tasks.Task.Delay(2000);

                //        if (negativeQueryRoot.negativeKeywords != null && negativeQueryRoot.negativeKeywords.Count > 0)
                //        {
                //            //get token
                //            APITokenCreation aPITokenCreation = new APITokenCreation();
                //            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(reportUser.aPIAuthorizationRequest);

                //            reportUser.aPIAuthorizationRequest.AccessToken = auth.AccessToken;
                //            reportUser.aPIAuthorizationRequest.TokenExpirationTime = auth.TokenExpirationTime;

                //            string negativeKeywordsEndpoint = "sp/negativeKeywords";
                //            string mediaTypeNegativeKeywords = "application/vnd.spNegativeKeyword.v3+json";

                //            List<NewAdGroupIds> InvalidNegativeKeywords = new List<NewAdGroupIds>();

                //            SimpleAdNegativeKeywords addNegativeKeywords = new SimpleAdNegativeKeywords();
                //            var response = await addNegativeKeywords.AddTheseNegativeKeywords(negativeQueryRoot, countryCodeToApply, reportUser.aPIAuthorizationRequest.ClientProfileCodes, negativeKeywordsEndpoint, mediaTypeNegativeKeywords, auth, InvalidNegativeKeywords);

                //            if (InvalidNegativeKeywords != null && InvalidNegativeKeywords.Count > 0)
                //            {
                //                foreach(var invalidnNeg in InvalidNegativeKeywords)
                //                {
                //                    saveSummaryReportActions.RemoveAll(x => x.Negative == true && x.AzCampaignId == invalidnNeg.CampaignId && x.AdGroup == invalidnNeg.OldAdGroupId && x.CountryId == countryCodeToApply && x.SearchTerm == invalidnNeg.KeywordText);
                //                    saveKeywordHistories.RemoveAll(x => x.Action == 2 && x.ClientId == auth.ClientId && x.CountryId == countryCodeToApply && x.SearchTerm == invalidnNeg.KeywordText);
                //                }
                //            }
                //        }

                //        //negative products
                //        NegativeProduct negativeProductQueryRoot = new NegativeProduct();

                //        foreach (var productNegative in productNegativesToApplyHere)
                //        {
                //            NegativeTargetingClause negativeProductQueryItem = new NegativeTargetingClause();
                //            negativeProductQueryItem.campaignId = productNegative.AzCampaignId;
                //            negativeProductQueryItem.state = "ENABLED";
                //            negativeProductQueryItem.adGroupId = productNegative.AdGroup;

                //            BusinessObjects.SearchTermManagement.Expression expression = new BusinessObjects.SearchTermManagement.Expression();
                //            expression.type = "ASIN_SAME_AS";
                //            expression.value = productNegative.SearchTerm;
                //            negativeProductQueryItem.expression.Add(expression);
                //            negativeProductQueryRoot.negativeTargetingClauses.Add(negativeProductQueryItem);
                //        }

                //        if (negativeProductQueryRoot.negativeTargetingClauses != null && negativeProductQueryRoot.negativeTargetingClauses.Count > 0) 
                //        {
                //            //brief pause
                //            await System.Threading.Tasks.Task.Delay(2000);

                //            //get token
                //            APITokenCreation aPITokenCreation = new APITokenCreation();
                //            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(reportUser.aPIAuthorizationRequest);

                //            reportUser.aPIAuthorizationRequest.AccessToken = auth.AccessToken;
                //            reportUser.aPIAuthorizationRequest.TokenExpirationTime = auth.TokenExpirationTime;

                //            string negativeProductEndpoint = "/sp/negativeTargets";
                //            string mediaTypeNegativeProducts = "application/vnd.spNegativeTargetingClause.v3+json";

                //            List<NewAdGroupIds> InvalidNegativeProducts = new List<NewAdGroupIds>();

                //            SimpleAdNegativeProduct addNegativeProd = new SimpleAdNegativeProduct();
                //            var responseNegative = await addNegativeProd.SetNegativeProduct(negativeProductQueryRoot, countryCodeToApply, reportUser.aPIAuthorizationRequest.ClientProfileCodes, negativeProductEndpoint, mediaTypeNegativeProducts, auth, InvalidNegativeProducts);

                //            if (InvalidNegativeProducts != null && InvalidNegativeProducts.Count > 0)
                //            {
                //                foreach(var invalidNeg in InvalidNegativeProducts)
                //                {
                //                    saveSummaryReportActions.RemoveAll(x => x.Negative == true && x.AzCampaignId == invalidNeg.CampaignId && x.CountryId == countryCodeToApply && x.AdGroup == invalidNeg.OldAdGroupId && x.SearchTerm == invalidNeg.KeywordText);
                //                    saveKeywordHistories.RemoveAll(x => x.Action == 2 && x.ClientId == auth.ClientId && x.CountryId == countryCodeToApply && x.SearchTerm == invalidNeg.KeywordText);
                //                }
                //            }
                //        }


                //    }
                //}

 

                SaveKeywordManagementData saveKeywordManagementData = new SaveKeywordManagementData();

                if (saveSummaryReportActions.Count > 0)
                {
                    //save updates to searchtermsummaryreport
                    var actionsSaved = await saveKeywordManagementData.SaveKeywordNegPos(saveSummaryReportActions, reportUser.ClientId);
                }
           
                if (saveKeywordHistories.Count > 0)
                {
                    //save to history table - Date, country, keyword, product, action, reason, ClientId
                    var historySaved = await saveKeywordManagementData.SaveKeywordHistory(saveKeywordHistories, reportUser);
                }

                if (saveActionRequireds.Count > 0)
                {
                    //save to actions needed table - campaignId, actionId - from actionitems table, Description, Resolved
                    var actionsRequiredSaved = await saveKeywordManagementData.SaveActionsRequired(saveActionRequireds, reportUser);
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ProcessReportsLogicSecond";
                logError.ClientId = reportUser.ClientId;
                logError.Parameters = "None. Nightly process for " + DateTime.Now.Date.ToString();
                await logging.WriteToLog(logError);

            }

            return true;

        }

        public async Task<BusinessObjects.CreateCampaign.Create.TargetingClause> MakeAsinObjectToSend(string Asin, string CampaignId, string AdGroupId, string Bid)
        {
            List<BusinessObjects.CreateCampaign.Create.Expression> expressionList = new List<BusinessObjects.CreateCampaign.Create.Expression>();
            BusinessObjects.CreateCampaign.Create.Expression expressionValue = new BusinessObjects.CreateCampaign.Create.Expression();
            expressionValue.type = "ASIN_SAME_AS";
            expressionValue.value = Asin;
            expressionList.Add(expressionValue);


            BusinessObjects.CreateCampaign.Create.TargetingClause targeting = new BusinessObjects.CreateCampaign.Create.TargetingClause();
            targeting.expression = expressionList;
            targeting.campaignId = CampaignId;
            targeting.expressionType = "MANUAL";
            targeting.state = "ENABLED";
            targeting.bid = Convert.ToDecimal(Bid);
            targeting.adGroupId = AdGroupId;

            return targeting;
        }

        public async Task<bool> ProcessOverage(Guid ClientId)
        {
            //bill any overages
            try
            {
                MonthlyBillingOverage monthlyBillingOverage = new MonthlyBillingOverage();
                await monthlyBillingOverage.BillOverage(ClientId);
            }
            catch (Exception ex)
            {
                //nothing to do. keep going.
            }
            return true;
        }
    }
}
