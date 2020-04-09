using Toggl.Shared;

namespace Toggl.Core.Models
{
    public enum ReportPeriod
    {
        Unknown = 0,
        Today,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisYear,
        LastYear
    }

    public static class ReportPeriodExtensions
    {
        public static string ToHumanReadableString(this ReportPeriod p)
        {
            switch (p)
            {
                case ReportPeriod.LastMonth:
                    return Resources.LastMonth;
                case ReportPeriod.LastWeek:
                    return Resources.LastWeek;
                case ReportPeriod.Yesterday:
                    return Resources.Yesterday;
                case ReportPeriod.Today:
                    return Resources.Today;
                case ReportPeriod.ThisWeek:
                    return Resources.ThisWeek;
                case ReportPeriod.ThisMonth:
                    return Resources.ThisMonth;
                case ReportPeriod.ThisYear:
                    return Resources.ThisYear;
                case ReportPeriod.LastYear:
                    return Resources.LastYear;
                default:
                    return Resources.Unknown;
            }
        }
    }
}
