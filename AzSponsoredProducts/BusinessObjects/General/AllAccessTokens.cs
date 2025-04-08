using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace AdTool.AzSponsoredProducts.BusinessObjects.General
{

    public class AllAccessTokens
    {
        public string AccessToken { get; set;}
        public string RefreshToken { get; set;}
        public DateTime? TokenExpirationTime { get; set; }
        public Guid ClientId { get; set; }
        public string ErrorMessage { get; set; }
    }

}
