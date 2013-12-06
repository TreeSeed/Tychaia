// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Website
{
    public static class DateTimeHelper
    {
        public static string RenderTimestamp(this long timestamp)
        {
            return ToPreformatted(new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddSeconds(timestamp));
        }

        public static string ToPreformatted(this DateTime date)
        {
            var dateAppend = "th";
            if (date.Day % 10 == 1 && date.Day != 11)
                dateAppend = "st";
            else if (date.Day % 10 == 2 && date.Day != 12)
                dateAppend = "nd";
            else if (date.Day % 10 == 3 && date.Day != 13)
                dateAppend = "rd";
            return date.ToString("ddd d") + dateAppend + date.ToString(" MMM");
        }
    }
}
