using AdTool.AzSponsoredProducts.AmazonAPI.ProcessReports;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.Data.ReportData;
using AdTool.Entities.AzSp.ClientAuthorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.ProcessReports
{
    public class DailyReportLogic
    {
        private static readonly object ReportLock = new object();

        public async Task<bool> ProccessReport(APIAuthorizationRequest aPIAuthorizationRequest, ReportUser reportUser, ClientProfileCodes profileCode, bool keepProccessing, string reportUrl)
        {
            try
            {
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
                    List<DailyReportOutput> reportOutput = JsonConvert.DeserializeObject<List<DailyReportOutput>>(rawReportOutput);


                    //process daily report into item to save
                    List<DailyReportSave> dailyReports = new List<DailyReportSave>();

                    foreach (var reportRecord in reportOutput)
                    {
                        DailyReportSave dailyReport = dailyReports.Where(x => x.ReportDate == Convert.ToDateTime(reportRecord.date)).FirstOrDefault();

                        if (dailyReport != null)
                        {
                            dailyReport.Impressions = dailyReport.Impressions + Convert.ToInt32(reportRecord.impressions);
                            dailyReport.Clicks = dailyReport.Clicks + Convert.ToInt32(reportRecord.clicks);
                            dailyReport.Orders = dailyReport.Orders + Convert.ToInt32(reportRecord.unitsSoldClicks14d);
                            dailyReport.Cost = dailyReport.Cost + Convert.ToDecimal(reportRecord.cost);
                        }
                        else
                        {
                            DailyReportSave dailyReporttoAdd = new DailyReportSave();
                            dailyReporttoAdd.Impressions = Convert.ToInt32(reportRecord.impressions);
                            dailyReporttoAdd.Clicks = Convert.ToInt32(reportRecord.clicks);
                            dailyReporttoAdd.Orders = Convert.ToInt32(reportRecord.unitsSoldClicks14d);
                            dailyReporttoAdd.ReportDate = Convert.ToDateTime(reportRecord.date);
                            dailyReporttoAdd.ClientId = reportUser.ClientId;
                            dailyReporttoAdd.CountryId = profileCode.CountryId;
                            dailyReporttoAdd.Cost = Convert.ToDecimal(reportRecord.cost);

                            dailyReports.Add(dailyReporttoAdd);
                        }

                    }

                    //calculate cpc
                    foreach (var dailyreport in dailyReports)
                    {
                        try
                        {
                            var multiplier = Math.Pow(10, 2);
                            var roundedUpNumber = Math.Ceiling((Convert.ToDouble(dailyreport.Cost) / dailyreport.Clicks) * multiplier) / multiplier;
                            dailyreport.CPC = Convert.ToDecimal(roundedUpNumber);
                        }
                        catch (Exception ex)
                        {
                            dailyreport.CPC = 0;
                        }

                    }


                    //save first report to db
                    SaveReportData saveReportData = new SaveReportData();
                    var saveResponse = await saveReportData.SaveDailyReport(dailyReports, aPIAuthorizationRequest.ClientId);

                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
