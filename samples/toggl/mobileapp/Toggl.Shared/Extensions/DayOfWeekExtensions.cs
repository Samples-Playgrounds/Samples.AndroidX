using System;

namespace Toggl.Shared.Extensions
{
    public static class DayOfWeekExtensions
    {
        public static string Initial(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return Resources.MondayInitial;

                case DayOfWeek.Tuesday:
                    return Resources.TuesdayInitial;

                case DayOfWeek.Wednesday:
                    return Resources.WednesdayInitial;

                case DayOfWeek.Thursday:
                    return Resources.ThursdayInitial;

                case DayOfWeek.Friday:
                    return Resources.FridayInitial;

                case DayOfWeek.Saturday:
                    return Resources.SaturdayInitial;

                case DayOfWeek.Sunday:
                    return Resources.SundayInitial;

                default:
                    throw new ArgumentException($"Unsupported value of {nameof(DayOfWeek)}");
            }
        }

        public static string FullName(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return Resources.Monday;

                case DayOfWeek.Tuesday:
                    return Resources.Tuesday;

                case DayOfWeek.Wednesday:
                    return Resources.Wednesday;

                case DayOfWeek.Thursday:
                    return Resources.Thursday;

                case DayOfWeek.Friday:
                    return Resources.Friday;

                case DayOfWeek.Saturday:
                    return Resources.Saturday;

                case DayOfWeek.Sunday:
                    return Resources.Sunday;

                default:
                    throw new ArgumentException($"Unsupported value of {nameof(DayOfWeek)}");
            }
        }
    }
}
