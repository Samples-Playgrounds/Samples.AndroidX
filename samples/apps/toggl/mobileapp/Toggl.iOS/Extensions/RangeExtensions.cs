using Foundation;
using System;

namespace Toggl.iOS.Extensions
{
    public static class RangeExtensions
    {
        public static bool ContainsNumber(this NSRange range, nuint number)
        {
            var start = (nuint)range.Location;
            var end = (nuint)(range.Location + range.Length);
            return number >= start && number <= end;
        }
    }
}
