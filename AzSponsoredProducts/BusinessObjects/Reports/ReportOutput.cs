using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Reports
{
    public class ReportOutput
    {
        public Guid ClientId { get; set; }
        public Guid BulkId { get; set; }
        public int Country { get; set; }
        public string keywordId { get; set; }
        //should be the same as keyword
        public string keyword { get; set; }
        //same as spend
        public decimal? cost { get; set; }
        public string searchTerm { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public int? clicks { get; set; }
        public string keywordType { get; set; }
        public int? impressions { get; set; }
        public string adGroupId { get; set; }
        public string adGroupName { get; set; }
        public decimal? costPerClick { get; set; }
        public string portfolioId { get; set; }
        public int? purchases14d { get; set; }
        public int? kindleEditionNormalizedPagesRead14d { get; set; }
        public double? attributedSalesSameSku14d { get; set; }
        public decimal? clickThroughRate { get; set; }
        public decimal? roasClicks14d { get; set; }
        public int? unitsSoldClicks14d { get; set; }
        public string campaignStatus { get; set; }
        public string savingDate { get; set; }
    }

    public class DailyReportOutput
    {
       
        public decimal? cost { get; set; }
     
        public int? clicks { get; set; }
     
        public int? impressions { get; set; }
        
        public int? kindleEditionNormalizedPagesRead14d { get; set; }

        public int? unitsSoldClicks14d { get; set; }
        public string? date { get; set; }
    }

    public class MonthlyReportOutput
    {
        public Guid ClientId { get; set; }
        public Guid BulkId { get; set; }
        public int Country { get; set; }
        public string keywordId { get; set; }
        //should be the same as keyword
        public string keyword { get; set; }
        //same as spend
        public decimal? cost { get; set; }
        public string searchTerm { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public int? clicks { get; set; }
        public string keywordType { get; set; }
        public int? impressions { get; set; }
        public string adGroupId { get; set; }
        public string adGroupName { get; set; }
        public decimal? costPerClick { get; set; }
        public string portfolioId { get; set; }
        public int? purchases14d { get; set; }
        public int? kindleEditionNormalizedPagesRead14d { get; set; }
        public double? attributedSalesSameSku14d { get; set; }
        public decimal? clickThroughRate { get; set; }
        public decimal? roasClicks14d { get; set; }
        public int? unitsSoldClicks14d { get; set; }
        public string campaignStatus { get; set; }
        public string savingDate { get; set; }
        public string date { get; set; }
        public DateTime dateRecord { get; set; }
    }

    public class MonthlyReportOutputForKeywords
    {
        public Guid ClientId { get; set; }
        public Guid BulkId { get; set; }
        public int Country { get; set; }
        public string keywordId { get; set; }
        //should be the same as keyword
        public string keyword { get; set; }
        //same as spend
        public decimal? cost { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public int? clicks { get; set; }
        public string keywordType { get; set; }
        public int? impressions { get; set; }
        public string adGroupId { get; set; }
        public string adGroupName { get; set; }
        public decimal? costPerClick { get; set; }
        public string portfolioId { get; set; }
        public int? purchases14d { get; set; }
        public int? kindleEditionNormalizedPagesRead14d { get; set; }
        public double? attributedSalesSameSku14d { get; set; }
        public decimal? clickThroughRate { get; set; }
        public decimal? roasClicks14d { get; set; }
        public int? unitsSoldClicks14d { get; set; }
        public string campaignStatus { get; set; }
        public string savingDate { get; set; }
        public string date { get; set; }
        public DateTime dateRecord { get; set; }
    }

    public class DailyKeywordDataOutput
    {
        public string id { get; set; }
        public string partitionKey { get; set; }
        public string ClientId { get; set; }
        public int Country { get; set; }
        public string keywordId { get; set; }
        //should be the same as keyword
        public string keyword { get; set; }
        //same as spend
        public decimal? cost { get; set; }
        public string searchTerm { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public int? clicks { get; set; }
        public string keywordType { get; set; }
        public int? impressions { get; set; }
        public string adGroupId { get; set; }
        public string adGroupName { get; set; }
        public decimal? costPerClick { get; set; }
        public string portfolioId { get; set; }
        public int? purchases14d { get; set; }
        public int? kindleEditionNormalizedPagesRead14d { get; set; }
        public double? attributedSalesSameSku14d { get; set; }
        public decimal? clickThroughRate { get; set; }
        public decimal? roasClicks14d { get; set; }
        public int? unitsSoldClicks14d { get; set; }
        public string campaignStatus { get; set; }
        public string savingDate { get; set; }
        public string date { get; set; }
        public DateTime dateRecord { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CountryName { get; set; }
        public bool Negative { get; set; }
        public int? QAPCampaignId { get; set; }
        public string UsageType { get; set; }
    }

    public class DailyKeywordDataOutputForKeywords
    {
        public string id { get; set; }
        public string partitionKey { get; set; }
        public string ClientId { get; set; }
        public int Country { get; set; }
        public string keywordId { get; set; }
        //should be the same as keyword
        public string keyword { get; set; }
        //same as spend
        public decimal? cost { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public int? clicks { get; set; }
        public string keywordType { get; set; }
        public int? impressions { get; set; }
        public string adGroupId { get; set; }
        public string adGroupName { get; set; }
        public decimal? costPerClick { get; set; }
        public string portfolioId { get; set; }
        public int? purchases14d { get; set; }
        public int? kindleEditionNormalizedPagesRead14d { get; set; }
        public double? attributedSalesSameSku14d { get; set; }
        public decimal? clickThroughRate { get; set; }
        public decimal? roasClicks14d { get; set; }
        public int? unitsSoldClicks14d { get; set; }
        public string campaignStatus { get; set; }
        public string savingDate { get; set; }
        public string date { get; set; }
        public DateTime dateRecord { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CountryName { get; set; }
        public bool Negative { get; set; }
        public int? QAPCampaignId { get; set; }
        public string UsageType { get; set; }
    }


    public class DailyCampaignData
    {
        public string id { get; set; }
        public string partitionKey { get; set; }
        public string ClientId { get; set; }
        public int Country { get; set; }
        //same as spend
        public decimal? cost { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public int? clicks { get; set; }
        public int? impressions { get; set; }
        public string portfolioId { get; set; }
        public int? purchases14d { get; set; }
        public int? kindleEditionNormalizedPagesRead14d { get; set; }
        public decimal? attributedSalesSameSku14d { get; set; }
        public int? unitsSoldClicks14d { get; set; }
        public string campaignStatus { get; set; }
        public string savingDate { get; set; }
        public DateTime dateRecord { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CountryName { get; set; }
        public int? QAPCampaignId { get; set; }
        public string UsageType { get; set; }
    }
}
