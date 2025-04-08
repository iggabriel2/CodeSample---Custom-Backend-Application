using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace AdTool.AzSponsoredProducts.Utils
{
    class TimeZoneUtils
    {
        public async Task<DateTime> GetProfileTimeZoneEndDate(string TimeZone)
        {
            var outputTimeZone = TimeZoneConverter.TZConvert.IanaToWindows(TimeZone);
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(outputTimeZone);
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            return currentDateTime.Date;

        }
    }
}
