using Toggl.Core.Models;
using Toggl.iOS.Intents;

namespace Toggl.iOS.Extensions
{
    public static class IntentPeriodExtension
    {
        public static ShowReportPeriodReportPeriod ToShowReportPeriodReportPeriod(this ReportPeriod p)
        {
            switch (p)
            {
                case ReportPeriod.LastMonth:
                    return ShowReportPeriodReportPeriod.LastMonth;
                case ReportPeriod.LastWeek:
                    return ShowReportPeriodReportPeriod.LastWeek;
                case ReportPeriod.Yesterday:
                    return ShowReportPeriodReportPeriod.Yesterday;
                case ReportPeriod.Today:
                    return ShowReportPeriodReportPeriod.Today;
                case ReportPeriod.ThisWeek:
                    return ShowReportPeriodReportPeriod.ThisWeek;
                case ReportPeriod.ThisMonth:
                    return ShowReportPeriodReportPeriod.ThisMonth;
                case ReportPeriod.ThisYear:
                    return ShowReportPeriodReportPeriod.ThisYear;
                case ReportPeriod.LastYear:
                    return ShowReportPeriodReportPeriod.LastYear;
                default:
                    return ShowReportPeriodReportPeriod.Unknown;
            }
        }
        public static ReportPeriod ToReportPeriod(this ShowReportPeriodReportPeriod intentPeriod)
        {
            switch (intentPeriod)
            {
                case ShowReportPeriodReportPeriod.Today:
                    return ReportPeriod.Today;
                case ShowReportPeriodReportPeriod.Yesterday:
                    return ReportPeriod.Yesterday;
                case ShowReportPeriodReportPeriod.LastMonth:
                    return ReportPeriod.LastMonth;
                case ShowReportPeriodReportPeriod.ThisMonth:
                    return ReportPeriod.ThisMonth;
                case ShowReportPeriodReportPeriod.LastWeek:
                    return ReportPeriod.LastWeek;
                case ShowReportPeriodReportPeriod.ThisWeek:
                    return ReportPeriod.ThisWeek;
                case ShowReportPeriodReportPeriod.ThisYear:
                    return ReportPeriod.ThisYear;
                case ShowReportPeriodReportPeriod.LastYear:
                    return ReportPeriod.LastYear;
                default:
                    return ReportPeriod.Unknown;
            }
        }
    }
}
