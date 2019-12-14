using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Toggl.Core.Reports;
using Toggl.Shared;

namespace Toggl.Droid.ViewHelpers
{
    public struct ReportsSummaryData
    {
        public readonly IReadOnlyList<ChartSegment> Segments;
        public readonly bool ShowEmptyState;
        public readonly TimeSpan TotalTime;
        public readonly bool TotalTimeIsZero;
        public readonly float BillablePercentage;
        public readonly DurationFormat DurationFormat;

        private ReportsSummaryData(
            IReadOnlyList<ChartSegment> segments,
            bool showEmptyState,
            TimeSpan totalTime,
            bool totalTimeIsZero,
            float billablePercentage,
            DurationFormat durationFormat)
        {
            Segments = segments;
            ShowEmptyState = showEmptyState;
            TotalTime = totalTime;
            TotalTimeIsZero = totalTimeIsZero;
            BillablePercentage = billablePercentage;
            DurationFormat = durationFormat;
        }

        public static ReportsSummaryData Empty()
        {
            return new ReportsSummaryData(ImmutableList<ChartSegment>.Empty, false, TimeSpan.Zero, true, 0f, DurationFormat.Classic);
        }

        public static ReportsSummaryData Create(
            IReadOnlyList<ChartSegment> segments,
            bool showEmptyState, TimeSpan totalTime,
            bool totalTimeIsZero,
            float? billablePercentage,
            DurationFormat durationFormat)
        {
            return new ReportsSummaryData(segments, showEmptyState, totalTime, totalTimeIsZero, billablePercentage ?? 0f, durationFormat);
        }
    }
}
