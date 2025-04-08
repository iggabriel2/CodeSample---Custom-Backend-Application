using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.General
{
  
    public class AccountInfo
    {
        public string marketplaceStringId { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string subType { get; set; }
        public bool validPaymentMethod { get; set; }
    }

    public class ProfileCodes
    {
        public long profileId { get; set; }
        public string countryCode { get; set; }
        public string currencyCode { get; set; }
        public string timezone { get; set; }
        public AccountInfo accountInfo { get; set; }
        public ProfileCodes() { 
            accountInfo = new AccountInfo();
        }
    }


}
