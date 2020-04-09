using System;
using Toggl.Shared;

namespace Toggl.Core.UI.Calendar
{
    public struct CalendarItemLayoutAttributes
    {
        public DateTime StartTime { get; }

        public TimeSpan Duration { get; }

        public DateTime EndTime => StartTime + Duration;

        public int TotalColumns { get; }

        public int ColumnIndex { get; }

        public CalendarItemLayoutAttributes(
            DateTime startTime,
            TimeSpan duration,
            int totalColumns,
            int columnIndex)
        {
            Ensure.Argument.IsNotZero(totalColumns, nameof(totalColumns));
            Ensure.Argument.IsInClosedRange(columnIndex, 0, totalColumns - 1, nameof(totalColumns));

            StartTime = startTime;
            Duration = duration;
            TotalColumns = totalColumns;
            ColumnIndex = columnIndex;
        }
    }
}
