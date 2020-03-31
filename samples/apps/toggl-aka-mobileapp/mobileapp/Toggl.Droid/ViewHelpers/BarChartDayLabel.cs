namespace Toggl.Droid.ViewHelpers
{
    public struct BarChartDayLabel
    {
        public readonly string DayOfWeek;
        public readonly string Date;

        public BarChartDayLabel(string dayOfWeek, string date)
        {
            DayOfWeek = dayOfWeek;
            Date = date;
        }
    }
}
