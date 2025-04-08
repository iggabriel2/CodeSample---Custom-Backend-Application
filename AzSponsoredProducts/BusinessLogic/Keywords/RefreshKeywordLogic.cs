using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessLogic.Keywords
{
    public class RefreshKeywordLogic
    {
        public async Task<bool> RefreshKeywordsNow()
        {
            //run the keyword refresh behind the scenes
            RefreshKeywords refreshKeywords = new RefreshKeywords();
            refreshKeywords.RefreshExpiredKeywords();
            return true;
        }
    }
}
