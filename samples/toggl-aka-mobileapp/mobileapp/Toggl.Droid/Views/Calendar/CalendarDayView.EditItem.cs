using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Graphics;
using Android.Views;
using Toggl.Core.Calendar;
using Toggl.Core.Extensions;
using Toggl.Droid.Extensions;
using Toggl.Droid.Helper;
using Toggl.Droid.ViewHelpers;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Views.Calendar
{
    public partial class CalendarDayView
    {
        private const int smoothAutoScrollDurationInMillis = 300;
        private readonly RectF dragTopRect = new RectF();
        private readonly RectF dragBottomRect = new RectF();

        private string startHourLabel = string.Empty;
        private string endHourLabel = string.Empty;
        private int distanceFromItemTopAndFirstTouch;
        private int handleTouchExtraMargins;
        private int autoScrollExtraDelta;
        private bool shouldTryToAutoScrollToEvent = false;
        private DateTimeOffset previousStartTime;
        private DateTimeOffset previousEndTime;
        private EditAction editAction = EditAction.None;

        partial void initEventEditionBackingFields()
        {
            handleTouchExtraMargins = 24.DpToPixels(Context);
            autoScrollExtraDelta = 5.DpToPixels(Context);
        }

        private void onTouchDownWhileEditingItem(MotionEvent e1)
        {
            var activeItemInfo = itemEditInEditMode;
            if (!activeItemInfo.IsValid) return;

            calculateTopDragRect(activeItemInfo);
            calculateBottomDragRect(activeItemInfo);
            activeItemInfo.CalculateRect(tapCheckRectF);

            var touchX = e1.GetX();
            var touchY = e1.GetY();

            if (dragTopRect.Contains(touchX, touchY + scrollOffset))
            {
                editAction = EditAction.ChangeStart;
            }
            else if (activeItemInfo.OriginalIndex == runningTimeEntryIndex)
            {
                editAction = EditAction.None;
            }
            else if (dragBottomRect.Contains(touchX, touchY + scrollOffset))
            {
                editAction = EditAction.ChangeEnd;
            }
            else if (tapCheckRectF.Contains(touchX, touchY + scrollOffset))
            {
                var topDuringOffsetChangeStart = (int)tapCheckRectF.Top;
                var firstTouchYDuringOffsetChangeStart = (int) e1.GetY() + scrollOffset;
                distanceFromItemTopAndFirstTouch = firstTouchYDuringOffsetChangeStart - topDuringOffsetChangeStart;
                editAction = EditAction.ChangeOffset;
            }
            else
            {
                editAction = EditAction.None;
            }
        }

        private void calculateTopDragRect(CalendarItemEditInfo activeItemEditInfo)
        {
            activeItemEditInfo.CalculateRect(dragTopRect);
            dragTopRect.Bottom = dragTopRect.Top + handleTouchExtraMargins;
            dragTopRect.Top -= handleTouchExtraMargins;
            dragTopRect.Left = dragTopRect.Right - handleTouchExtraMargins;
            dragTopRect.Right += handleTouchExtraMargins;
        }

        private void calculateBottomDragRect(CalendarItemEditInfo activeItemEditInfo)
        {
            activeItemEditInfo.CalculateRect(dragBottomRect);
            dragBottomRect.Top = dragBottomRect.Bottom - handleTouchExtraMargins;
            dragBottomRect.Bottom += handleTouchExtraMargins;
            dragBottomRect.Right = dragBottomRect.Left + handleTouchExtraMargins;
            dragBottomRect.Left -= handleTouchExtraMargins;
        }

        private bool handleDragInEditMode(MotionEvent e1, MotionEvent e2, float deltaX, float deltaY)
        {
            var calendarItemInfoInEditMode = itemEditInEditMode;
            if (!calendarItemInfoInEditMode.IsValid) return false;

            var histCount = e2.HistorySize;
            var avgYtouch = 0f;
            for (var i = 0; i < histCount; i++)
            {
                avgYtouch += e2.GetHistoricalY(0, i);
            }

            var touchY = histCount > 0 ? avgYtouch / histCount : e2.GetY();

            switch (editAction)
            {
                case EditAction.ChangeStart:
                    changeStartTime(touchY - deltaY + scrollOffset);
                    break;

                case EditAction.ChangeEnd:
                    changeEndTime(touchY - deltaY + scrollOffset);
                    break;

                case EditAction.ChangeOffset:
                    changeOffset(touchY + -deltaY + scrollOffset - distanceFromItemTopAndFirstTouch);
                    break;

                case EditAction.None:
                    return false;

                default:
                    return false;
            }

            Invalidate();
            return true;
        }

        private void changeStartTime(float touchY)
        {
            if (!itemEditInEditMode.IsValid)
                return;

            if (touchY < 0 || touchY >= maxHeight)
                return;

            itemInEditModeRect.Top = touchY;

            var itemToEdit = itemEditInEditMode;
            var calendarItem = itemToEdit.CalendarItem;

            var now = timeService.CurrentDateTime;

            var newStartTime = newStartTimeWithDynamicDuration(touchY, allItemsStartAndEndTime);
            var newDuration = calendarItem.Duration.HasValue ? calendarItem.EndTime(now) - newStartTime : null as TimeSpan?;

            if (newDuration != null && newDuration <= TimeSpan.Zero ||
                newDuration == null && newStartTime > now)
                return;

            calendarItem = calendarItem
                .WithStartTime(newStartTime)
                .WithDuration(newDuration);

            updateItemInEditMode(calendarItem.StartTime, calendarItem.Duration(now));

            if (previousStartTime != newStartTime)
            {
                vibrate();
                previousStartTime = newStartTime;
                shouldTryToAutoScrollToEvent = true;
            }

            if (isScrolling) return;

            if (touchY < scrollOffset)
                autoScroll((int) -(scrollOffset - touchY + autoScrollExtraDelta));
            else
                stopScroll();
        }

        private void changeEndTime(float touchY)
        {
            if (!itemEditInEditMode.IsValid)
                return;

            var itemToEdit = itemEditInEditMode;
            if (itemToEdit.CalendarItem.Duration == null)
                return;

            if (touchY < 0 || touchY >= maxHeight)
                return;
            
            itemInEditModeRect.Bottom = touchY;

            var now = timeService.CurrentDateTime;
            var calendarItem = itemToEdit.CalendarItem;

            var newEndTime = newEndTimeWithDynamicDuration(touchY, allItemsStartAndEndTime);
            var newDuration = newEndTime - calendarItem.StartTime;

            if (newDuration <= TimeSpan.Zero || newEndTime >= currentDate.AddDays(1))
                return;

            calendarItem = calendarItem
                .WithStartTime(calendarItem.StartTime.ToLocalTime())
                .WithDuration(newDuration);

            updateItemInEditMode(calendarItem.StartTime, calendarItem.Duration(now));

            if (previousEndTime != newEndTime)
            {
                vibrate();
                previousEndTime = newEndTime;
                shouldTryToAutoScrollToEvent = true;
            }

            if (isScrolling) return;

            if (touchY > Height + scrollOffset)
                autoScroll((int) (touchY - (Height + scrollOffset) + autoScrollExtraDelta));
            else
                stopScroll();
        }

        private void changeOffset(float touchY)
        {
            if (!itemEditInEditMode.IsValid)
                return;

            var itemToEdit = itemEditInEditMode;
            var calendarItem = itemToEdit.CalendarItem;
            
            if (touchY < 0 || touchY >= maxHeight)
                return;
            
            var newBottom = touchY + itemInEditModeRect.Height();
            if (newBottom < 0 || newBottom >= maxHeight)
                return;
            
            itemInEditModeRect.Top = touchY;
            itemInEditModeRect.Bottom = newBottom;

            var newStartTime = newStartTimeWithStaticDuration(touchY, allItemsStartAndEndTime, calendarItem.Duration);

            var now = timeService.CurrentDateTime;

            if (newStartTime + calendarItem.Duration >= currentDate.AddDays(1))
                return;

            calendarItem = calendarItem
                .WithStartTime(newStartTime);

            updateItemInEditMode(calendarItem.StartTime.ToLocalTime(), calendarItem.Duration(now));

            if (previousStartTime != newStartTime)
            {
                vibrate();
                previousStartTime = newStartTime;
                shouldTryToAutoScrollToEvent = true;
            }

            var duration = calendarItem.Duration(now);
            var durationInPx = duration.TotalHours * hourHeight;
            
            if (isScrolling) return;
            
            if (touchY < scrollOffset)
                autoScroll((int) -(scrollOffset - touchY + autoScrollExtraDelta));
            else if (touchY + durationInPx > Height + scrollOffset)
                autoScroll((int) (touchY + durationInPx - (Height + scrollOffset) + autoScrollExtraDelta));
            else
                stopScroll();
        }

        private void autoScroll(int deltaY, bool smoothly = false)
        {
            if (isScrolling) return;
            
            scroller.ForceFinished(true);
            isScrolling = true;
            if (smoothly)
                scroller.StartScroll(0, scrollOffset, 0, deltaY);
            else
                scroller.StartScroll(0, scrollOffset, 0, deltaY, smoothAutoScrollDurationInMillis);

            handler.Post(continueScroll);
        }

        private void stopScroll()
        {
            scroller.ForceFinished(true);
            isScrolling = false;
        }

        private void updateItemInEditMode(DateTimeOffset startTime, TimeSpan duration)
        {
            var newCalendarItem = itemEditInEditMode.CalendarItem
                .WithStartTime(startTime.ToLocalTime())
                .WithDuration(duration);

            itemEditInEditMode = itemEditInEditMode.WithCalendarItem(newCalendarItem, hourHeight, minHourHeight, timeService.CurrentDateTime);

            updateEditingStartEndLabels();
            notifyUpdateInItemInEditMode();
        }

        private void notifyUpdateInItemInEditMode()
        {
            if (itemEditInEditMode.OriginalIndex != runningTimeEntryIndex)
            {
                calendarItemTappedSubject.OnNext(itemEditInEditMode.CalendarItem);
                return;
            }

            calendarItemTappedSubject.OnNext(itemEditInEditMode.CalendarItem.WithDuration(null));
        }

        private void updateEditingStartEndLabels()
        {
            var calendarItem = itemEditInEditMode.CalendarItem;
            var startHour = calendarItem.StartTime.ToLocalTime();
            startHourLabel = formatEditingHour(startHour.DateTime);
            if (calendarItem.EndTime.HasValue)
            {
                var endHour = calendarItem.EndTime.Value.ToLocalTime();
                endHourLabel = formatEditingHour(endHour.DateTime);
            }
        }

        private string formatEditingHour(DateTime hour)
            => hour.ToString(editingHoursFormat(), CultureInfo.CurrentCulture);

        private string editingHoursFormat()
            => timeOfDayFormat.IsTwentyFourHoursFormat
                ? Shared.Resources.EditingTwentyFourHoursFormat
                : Shared.Resources.EditingTwelveHoursFormat;

        private DateTimeOffset newStartTimeWithDynamicDuration(float yOffset, IList<DateTimeOffset> currentItemsStartAndEndTimes)
        {
            if (!currentItemsStartAndEndTimes.Any())
            {
                return dateAtYOffset(yOffset).ToLocalTime().RoundToClosestQuarter();
            }

            var newStartTime = dateAtYOffset(yOffset).ToLocalTime();

            var startSnappingPointDifference = distanceToClosestSnappingPoint(newStartTime, currentItemsStartAndEndTimes.Append(newStartTime.RoundToClosestQuarter()));

            newStartTime += startSnappingPointDifference;

            return newStartTime;
        }

        private DateTimeOffset newEndTimeWithDynamicDuration(float yOffset, IList<DateTimeOffset> currentItemsStartAndEndTimes)
        {
            if (!currentItemsStartAndEndTimes.Any())
            {
                return dateAtYOffset(yOffset).ToLocalTime().RoundToClosestQuarter();
            }

            var newEndTime = dateAtYOffset(yOffset).ToLocalTime();

            var endSnappingPointDifference = distanceToClosestSnappingPoint((DateTimeOffset) newEndTime, currentItemsStartAndEndTimes.Append(newEndTime.RoundToClosestQuarter()));

            newEndTime += endSnappingPointDifference;

            return newEndTime;
        }

        private DateTimeOffset newStartTimeWithStaticDuration(float yOffset, IList<DateTimeOffset> currentItemsStartAndEndTimes, TimeSpan? duration)
        {
            if (!currentItemsStartAndEndTimes.Any())
            {
                return dateAtYOffset(yOffset).ToLocalTime().RoundToClosestQuarter();
            }

            var newStartTime = dateAtYOffset(yOffset).ToLocalTime();
            var newEndTime = duration.HasValue ? newStartTime + duration : null;

            var startSnappingPointDifference = distanceToClosestSnappingPoint(newStartTime, currentItemsStartAndEndTimes.Append(newStartTime.RoundToClosestQuarter()));

            if (newEndTime.HasValue)
            {
                var endSnappingPointDifference = distanceToClosestSnappingPoint((DateTimeOffset) newEndTime, currentItemsStartAndEndTimes);
                var snappingPointDifference = startSnappingPointDifference.Positive() < endSnappingPointDifference.Positive() ? startSnappingPointDifference : endSnappingPointDifference;
                newStartTime += snappingPointDifference;
            }
            else
            {
                newStartTime += startSnappingPointDifference;
            }

            return newStartTime;
        }

        private TimeSpan distanceToClosestSnappingPoint(DateTimeOffset time, IEnumerable<DateTimeOffset> data)
            => data.Aggregate(TimeSpan.MaxValue,
                (min, next) => min.Positive() <= (next - time).Positive()
                    ? min
                    : next - time);

        private DateTimeOffset dateAtYOffset(float y)
        {
            var seconds = (y / hourHeight) * 60 * 60;
            var timespan = TimeSpan.FromSeconds(seconds);
            var nextDay = currentDate.AddDays(1);

            var offset = currentDate + timespan;

            if (offset < currentDate)
                return currentDate;
            if (offset > nextDay)
                return nextDay;

            return currentDate + timespan;
        }

        private void vibrate()
        {
            hapticFeedbackProvider?.ActivateVibration(vibrationDurationInMilliseconds, vibrationAmplitude);
        }

        private static float calculateHourOffsetFrom(DateTimeOffset dateTimeOffset, float hourHeight)
        {
            return (dateTimeOffset.Hour + dateTimeOffset.Minute / 60f) * hourHeight;
        }

        private enum EditAction
        {
            ChangeStart,
            ChangeEnd,
            ChangeOffset,
            None
        }

        private struct CalendarItemEditInfo
        {
            public CalendarItem CalendarItem { get; private set; }
            public int OriginalIndex { get; private set; }
            public bool IsValid { get; private set; }
            public bool HasChanged { get; private set; }

            private float top;
            private float bottom;
            private float left;
            private float right;

            public static readonly CalendarItemEditInfo None = new CalendarItemEditInfo { IsValid = false };

            public CalendarItemEditInfo(CalendarItem calendarItem, CalendarItemRectAttributes attributes, int originalIndex, float hourHeight, float minHeight, DateTimeOffset now)
            {
                var startTime = calendarItem.StartTime.LocalDateTime;
                var duration = calendarItem.Duration(now);
                CalendarItem = calendarItem;
                OriginalIndex = originalIndex;
                IsValid = true;
                HasChanged = false;
                top = calculateHourOffsetFrom(startTime, hourHeight);
                bottom = calculateHourOffsetFrom(startTime + duration, hourHeight);
                left = attributes.Left;
                right = attributes.Right;

                enforceMinHeight(minHeight);
            }

            public CalendarItemEditInfo WithCalendarItem(CalendarItem calendarItem, float hourHeight, float minHeight, DateTimeOffset now)
            {
                var startTime = calendarItem.StartTime.LocalDateTime;
                var duration = calendarItem.Duration(now);
                var newCalendarItemEditInfo = new CalendarItemEditInfo
                {
                    CalendarItem = calendarItem,
                    OriginalIndex = OriginalIndex,
                    IsValid = true,
                    HasChanged = true,
                    top = calculateHourOffsetFrom(startTime, hourHeight),
                    bottom = calculateHourOffsetFrom(startTime + duration, hourHeight),
                    left = left,
                    right = right
                };
                newCalendarItemEditInfo.enforceMinHeight(minHeight);

                return newCalendarItemEditInfo;
            }

            private void enforceMinHeight(float minHeight)
            {
                if (bottom - top < minHeight)
                {
                    bottom = top + minHeight;
                }
            }

            public void CalculateRect(RectF outRectF)
            {
                outRectF.Set(left, top, right, bottom);
            }
        }
    }
}