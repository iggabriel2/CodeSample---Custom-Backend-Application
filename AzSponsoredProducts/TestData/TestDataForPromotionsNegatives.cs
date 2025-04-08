using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.TestData
{
    public class TestDataForPromotionsNegatives
    {
        public async static Task<List<SaveSummaryReportAction>> GetTestDataForPromotions()
        {
            List<SaveSummaryReportAction> test = new List<SaveSummaryReportAction>();

            SaveSummaryReportAction saveSummaryReportTest = new SaveSummaryReportAction();
            saveSummaryReportTest.AzCampaignId = "185490538406947";
            saveSummaryReportTest.Promoted = true;
            saveSummaryReportTest.CountryId = 1;
            saveSummaryReportTest.SearchTerm = "test 525";
            saveSummaryReportTest.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest.Product = false;
            test.Add(saveSummaryReportTest);


            SaveSummaryReportAction saveSummaryReportTest2 = new SaveSummaryReportAction();
            saveSummaryReportTest2.AzCampaignId = "185490538406947";
            saveSummaryReportTest2.Promoted = true;
            saveSummaryReportTest2.CountryId = 1;
            saveSummaryReportTest2.SearchTerm = "test2 525";
            saveSummaryReportTest2.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest2.Product = false;
            test.Add(saveSummaryReportTest2);


            SaveSummaryReportAction saveSummaryReportTest3 = new SaveSummaryReportAction();
            saveSummaryReportTest3.AzCampaignId = "276830428709839";
            saveSummaryReportTest3.Promoted = true;
            saveSummaryReportTest3.CountryId = 1;
            saveSummaryReportTest3.SearchTerm = "test3 525";
            saveSummaryReportTest3.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest3.Product = false;
            test.Add(saveSummaryReportTest3);

            //products
            //SaveSummaryReportAction saveSummaryReportTest4 = new SaveSummaryReportAction();
            //saveSummaryReportTest4.AzCampaignId = "185490538406947";
            //saveSummaryReportTest4.Promoted = true;
            //saveSummaryReportTest4.CountryId = 1;
            //saveSummaryReportTest4.SearchTerm = "B0BZWB6GPY";
            //saveSummaryReportTest4.DefaultBid = Convert.ToDecimal("0.52");
            //saveSummaryReportTest4.Product = true;
            //test.Add(saveSummaryReportTest4);


            //SaveSummaryReportAction saveSummaryReportTest5 = new SaveSummaryReportAction();
            //saveSummaryReportTest5.AzCampaignId = "185490538406947";
            //saveSummaryReportTest5.Promoted = true;
            //saveSummaryReportTest5.CountryId = 1;
            //saveSummaryReportTest5.SearchTerm = "B0B7L29TB7";
            //saveSummaryReportTest5.DefaultBid = Convert.ToDecimal("0.52");
            //saveSummaryReportTest5.Product = true;
            //test.Add(saveSummaryReportTest5);


            //SaveSummaryReportAction saveSummaryReportTest6 = new SaveSummaryReportAction();
            //saveSummaryReportTest6.AzCampaignId = "276830428709839";
            //saveSummaryReportTest6.Promoted = true;
            //saveSummaryReportTest6.CountryId = 1;
            //saveSummaryReportTest6.SearchTerm = "B08PD33VZ3";
            //saveSummaryReportTest6.DefaultBid = Convert.ToDecimal("0.52");
            //saveSummaryReportTest6.Product = true;
            //test.Add(saveSummaryReportTest6);

            return test;
        }

        public async static Task<List<SaveSummaryReportAction>> GetTestDataForNegatives()
        {
            List<SaveSummaryReportAction> test = new List<SaveSummaryReportAction>();

            SaveSummaryReportAction saveSummaryReportTest = new SaveSummaryReportAction();
            saveSummaryReportTest.AzCampaignId = "185490538406947";
            saveSummaryReportTest.Negative = true;
            saveSummaryReportTest.CountryId = 1;
            saveSummaryReportTest.SearchTerm = "test 525 Neg";
            saveSummaryReportTest.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest.Product = false;
            test.Add(saveSummaryReportTest);


            SaveSummaryReportAction saveSummaryReportTest2 = new SaveSummaryReportAction();
            saveSummaryReportTest2.AzCampaignId = "185490538406947";
            saveSummaryReportTest2.Negative = true;
            saveSummaryReportTest2.CountryId = 1;
            saveSummaryReportTest2.SearchTerm = "test2 525 Neg";
            saveSummaryReportTest2.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest2.Product = false;
            test.Add(saveSummaryReportTest2);


            SaveSummaryReportAction saveSummaryReportTest3 = new SaveSummaryReportAction();
            saveSummaryReportTest3.AzCampaignId = "11372323658190";
            saveSummaryReportTest3.Negative = true;
            saveSummaryReportTest3.CountryId = 1;
            saveSummaryReportTest3.SearchTerm = "test3 525 Neg";
            saveSummaryReportTest3.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest3.Product = false;
            test.Add(saveSummaryReportTest3);

            //products
            SaveSummaryReportAction saveSummaryReportTest4 = new SaveSummaryReportAction();
            saveSummaryReportTest4.AzCampaignId = "185490538406947";
            saveSummaryReportTest4.Negative = true;
            saveSummaryReportTest4.CountryId = 1;
            saveSummaryReportTest4.SearchTerm = "B0BZWB6GPY";
            saveSummaryReportTest4.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest4.Product = true;
            saveSummaryReportTest4.AdGroup = "205150876783870";
            test.Add(saveSummaryReportTest4);


            SaveSummaryReportAction saveSummaryReportTest5 = new SaveSummaryReportAction();
            saveSummaryReportTest5.AzCampaignId = "185490538406947";
            saveSummaryReportTest5.Negative = true;
            saveSummaryReportTest5.CountryId = 1;
            saveSummaryReportTest5.SearchTerm = "B0B7L29TB7";
            saveSummaryReportTest5.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest5.Product = true;
            saveSummaryReportTest5.AdGroup = "205150876783870";
            test.Add(saveSummaryReportTest5);


            SaveSummaryReportAction saveSummaryReportTest6 = new SaveSummaryReportAction();
            saveSummaryReportTest6.AzCampaignId = "276830428709839";
            saveSummaryReportTest6.Negative = true;
            saveSummaryReportTest6.CountryId = 1;
            saveSummaryReportTest6.SearchTerm = "B08PD33VZ3";
            saveSummaryReportTest6.DefaultBid = Convert.ToDecimal("0.52");
            saveSummaryReportTest6.AdGroup = "235511145803513";
            saveSummaryReportTest6.Product = true;
            test.Add(saveSummaryReportTest6);

            return test;
        }
    }
}
