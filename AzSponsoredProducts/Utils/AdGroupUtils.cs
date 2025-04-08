using AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords;
using AdTool.AzSponsoredProducts.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class AdGroupUtils
    {
        public async Task<string> GetNewAdGroupName(string campaignId, int CountryId, Guid ClientId, string AdGroupId)
        {
            //get the existing ad group name, see if the last string value after the last space is a number and increase by 1 or add " 2"
            RetrieveData rd = new RetrieveData();
            string adGroupName = await rd.GetAdGroupName(campaignId, CountryId, ClientId, AdGroupId);
            string newAdGroupName = "";

            string baseAdGroupEnd = adGroupName.Substring(adGroupName.LastIndexOf(" "), adGroupName.Length - adGroupName.LastIndexOf(" ")).Trim();
            string baseAdGroupEndWithoutParen = baseAdGroupEnd.Replace(")", "").Trim();
            long adGroupNumber = 0;

            bool canConvert = long.TryParse(baseAdGroupEndWithoutParen, out adGroupNumber);
            if (canConvert == true)
            {
                long newNumber = adGroupNumber + 1;
                newAdGroupName = adGroupName.Substring(0, adGroupName.IndexOf(baseAdGroupEnd)).Trim() + " " + newNumber.ToString() + ")";
            }
            else
            {
                long newNumber = 2;
                newAdGroupName = adGroupName.Substring(0, adGroupName.LastIndexOf(")")).Trim() + " " + newNumber.ToString() + ")";
            }

            return newAdGroupName;
        }
    }
}
