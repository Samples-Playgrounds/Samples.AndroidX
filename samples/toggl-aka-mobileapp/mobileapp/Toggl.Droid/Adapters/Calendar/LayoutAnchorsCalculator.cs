using Android.Content;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.UI.Calendar;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Adapters.Calendar
{
    public class LayoutAnchorsCalculator
    {
        private readonly CalendarLayoutCalculator calendarLayoutCalculator;
        private readonly IList<AnchorData> emptyAnchorData = ImmutableList<AnchorData>.Empty;
        private readonly int screenWidth;
        private readonly int anchorCount;
        private readonly float leftMargin;
        private readonly float leftPadding;
        private readonly float rightPadding;
        private readonly float itemSpacing;

        private readonly float minHourHeight;
        private readonly float hourHeight;
        private readonly float availableWidth;
        private readonly float halfAvailableWidth;

        public LayoutAnchorsCalculator(Context context, int screenWidth, int anchorCount, CalendarLayoutCalculator calendarLayoutCalculator)
        {
            this.screenWidth = screenWidth;
            this.anchorCount = anchorCount;
            this.calendarLayoutCalculator = calendarLayoutCalculator;
            hourHeight = 56.DpToPixels(context);
            minHourHeight = hourHeight / 4f;
            leftMargin = 68.DpToPixels(context);
            leftPadding = 4.DpToPixels(context);
            rightPadding = 4.DpToPixels(context);
            itemSpacing = 4.DpToPixels(context);
            availableWidth = this.screenWidth - leftMargin;
            halfAvailableWidth = availableWidth / 2f;
        }

        public List<Anchor> CalculateAnchors(IList<CalendarItem> calendarItems, bool hasCalendarsLinked)
        {
            var calendarEvents = new List<CalendarItem>();
            var timeEntries = new List<CalendarItem>();
            var originalIndexes = new Dictionary<CalendarItem, int>(calendarItems.Count);
            var adapterIndex = anchorCount;
            foreach (var calendarItem in calendarItems)
            {
                if (calendarItem.Source == CalendarItemSource.TimeEntry)
                    timeEntries.Add(calendarItem);
                else
                    calendarEvents.Add(calendarItem);
                originalIndexes.Add(calendarItem, adapterIndex++);
            }

            var newAnchors = new SparseArray<IList<AnchorData>>(anchorCount);

            if (hasCalendarsLinked || calendarEvents.Count > 0)
            {
                calculateAttributes(calendarEvents, halfAvailableWidth, leftMargin, originalIndexes, newAnchors);
                calculateAttributes(timeEntries, halfAvailableWidth, leftMargin + halfAvailableWidth, originalIndexes, newAnchors);
            }
            else
            {
                calculateAttributes(timeEntries, availableWidth, leftMargin, originalIndexes, newAnchors);
            }

            return Enumerable.Range(0, anchorCount)
                .Select(anchor => new Anchor((int)hourHeight, newAnchors.Get(anchor, emptyAnchorData).ToArray()))
                .ToList();
        }

        private void calculateAttributes(List<CalendarItem> calendarItems, float maxWidth, float groupLeftMargin, Dictionary<CalendarItem, int> originalIndexes, SparseArray<IList<AnchorData>> anchors)
        {
            var calendarItemLayoutAttributes = calendarLayoutCalculator.CalculateLayoutAttributes(calendarItems);

            for (var index = 0; index < calendarItemLayoutAttributes.Count; index++)
            {
                var calendarItem = calendarItemLayoutAttributes[index];
                var startHour = calendarItem.StartTime.Hour;
                var endHour = calendarItem.EndTime.Hour;
                var eventAbsolutePosition = calendarItem.StartTime.Hour * hourHeight + calendarItem.StartTime.Minute / 60f * hourHeight;
                var totalItemSpacing = (calendarItem.TotalColumns - 1) * itemSpacing;
                var eventWidth = (maxWidth - leftPadding - rightPadding - totalItemSpacing) / calendarItem.TotalColumns;
                var eventHeight = Math.Max(minHourHeight, hourHeight * (calendarItem.Duration.TotalMinutes / 60f));
                var adapterPosition = originalIndexes[calendarItems[index]];

                for (var anchorIndex = startHour; anchorIndex <= endHour; anchorIndex++)
                {
                    var anchorAbsoluteTop = anchorIndex * hourHeight;
                    var anchoredData = anchors.Get(anchorIndex, new List<AnchorData>());
                    var leftOffset = (int)(groupLeftMargin + leftPadding + eventWidth * calendarItem.ColumnIndex + calendarItem.ColumnIndex * itemSpacing);
                    anchoredData.Add(new AnchorData(adapterPosition, (int)(eventAbsolutePosition - anchorAbsoluteTop), leftOffset, (int)eventHeight, (int)eventWidth));
                    anchors.Put(anchorIndex, anchoredData);
                }
            }
        }
    }
}
