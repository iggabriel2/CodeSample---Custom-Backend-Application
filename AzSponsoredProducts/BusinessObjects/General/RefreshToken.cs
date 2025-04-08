using Configuration;

namespace AdTool.AzSponsoredProducts.BusinessObjects.General
{
    public class RefreshTokenRequest
    {
        private string Grant_type = "refresh_token";

        public string grant_type { get { return this.Grant_type; } set { Grant_type = value; } }
        public string refresh_token {get;set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }

        public RefreshTokenRequest() {
            client_id = AzApiInfo.ClientId;
            client_secret = AzApiInfo.ClientSecret;
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
