using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Toggl.Core.Conversions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;

namespace Toggl.Droid.ViewHelpers
{
    public struct BarChartData
    {
        public readonly string StartDate;
        public readonly string EndDate;
        public readonly IImmutableList<BarViewModel> Bars;
        public readonly int MaximumHoursPerBar;
        public readonly IImmutableList<BarChartDayLabel> HorizontalLabels;
        public readonly bool WorkspaceIsBillable;

        private BarChartData(DateTimeOffset startDate, DateTimeOffset endDate, bool workspaceIsBillable, DateFormat dateFormat, IImmutableList<BarViewModel> bars, int maximumHoursPerBar, IImmutableList<DateTimeOffset> horizontalLegend)
        {
            StartDate = startDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
            EndDate = endDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
            Bars = bars;
            MaximumHoursPerBar = maximumHoursPerBar;
            WorkspaceIsBillable = workspaceIsBillable;
            HorizontalLabels =
                horizontalLegend
                    ?.Select(date => new BarChartDayLabel(DateTimeOffsetConversion.ToDayOfWeekInitial(date), date.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture)))
                    .ToImmutableList()
                ?? ImmutableList<BarChartDayLabel>.Empty;
        }

        public static BarChartData Create(DateTimeOffset startDate, DateTimeOffset endDate, bool workspaceIsBillable, DateFormat dateFormat, IImmutableList<BarViewModel> bars, int maximumHoursPerBar, IImmutableList<DateTimeOffset> horizontalLegend)
            => new BarChartData(startDate, endDate, workspaceIsBillable, dateFormat, bars, maximumHoursPerBar, horizontalLegend);
    }
}
