using System;

namespace Toggl.Shared.Extensions
{
    public static class BeginningOfWeekExtensions
    {
        public static string ToLocalizedString(this BeginningOfWeek beginningOfWeek)
        {
            switch (beginningOfWeek)
            {
                case BeginningOfWeek.Monday:
                    return Resources.Monday;

                case BeginningOfWeek.Tuesday:
                    return Resources.Tuesday;

                case BeginningOfWeek.Wednesday:
                    return Resources.Wednesday;

                case BeginningOfWeek.Thursday:
                    return Resources.Thursday;

                case BeginningOfWeek.Friday:
                    return Resources.Friday;

                case BeginningOfWeek.Saturday:
                    return Resources.Saturday;

                case BeginningOfWeek.Sunday:
                    return Resources.Sunday;
                default:
                    throw new ArgumentException($"Unsupported value of {nameof(BeginningOfWeek)}");
            }
        }
    }
}
