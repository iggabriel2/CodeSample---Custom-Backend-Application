using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class GetReports
    {
        private static SemaphoreSlim _semaphoreSlim2 = new SemaphoreSlim(1);

        public async Task<bool> GetReportUrls(APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, List<ReportIds> allReportIds)
        {
            //get token if I need a new one
            APITokenCreation aPITokenCreation = new APITokenCreation();
            APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(aPIAuthorizationRequest);

            //will try up to 50 minutes
            int reportAttempts = 0;

            Random r = new Random();
            int rInt = r.Next(240000, 360000);

            bool finished = false;
            int attempt = 0;

            while (!finished && attempt < 20) {
                await System.Threading.Tasks.Task.Delay(rInt);

                await _semaphoreSlim2.WaitAsync();

                foreach (ReportIds reportIds in allReportIds)
                {
                    CheckReportStatus checkReportStatus = new CheckReportStatus();
                    CheckSnapshotStatus checkSnapshotStatus = new CheckSnapshotStatus();

                    //no report ids, mark as done
                    if (string.IsNullOrEmpty(reportIds.DailyReportId))
                    {
                        reportIds.DailyUrlAcquired = true;
                    }

                    if (string.IsNullOrEmpty(reportIds.MonthlyReportId))
                    {
                        reportIds.MonthlyUrlAcquired = true;
                    }

                    if (string.IsNullOrEmpty(reportIds.LastMonthlyReportId))
                    {
                        reportIds.LastMonthUrlAcquired = true;
                    }

                    //if (string.IsNullOrEmpty(reportIds.KeywordSnapshotId))
                    //{
                    //    reportIds.KeywordSnapshotUrlAcquired = true;
                    //}

                    if (string.IsNullOrEmpty(reportIds.AdGroupSnapshotId))
                    {
                        reportIds.AdGroupSnapshotUrlAcquired = true;
                    }

                    if (string.IsNullOrEmpty(reportIds.ProductTargetSnapshotId))
                    {
                        reportIds.ProductTargetSnapshotUrlAcquired = true;
                    }

                    if (string.IsNullOrEmpty(reportIds.KeywordMonthlyReportId))
                    {
                        reportIds.KeywordMonthlyUrlAcquired = true;
                    }

                    if (string.IsNullOrEmpty(reportIds.KeywordLastMonthlyReportId))
                    {
                        reportIds.KeywordLastMonthUrlAcquired = true;
                    }


                    if (string.IsNullOrEmpty(reportIds.DailyReportUrl) && !string.IsNullOrEmpty(reportIds.DailyReportId))
                    {
                        try
                        {
                            reportIds.DailyReportUrl = await checkReportStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.DailyReportId);

                            if (!string.IsNullOrEmpty(reportIds.DailyReportUrl))
                            {
                                reportIds.DailyUrlAcquired = true;
                            }
                        }
                        catch(Exception ex)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = ex.ToString();
                            logError.FailureMethod = "GetReportUrls - Daily";
                            logError.ClientId = reportUser.ClientId;
                            logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                            await logging.WriteToLog(logError);
                        }
                        
                    }

                    //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                    await System.Threading.Tasks.Task.Delay(1000);

                    if (string.IsNullOrEmpty(reportIds.MonthlyReportUrl) && !string.IsNullOrEmpty(reportIds.MonthlyReportId))
                    {
                        try
                        {
                            reportIds.MonthlyReportUrl = await checkReportStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.MonthlyReportId);

                            if (!string.IsNullOrEmpty(reportIds.MonthlyReportUrl))
                            {
                                reportIds.MonthlyUrlAcquired = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = ex.ToString();
                            logError.FailureMethod = "GetReportUrls - Monthly";
                            logError.ClientId = reportUser.ClientId;
                            logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                            await logging.WriteToLog(logError);
                        }
                       
                    }

                    //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                    await System.Threading.Tasks.Task.Delay(1000);

                    if (string.IsNullOrEmpty(reportIds.LastMonthlyReportUrl) && reportIds.ProcessLastMonth == true && !string.IsNullOrEmpty(reportIds.LastMonthlyReportId))
                    {
                        try
                        {
                            reportIds.LastMonthlyReportUrl = await checkReportStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.LastMonthlyReportId);

                            if (!string.IsNullOrEmpty(reportIds.LastMonthlyReportUrl))
                            {
                                reportIds.LastMonthUrlAcquired = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = ex.ToString();
                            logError.FailureMethod = "GetReportUrls - Last Month";
                            logError.ClientId = reportUser.ClientId;
                            logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                            await logging.WriteToLog(logError);
                        }
                       
                    }

                    //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                    await System.Threading.Tasks.Task.Delay(1000);

                    if (string.IsNullOrEmpty(reportIds.KeywordMonthlyReportUrl) && !string.IsNullOrEmpty(reportIds.KeywordMonthlyReportId))
                    {
                        try
                        {
                            reportIds.KeywordMonthlyReportUrl = await checkReportStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.KeywordMonthlyReportId);

                            if (!string.IsNullOrEmpty(reportIds.KeywordMonthlyReportUrl))
                            {
                                reportIds.KeywordMonthlyUrlAcquired = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = ex.ToString();
                            logError.FailureMethod = "GetReportUrls - Keyword Monthly";
                            logError.ClientId = reportUser.ClientId;
                            logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                            await logging.WriteToLog(logError);
                        }

                    }

                    //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                    await System.Threading.Tasks.Task.Delay(1000);

                    if (string.IsNullOrEmpty(reportIds.KeywordLastMonthlyReportUrl) && reportIds.ProcessLastMonth == true && !string.IsNullOrEmpty(reportIds.KeywordLastMonthlyReportId))
                    {
                        try
                        {
                            reportIds.KeywordLastMonthlyReportUrl = await checkReportStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.KeywordLastMonthlyReportId);

                            if (!string.IsNullOrEmpty(reportIds.KeywordLastMonthlyReportUrl))
                            {
                                reportIds.KeywordLastMonthUrlAcquired = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = ex.ToString();
                            logError.FailureMethod = "GetReportUrls - Keyword Last Month";
                            logError.ClientId = reportUser.ClientId;
                            logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                            await logging.WriteToLog(logError);
                        }

                    }

                    //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                    await System.Threading.Tasks.Task.Delay(1000);

                    ////get keyword snapshot url
                    //if (string.IsNullOrEmpty(reportIds.KeywordSnapshotUrl) && !string.IsNullOrEmpty(reportIds.KeywordSnapshotId))
                    //{
                    //    try
                    //    {
                    //        reportIds.KeywordSnapshotUrl = await checkSnapshotStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.KeywordSnapshotId);

                    //        if (!string.IsNullOrEmpty(reportIds.KeywordSnapshotUrl))
                    //        {
                    //            reportIds.KeywordSnapshotUrlAcquired = true;
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Logging logging = new Logging();
                    //        LogError logError = new LogError();
                    //        logError.ErrorMessage = ex.ToString();
                    //        logError.FailureMethod = "GetReportUrls - Keyword Snapshot";
                    //        logError.ClientId = reportUser.ClientId;
                    //        logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                    //        await logging.WriteToLog(logError);
                    //    }

                    //}

                    ////one second sleep between calls - precaution to avoid sending too many to Amazon at once
                    //await System.Threading.Tasks.Task.Delay(1000);

                    //get ad group snapshot url
                    if (string.IsNullOrEmpty(reportIds.AdGroupSnapshotUrl) && !string.IsNullOrEmpty(reportIds.AdGroupSnapshotId))
                    {
                        try
                        {
                            reportIds.AdGroupSnapshotUrl = await checkSnapshotStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.AdGroupSnapshotId);

                            if (!string.IsNullOrEmpty(reportIds.AdGroupSnapshotUrl))
                            {
                                reportIds.AdGroupSnapshotUrlAcquired = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = ex.ToString();
                            logError.FailureMethod = "GetReportUrls - Ad Group Snapshot";
                            logError.ClientId = reportUser.ClientId;
                            logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                            await logging.WriteToLog(logError);
                        }

                    }

                    //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                    await System.Threading.Tasks.Task.Delay(1000);

                    //get product target snapshot url
                    if (string.IsNullOrEmpty(reportIds.ProductTargetSnapshotUrl) && !string.IsNullOrEmpty(reportIds.ProductTargetSnapshotId))
                    {
                        try
                        {
                            reportIds.ProductTargetSnapshotUrl = await checkSnapshotStatus.CheckStatus(aPIAuthorizationRequest, reportIds.ClientProfileCode, reportIds.ProductTargetSnapshotId);

                            if (!string.IsNullOrEmpty(reportIds.ProductTargetSnapshotUrl))
                            {
                                reportIds.ProductTargetSnapshotUrlAcquired = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging logging = new Logging();
                            LogError logError = new LogError();
                            logError.ErrorMessage = ex.ToString();
                            logError.FailureMethod = "GetReportUrls - Product Target Snapshot";
                            logError.ClientId = reportUser.ClientId;
                            logError.Parameters = JsonConvert.SerializeObject(allReportIds);
                            await logging.WriteToLog(logError);
                        }

                    }

                }

                _semaphoreSlim2.Release();

                //if all are finished, stop
                ReportIds anyThatArentFinished = new ReportIds();
                anyThatArentFinished = allReportIds.Where(x => x.LastMonthUrlAcquired == false || x.KeywordLastMonthUrlAcquired == false || x.KeywordMonthlyUrlAcquired == false || x.DailyUrlAcquired == false || x.MonthlyUrlAcquired == false || x.AdGroupSnapshotUrlAcquired == false || x.ProductTargetSnapshotUrlAcquired == false).FirstOrDefault();

                if (anyThatArentFinished == null)
                {
                    finished = true;
                }

                attempt++;
            }
            return true;
        }
    }
}
