using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Utils;
using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class CreateReportIds
    {
        public async Task<string> CreateDailyReportId(ReportLoggingByClient reportLogging, APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing)
        {
            try
            {
                //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                await System.Threading.Tasks.Task.Delay(1000);

                DateTime endDate = reportLogging.Today;

                //get token if I need a new one
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(aPIAuthorizationRequest);

                DateTime startDate = new DateTime();

                //make sure startdate is not before client join date
                if (reportLogging.LastRunDate.AddDays(-5) < reportLogging.StartDate)
                {
                    startDate = reportLogging.StartDate.Date;
                }
                else
                {
                    startDate = reportLogging.LastRunDate.AddDays(-3).Date;
                }


                //make sure enddate is at least equal to startdate
                if (endDate < startDate)
                {
                    return null;
                }

                //create and retrieve first report - make sure start date and end date don't overlap and that end date is always three days in the past
                CreateDailyReports createDailyReport = new CreateDailyReports();
                string reportId = await createDailyReport.CreateDailyReportHere(aPIAuthorizationRequest, startDate, endDate, profileCode);

                return reportId;
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        public async Task<string> CreateMonthlyReportId(ReportLoggingByClient reportLogging, APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, bool ProcessLastMonth = false)
        {
            try
            {
                //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                await System.Threading.Tasks.Task.Delay(1000);

                DateTime firstOfthisMonth = new DateTime(reportLogging.Today.Year, reportLogging.Today.Month, 1).Date;


                //get token if I need a new one
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(aPIAuthorizationRequest);

                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();

                if (reportLogging.Today.Month == reportLogging.StartDate.Month && reportLogging.Today.Year == reportLogging.StartDate.Year)
                {
                    startDate = reportLogging.StartDate.Date;
                    endDate = reportLogging.Today.Date;
                }
                else
                {
                    if (ProcessLastMonth && reportLogging.LastRunDate.AddDays(1).Month == reportLogging.Today.Month)
                    {
                        startDate = new DateTime(reportLogging.Today.AddMonths(-1).Date.Year, reportLogging.Today.AddMonths(-1).Date.Month, 1).Date;
                        endDate = new DateTime(reportLogging.Today.AddMonths(-1).Date.Year, reportLogging.Today.AddMonths(-1).Date.Month, DateTime.DaysInMonth(year: reportLogging.Today.AddMonths(-1).Date.Year, month: reportLogging.Today.AddMonths(-1).Date.Month));
                    }
                    else if (ProcessLastMonth && reportLogging.LastRunDate.AddDays(1) < firstOfthisMonth)
                    {
                        startDate = new DateTime(reportLogging.LastRunDate.Date.Year, reportLogging.LastRunDate.Date.Month, 1).Date;
                        endDate = new DateTime(reportLogging.LastRunDate.Date.Year, reportLogging.LastRunDate.Date.Month, DateTime.DaysInMonth(year: reportLogging.LastRunDate.Date.Year, month: reportLogging.LastRunDate.Date.Month));
                    }
                    else if (ProcessLastMonth)
                    {
                        //this is just a precaution. In case anything goes wrong getting the dates for last month, we don't want to run this month twice, so go ahead and quit.
                        return null;
                    }
                    else
                    {
                        startDate = new DateTime(reportLogging.Today.Date.Year, reportLogging.Today.Date.Month, 1).Date;
                        endDate = reportLogging.Today;
                    }

                }

                //make sure enddate is at least equal to startdate
                if (endDate < startDate)
                {
                    return null;
                }

                //create and retrieve first report - make sure start date and end date don't overlap and that end date is always three days in the past
                CreateMonthlySummaryReport createsummaryReport = new CreateMonthlySummaryReport();
                string reportId = await createsummaryReport.CreateSummaryReportHere(aPIAuthorizationRequest, startDate, endDate, profileCode);

                return reportId;

            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<string> CreateMonthlyReportIdForKeywords(ReportLoggingByClient reportLogging, APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, bool ProcessLastMonth = false)
        {
            try
            {
                //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                await System.Threading.Tasks.Task.Delay(1000);

                DateTime firstOfthisMonth = new DateTime(reportLogging.Today.Year, reportLogging.Today.Month, 1).Date;


                //get token if I need a new one
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(aPIAuthorizationRequest);

                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();

                if (reportLogging.Today.Month == reportLogging.StartDate.Month && reportLogging.Today.Year == reportLogging.StartDate.Year)
                {
                    startDate = reportLogging.StartDate.Date;
                    endDate = reportLogging.Today.Date;
                }
                else
                {
                    if (ProcessLastMonth && reportLogging.LastRunDate.AddDays(1).Month == reportLogging.Today.Month)
                    {
                        startDate = new DateTime(reportLogging.Today.AddMonths(-1).Date.Year, reportLogging.Today.AddMonths(-1).Date.Month, 1).Date;
                        endDate = new DateTime(reportLogging.Today.AddMonths(-1).Date.Year, reportLogging.Today.AddMonths(-1).Date.Month, DateTime.DaysInMonth(year: reportLogging.Today.AddMonths(-1).Date.Year, month: reportLogging.Today.AddMonths(-1).Date.Month));
                    }
                    else if (ProcessLastMonth && reportLogging.LastRunDate.AddDays(1) < firstOfthisMonth)
                    {
                        startDate = new DateTime(reportLogging.LastRunDate.Date.Year, reportLogging.LastRunDate.Date.Month, 1).Date;
                        endDate = new DateTime(reportLogging.LastRunDate.Date.Year, reportLogging.LastRunDate.Date.Month, DateTime.DaysInMonth(year: reportLogging.LastRunDate.Date.Year, month: reportLogging.LastRunDate.Date.Month));
                    }
                    else if (ProcessLastMonth)
                    {
                        //this is just a precaution. In case anything goes wrong getting the dates for last month, we don't want to run this month twice, so go ahead and quit.
                        return null;
                    }
                    else
                    {
                        startDate = new DateTime(reportLogging.Today.Date.Year, reportLogging.Today.Date.Month, 1).Date;
                        endDate = reportLogging.Today;
                    }

                }

                //make sure enddate is at least equal to startdate
                if (endDate < startDate)
                {
                    return null;
                }

                //create and retrieve first report - make sure start date and end date don't overlap and that end date is always three days in the past
                CreateMonthlySummaryKeywordReport createsummaryReport = new CreateMonthlySummaryKeywordReport();
                string reportId = await createsummaryReport.CreateSummaryReportHere(aPIAuthorizationRequest, startDate, endDate, profileCode);

                return reportId;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> CreateSnapshot(ReportLoggingByClient reportLogging, APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, string snapshotType)
        {
            try
            {
                //one second sleep between calls - precaution to avoid sending too many to Amazon at once
                await System.Threading.Tasks.Task.Delay(1000);

                //get token if I need a new one
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(aPIAuthorizationRequest);

                //this is populated by default with all keyword types, so nothing to do
                SnapshotReqestKeywords snapshotReqestKeywords = new SnapshotReqestKeywords();


                SnapshotCreation createASnapshot = new SnapshotCreation();
                string reportId = await createASnapshot.CreateSnapshotHere(aPIAuthorizationRequest, profileCode, snapshotReqestKeywords, snapshotType);

                return reportId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
