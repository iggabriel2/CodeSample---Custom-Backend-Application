using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.Entities.AzSp.ClientAuthorization;

namespace AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement
{

    public class AzProductResponse
    {
        public List<ProductMetadataList> ProductMetadataList { get; set; }
        public APIAuthorization Authorization { get; set; }
        public AzProductResponse() { 
            ProductMetadataList = new List<ProductMetadataList>();
            Authorization = new APIAuthorization();
        } 
    }

    public class PriceToPay
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }

    public class ProductMetadataList
    {
        public string asin { get; set; }
        public string availability { get; set; }
        public string bestSellerRank { get; set; }
        public string brand { get; set; }
        public string category { get; set; }
        public string createdDate { get; set; }
        public string eligibilityStatus { get; set; }
        public string imageUrl { get; set; }
        public PriceToPay priceToPay { get; set; }
        public string title { get; set; }
        public ProductMetadataList()
        {
            priceToPay = new PriceToPay();
        }
    }




}
