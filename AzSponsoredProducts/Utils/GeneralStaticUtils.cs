using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Utils
{
    public static class GeneralStaticUtils
    {
        public static async Task<decimal> SafeDivision(decimal Numerator, decimal Denominator)
        {
            decimal dividedValue = (Denominator == 0) ? 0 : Numerator / Denominator;
            return dividedValue;
        }

        public static async Task<decimal> RoundUp(decimal input, int places = 2)
        {
            double inputToUse = Convert.ToDouble(input);
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Convert.ToDecimal(Math.Ceiling(inputToUse * multiplier) / multiplier);
        }

        public static async Task<decimal> Round(decimal input, int places = 2)
        {
            double inputToUse = Convert.ToDouble(input);
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Convert.ToDecimal(Math.Round(inputToUse * multiplier) / multiplier);
        }
    }
}
