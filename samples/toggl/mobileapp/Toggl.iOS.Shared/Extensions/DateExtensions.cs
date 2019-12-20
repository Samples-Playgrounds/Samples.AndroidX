using System;
using Foundation;

namespace Toggl.iOS.Shared.Extensions
{
    public static class DateExtensions
    {
        private static readonly DateTimeOffset referenceDate = new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static NSDate ToNSDate(this DateTimeOffset self)
            => NSDate.FromTimeIntervalSinceReferenceDate((self - referenceDate).TotalSeconds);

        public static DateTimeOffset ToDateTimeOffset(this NSDate self)
            => referenceDate.AddSeconds(self.SecondsSinceReferenceDate);
    }
}
