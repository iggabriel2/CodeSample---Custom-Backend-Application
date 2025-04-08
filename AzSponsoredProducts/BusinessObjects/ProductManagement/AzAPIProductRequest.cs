namespace AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement
{
    public class AzAPIProductRequest
    {
        public List<string> asins { get; set; }
        public bool checkItemDetails { get; set; }
        public string cursorToken { get; set; }
        public string adType { get; set; }
        public bool checkEligibility { get; set; }
        public int pageSize { get; set; }

        public AzAPIProductRequest()
        {
            asins = new List<string>();
        }
    }
}
