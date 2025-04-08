using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Get;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.ProductAdListResponse;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create
{
    public class ProductAdListResponse
    {
        //product request
        public class CampaignIdFilter
        {
            public List<string> include { get; set; }
            public CampaignIdFilter() 
            { 
                include = new List<string>();
            }
        }

        public class AdGroupRequestObject
        {
            public CampaignIdFilter campaignIdFilter { get; set; }
            public AdGroupRequestObject() 
            {
                campaignIdFilter = new CampaignIdFilter();
            }
        }



        //product response
        public class ExtendedData
        {
            public DateTime lastUpdateDateTime { get; set; }
            public string servingStatus { get; set; }
            public List<ServingStatusDetail> servingStatusDetails { get; set; }
            public DateTime creationDateTime { get; set; }
            public ExtendedData()
            {
                servingStatusDetails = new List<ServingStatusDetail>();
            }
        }

        public class ProductAd
        {
            public string adId { get; set; }
            public string campaignId { get; set; }
            public string customText { get; set; }
            public string asin { get; set; }
            public string state { get; set; }
            public string sku { get; set; }
            public string adGroupId { get; set; }
            public ExtendedData extendedData { get; set; }
            public ProductAd()
            {
                extendedData = new ExtendedData();
            }
        }

        public class ProductResponseRoot
        {
            public int totalResults { get; set; }
            public string nextToken { get; set; }
            public List<ProductAd> productAds { get; set; }
            public ProductResponseRoot()
            {
                productAds = new List<ProductAd>();
            }
        }

        public class ServingStatusDetail
        {
            public string name { get; set; }
            public string helpUrl { get; set; }
            public string message { get; set; }
        }
    }
}
