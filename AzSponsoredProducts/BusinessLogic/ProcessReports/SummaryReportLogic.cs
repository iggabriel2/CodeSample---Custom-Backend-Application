using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.Entities.AzSp.ClientAuthorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class SummaryReportLogic
    {
        private static readonly object ReportLock = new object();

        public async Task<Guid?> ProccessReport(APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, DateTime endDate)
        {
            try
            {
                //this would have to be updated if I were to use it again



















                //get token if I need a new one
                APITokenCreation aPITokenCreation = new APITokenCreation();
                APIAuthorization auth = await aPITokenCreation.ReturnRequestTokens(aPIAuthorizationRequest);

                DateTime startDate = new DateTime();
                startDate = DateTime.Now;


                //make sure enddate is at least equal to startdate
                if (endDate < startDate)
                {
                    return null;
                }

                //create and retrieve first report - make sure start date and end date don't overlap and that end date is always three days in the past
                CreateSummaryReport createsummaryReport = new CreateSummaryReport();
                string reportId = await createsummaryReport.CreateSummaryReportHere(aPIAuthorizationRequest, startDate, endDate, profileCode);

                if (reportId == null)
                {
                    keepProccessing = false;
                }

                string reportUrl = null;
                if (keepProccessing)
                {
                    //will try up to 50 minutes
                    int reportAttempts = 0;

                    Random r = new Random();
                    int rInt = r.Next(240000, 300000);

                    //get report url - check every five minutes
                    CheckReportStatus checkReportStatus = new CheckReportStatus();
                    while (string.IsNullOrEmpty(reportUrl) && reportAttempts < 10)
                    {
                        await System.Threading.Tasks.Task.Delay(rInt);

                        //sleep for five minutes
                        reportUrl = await checkReportStatus.CheckStatus(aPIAuthorizationRequest, profileCode, reportId);

                        if (string.IsNullOrEmpty(reportUrl))
                        {
                            reportAttempts++;
                        }
                    }

                    if (string.IsNullOrEmpty(reportUrl))
                    {
                        keepProccessing = false;
                    }
                }

                string rawReportOutput = "";
                if (keepProccessing)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(reportUrl);

                        if (response.IsSuccessStatusCode)
                        {

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (GZipStream csStream = new GZipStream(stream, CompressionMode.Decompress))
                            {
                                StreamReader reader = new StreamReader(csStream);
                                rawReportOutput = reader.ReadToEnd();
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(rawReportOutput))
                    {
                        keepProccessing = false;
                    }
                }

                if (keepProccessing)
                {
                    List<ReportOutput> reportOutput = JsonConvert.DeserializeObject<List<ReportOutput>>(rawReportOutput);


                    //guid to identify this bulk load so we can delete it
                    Guid reportGuid = Guid.NewGuid();

                    bool saveReportDateSuccess = false;
                    //save first report to db
                    SaveReportData saveReportData = new SaveReportData();
                    saveReportDateSuccess = await saveReportData.SaveBulkSummaryReport(aPIAuthorizationRequest.ClientId, reportOutput, profileCode, reportGuid);

                    if (saveReportDateSuccess)
                    {
                        return reportGuid;
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
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
