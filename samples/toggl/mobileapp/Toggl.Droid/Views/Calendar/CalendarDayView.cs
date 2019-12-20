using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Toggl.Core;
using Toggl.Core.Calendar;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Calendar;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHelpers;
using Toggl.Shared;
using Constants = Toggl.Core.Helper.Constants;
using Math = System.Math;

namespace Toggl.Droid.Views.Calendar
{
    [Register("toggl.droid.views.CalendarDayView")]
    public partial class CalendarDayView : View
    {
        private const int hoursPerDay = Constants.HoursPerDay;
        private const int vibrationDurationInMilliseconds = 5;
        private const int vibrationAmplitude = 7;
        private const int scrollAnimationDurationInMillis = 300;
        
        private readonly ISubject<CalendarItem?> calendarItemTappedSubject = new Subject<CalendarItem?>();
        private readonly ISubject<DateTimeOffset> emptySpansTouchedObservable = new Subject<DateTimeOffset>();
        private readonly ISubject<int> scrollOffsetSubject = new Subject<int>();
        private readonly RectF tapCheckRectF = new RectF();
        private readonly TimeSpan defaultDuration = TimeSpan.FromMinutes(30);

        private GestureDetector gestureDetector;
        private OverScroller scroller;
        private Handler handler;
        private ITimeService timeService;
        private CalendarLayoutCalculator calendarLayoutCalculator;
        private DateTime currentDate = DateTime.Now;
        private Paint currentHourPaint;
        private Vibrator hapticFeedbackProvider;
        private bool shouldDrawCurrentHourIndicator = false;
        private int scrollOffset;
        private bool isScrolling;
        private bool flingWasCalled;
        private float availableWidth;
        private int hourHeight;
        private int maxHeight;

        private ImmutableList<CalendarItem> originalCalendarItems = ImmutableList<CalendarItem>.Empty;
        private ImmutableList<CalendarItem> calendarItems = ImmutableList<CalendarItem>.Empty;
        private ImmutableList<CalendarItemRectAttributes> calendarItemLayoutAttributes = ImmutableList<CalendarItemRectAttributes>.Empty;
        
        public IObservable<CalendarItem?> CalendarItemTappedObservable
            => calendarItemTappedSubject.AsObservable();

        public IObservable<DateTimeOffset> EmptySpansTouchedObservable
            => emptySpansTouchedObservable.AsObservable();

        public IObservable<int> ScrollOffsetObservable
            => scrollOffsetSubject.AsObservable();

        #region Constructors

        protected CalendarDayView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CalendarDayView(Context context) : base(context)
        {
            init();
        }

        public CalendarDayView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init();
        }

        public CalendarDayView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init();
        }

        public CalendarDayView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            init();
        }

        #endregion

        private void init()
        {
            timeService = AndroidDependencyContainer.Instance.TimeService;
            calendarLayoutCalculator = new CalendarLayoutCalculator(timeService);
            hapticFeedbackProvider = (Vibrator)Context.GetSystemService(Context.VibratorService);
            var calendarGestureListener = new CalendarGestureListener(
                onTouchDown,
                onLongPress,
                scrollView,
                flingView,
                onSingleTapUp);
            gestureDetector = new GestureDetector(Context, calendarGestureListener);
            scroller = new OverScroller(Context);
            handler = new Handler(Looper.MainLooper);
            hourHeight = 56.DpToPixels(Context);
            maxHeight = hourHeight * 24;
            currentHourPaint = new Paint(PaintFlags.AntiAlias);
            currentHourPaint.Color = Context.SafeGetColor(Resource.Color.currentHourColor);
            currentHourPaint.StrokeWidth = 1.DpToPixels(Context);
            currentHourPaint.SetStyle(Paint.Style.FillAndStroke);

            initBackgroundBackingFields();
            initEventDrawingBackingFields();
            initEventEditionBackingFields();
        }

        public void DiscardEditModeChanges()
        {
            updateItemsAndRecalculateEventsAttrs(originalCalendarItems);
        }
        
        public void ClearEditMode()
        {
            editAction = EditAction.None;
            itemEditInEditMode = CalendarItemEditInfo.None;
            Invalidate();
        }
        
        public void SetCurrentDate(DateTimeOffset dateOnView)
        {
            currentDate = dateOnView.LocalDateTime.Date;
            var today = timeService.CurrentDateTime.LocalDateTime.Date;
            shouldDrawCurrentHourIndicator = currentDate == today;
        }

        public void UpdateCalendarHoursFormat(TimeFormat timeFormat)
        {
            timeOfDayFormat = timeFormat;
            var newHours = createHours();
            hours = newHours;
            Invalidate();
        }

        public void SetCurrentItemInEditMode(CalendarItem? calendarItemInEditMode)
        {
            var isCurrentlyEditingItem = isEditingItem();
            if (!calendarItemInEditMode.HasValue)
            {
                if (isCurrentlyEditingItem)
                {
                    cancelCurrentEdition();
                }
                else
                {
                    ClearEditMode();
                }
                return;
            }

            var calendarItem = calendarItemInEditMode.Value;
            if (!isCurrentlyEditingItem && calendarItem.Source == CalendarItemSource.Calendar)
                return;
            
            if (isCurrentlyEditingItem)
            {
                if (calendarItem.Id == itemEditInEditMode.CalendarItem.Id)
                    return;
                
                cancelCurrentEdition();
                if (calendarItem.Source == CalendarItemSource.Calendar)
                    return;
            }

            beginEdition(calendarItem);
        }
        
        private void beginEdition(CalendarItem calendarItem)
        {
            var calendarItemsToSearch = calendarItems;
            if (string.IsNullOrEmpty(calendarItem.Id))
            {
                var indexToInsertNewItem = calendarItemsToSearch.FindIndex(item => item.StartTime >= calendarItem.StartTime);
                indexToInsertNewItem = indexToInsertNewItem < 0 ? calendarItemsToSearch.Count : indexToInsertNewItem;
                updateItemsAndRecalculateEventsAttrs(calendarItemsToSearch.Insert(indexToInsertNewItem, calendarItem));
            }

            calendarItemsToSearch = calendarItems;
            var calendarLayoutItems = calendarItemLayoutAttributes;
            var calendarItemIndex = calendarItemsToSearch.IndexOf(calendarItem);
            if (calendarItemIndex < 0)
            {
                cancelCurrentEdition();
                return;
            }
            
            var itemInEditMode = new CalendarItemEditInfo(calendarItem, calendarLayoutItems[calendarItemIndex], calendarItemIndex, hourHeight, minHourHeight, timeService.CurrentDateTime);
            
            itemEditInEditMode = itemInEditMode;
            itemInEditMode.CalculateRect(itemInEditModeRect);
            updateEditingStartEndLabels();
            allItemsStartAndEndTime = selectItemsStartAndEndTime();
            Invalidate();
        }

        partial void initBackgroundBackingFields();
        partial void initEventDrawingBackingFields();
        partial void initEventEditionBackingFields();

        public override bool CanScrollHorizontally(int direction)
            => false;
        
        public override bool CanScrollVertically(int direction)
        {
            if (direction < 0)
                return scrollOffset > 0;
            return scrollOffset < maxHeight - Height;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            canvas.Save();
            canvas.Translate(0f, -scrollOffset);

            canvas.ClipRect(0, scrollOffset, Width, Height + scrollOffset);
            drawHourLines(canvas);
            drawCalendarItems(canvas);
            drawCurrentHourIndicator(canvas);

            canvas.Restore();
        }

        public void ScrollToCurrentHour(bool scrollSmoothly = false)
        {
            var hourOffset = calculateCurrentHourOffset();
            isScrolling = true;
            if (scrollSmoothly)
                scroller.StartScroll(0, scrollOffset, 0, hourOffset - scrollOffset, scrollAnimationDurationInMillis);
            else
                scroller.StartScroll(0, scrollOffset, 0, hourOffset - scrollOffset);
            
            continueScroll();
        }

        private int calculateCurrentHourOffset()
        {
            var now = timeService.CurrentDateTime.LocalDateTime;
            return (int)calculateHourOffsetFrom(now, hourHeight);
        }

        private void drawCurrentHourIndicator(Canvas canvas)
        {
            if (!shouldDrawCurrentHourIndicator) return;
            
            var currentHourY = calculateCurrentHourOffset();
            canvas.DrawLine(timeSliceStartX, currentHourY, Width, currentHourY, currentHourPaint);
            canvas.DrawCircle(timeSliceStartX, currentHourY, 4.DpToPixels(Context), currentHourPaint);
        }
        
        partial void drawHourLines(Canvas canvas);
        partial void drawCalendarItems(Canvas canvas);

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            availableWidth = Width - leftMargin;
            if (scrollOffsetIsPastBottomFrame())
            {
                scrollOffset = maxHeight - Height;
                scrollOffsetSubject.OnNext(scrollOffset);
            }
            availableWidth = Width - leftMargin;
            processBackgroundOnLayout(changed, left, top, right, bottom);
            processEventsOnLayout(changed, left, top, right, bottom);
        }

        public void SetOffset(int offset)
        {
            if (offset == scrollOffset) return;
            
            scrollOffset = offset;
            Invalidate();
        }

        public int GetOffset()
        {
            return scrollOffset;
        }
        
        partial void processBackgroundOnLayout(bool changed, int left, int top, int right, int bottom);
        partial void processEventsOnLayout(bool changed, int left, int top, int right, int bottom);

        private void continueScroll()
        {
            isScrolling = isScrolling && scroller.ComputeScrollOffset();
            if (!isScrolling)
            {
                Invalidate();
                return;
            }

            if (scroller.CurrY == scrollOffset)
            {
                handler.Post(continueScroll);
                return;
            }

            var oldScrollOffset = scrollOffset;
            scrollOffset = scroller.CurrY;

            if (scrollOffset < 0)
            {
                scrollOffset = 0;
            }
            else if (scrollOffsetIsPastBottomFrame())
            {
                scrollOffset = maxHeight - Height;
            }

            scrollOffsetSubject.OnNext(scrollOffset);
            OnScrollChanged(0, scrollOffset, 0, oldScrollOffset);

            handler.Post(continueScroll);
            Invalidate();
        }

        private bool scrollOffsetIsPastBottomFrame()
            => scrollOffset >= maxHeight - Height;

        private void onTouchDown(MotionEvent e1)
        {
            scroller.ForceFinished(true);
            flingWasCalled = false;
            handler.RemoveCallbacks(continueScroll);

            onTouchDownWhileEditingItem(e1);
            
            Invalidate();
        }

        private ImmutableList<DateTimeOffset> allItemsStartAndEndTime = ImmutableList<DateTimeOffset>.Empty;

        private void onLongPress(MotionEvent e1)
        {
            var touchX = e1.GetX();
            var touchY = e1.GetY();
            var calendarItemInfo = findCalendarItemFromPoint(touchX, touchY);
            
            if (calendarItemInfo.IsValid) 
                return;

            var startTime = dateAtYOffset(touchY + scrollOffset);
            var duration = defaultDuration;
            var teGaps = calendarLayoutCalculator.CalculateTwoHoursOrLessGapsLayoutAttributes(calendarItems);
            var matchingGap = teGaps.FirstOrDefault(gap => startTime > gap.StartTime && startTime < gap.StartTime + gap.Duration);
            duration = matchingGap.Duration == default ? duration : matchingGap.Duration;
            startTime = matchingGap.StartTime == default ? startTime : matchingGap.StartTime;
            
            var newCalendarItem = new CalendarItem("", CalendarItemSource.TimeEntry, startTime, duration, Shared.Resources.NewTimeEntry, CalendarIconKind.None);
            calendarItemTappedSubject.OnNext(newCalendarItem);
        }
        
        private ImmutableList<DateTimeOffset> selectItemsStartAndEndTime()
        {
            var calendarItemsToSelect = calendarItems;
            var startTimes = calendarItemsToSelect.Select(item => item.StartTime).Distinct();
            var endTimes = calendarItemsToSelect.Where(item => item.EndTime.HasValue).Select(item => (DateTimeOffset)item.EndTime).Distinct();
            return startTimes.Concat(endTimes).ToImmutableList();
        }

        private void onSingleTapUp(MotionEvent e1)
        {
            var touchX = e1.GetX();
            var touchY = e1.GetY();

            var calendarItemInfo = findCalendarItemFromPoint(touchX, touchY);
            var touchedEmptySpace = !calendarItemInfo.IsValid;

            if (touchedEmptySpace)
            {
                calendarItemTappedSubject.OnNext(null);
                return;
            }
            
            calendarItemTappedSubject.OnNext(calendarItemInfo.CalendarItem);
        }

        private void cancelCurrentEdition()
        {
            ClearEditMode();
            DiscardEditModeChanges();
        }
        
        private bool isEditingItem() => itemEditInEditMode.IsValid;

        private CalendarItemEditInfo findCalendarItemFromPoint(float x, float y)
        {
            var currentItemInEditMode = itemEditInEditMode;
            var calendarItemsAvailableDuringSearch = calendarItems;
            var itemsToSearch = calendarItemLayoutAttributes;

            if (currentItemInEditMode.IsValid)
            {
                currentItemInEditMode.CalculateRect(tapCheckRectF);
                if (tapCheckRectF.Contains(x, y + scrollOffset)) 
                    return currentItemInEditMode;
            }
            
            for (var i = 0; i < itemsToSearch.Count; i++)
            {
                if (currentItemInEditMode.IsValid && currentItemInEditMode.OriginalIndex == i) 
                    continue;
                
                var calendarItemAttr = itemsToSearch[i];
                calendarItemAttr.CalculateRect(hourHeight, minHourHeight, tapCheckRectF);
                if (tapCheckRectF.Contains(x, y + scrollOffset))
                    return new CalendarItemEditInfo(calendarItemsAvailableDuringSearch[i], itemsToSearch[i], i, hourHeight, minHourHeight, timeService.CurrentDateTime);
            }

            return CalendarItemEditInfo.None;
        }

        private void scrollView(MotionEvent e1, MotionEvent e2, float deltaX, float deltaY)
        {
            if (handleDragInEditMode(e1, e2, deltaX, deltaY)) 
                return;

            var oldScrollOffset = scrollOffset;
            scrollOffset += (int) deltaY;

            if (scrollOffset < 0)
            {
                scrollOffset = 0;
            }
            else if (scrollOffsetIsPastBottomFrame())
            {
                scrollOffset = maxHeight - Height;
            }
            
            scrollOffsetSubject.OnNext(scrollOffset);
            OnScrollChanged(0, scrollOffset, 0, oldScrollOffset);

            isScrolling = true;
            PostInvalidate();
        }

        private void flingView(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (editAction != EditAction.None) return;
            
            scroller.ForceFinished(true);

            flingWasCalled = true;
            isScrolling = true;
            scroller.Fling(0, scrollOffset, 0, (int) (-velocityY / 2f), 0, 0, 0, maxHeight);

            handler.Post(continueScroll);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    shouldTryToAutoScrollToEvent = false;
                    gestureDetector.OnTouchEvent(e);
                    return true;

                case MotionEventActions.Up:
                    gestureDetector.OnTouchEvent(e);
                    updateLayoutIfNeededDuringEdition();
                    if (scrollFrameToDisplayItemInEditModeIfNeeded())
                        return true;
                    if (flingWasCalled)
                        return true;
                    if (!isScrolling)
                        return true;

                    isScrolling = false;
                    Invalidate();
                    return true;

                case MotionEventActions.Move:
                    gestureDetector.OnTouchEvent(e);
                    return true;

                case MotionEventActions.Cancel:
                    gestureDetector.OnTouchEvent(e);
                    isScrolling = false;
                    return true;

                default:
                    return gestureDetector.OnTouchEvent(e) || base.OnTouchEvent(e);
            }
        }

        bool scrollFrameToDisplayItemInEditModeIfNeeded()
        {
            var currentItemInEditMode = itemEditInEditMode;
            if (!currentItemInEditMode.IsValid || !shouldTryToAutoScrollToEvent) return false;
            
            var startTime = currentItemInEditMode.CalendarItem.StartTime.ToLocalTime();
            var durationInPx = currentItemInEditMode.CalendarItem.Duration(timeService.CurrentDateTime.LocalDateTime).TotalHours * hourHeight;
            var eventTop = calculateHourOffsetFrom(startTime, hourHeight);
            var eventBottom = eventTop + durationInPx;
            
            var frameTop = scrollOffset;
            var frameBottom = Height + scrollOffset;

            if (eventTop < frameTop)
            {
                scroller.ForceFinished(true);
                isScrolling = false;
                autoScroll((int)-(Math.Abs(frameTop - eventTop) + autoScrollExtraDelta), true);
                return true;
            }

            if (eventBottom > frameBottom)
            {
                scroller.ForceFinished(true);
                isScrolling = false;
                autoScroll((int)Math.Abs(frameBottom - eventBottom) + autoScrollExtraDelta, true);
                return true;
            }

            return false;
        }

        void updateLayoutIfNeededDuringEdition()
        {
            var currentItemInEditMode = itemEditInEditMode;
            if (!currentItemInEditMode.IsValid) return;

            var newCalendarItem = currentItemInEditMode.CalendarItem;
            if (currentItemInEditMode.OriginalIndex == runningTimeEntryIndex)
            {
                newCalendarItem = newCalendarItem.WithDuration(null);
            }
            var newItems = calendarItems.SetItem(currentItemInEditMode.OriginalIndex, newCalendarItem);
            updateItemsAndRecalculateEventsAttrs(newItems);
        }

        private class CalendarGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private readonly Action<MotionEvent> onDown;
            private readonly Action<MotionEvent> onLongPress;
            private readonly Action<MotionEvent, MotionEvent, float, float> onScroll;
            private readonly Action<MotionEvent, MotionEvent, float, float> onFling;
            private readonly Action<MotionEvent> onSingleTapUp;

            public CalendarGestureListener(Action<MotionEvent> onDown,
                Action<MotionEvent> onLongPress,
                Action<MotionEvent, MotionEvent, float, float> onScroll,
                Action<MotionEvent, MotionEvent, float, float> onFling,
                Action<MotionEvent> onSingleTapUp)
            {
                this.onSingleTapUp = onSingleTapUp;
                this.onLongPress = onLongPress;
                this.onFling = onFling;
                this.onScroll = onScroll;
                this.onDown = onDown;
            }

            public override bool OnDown(MotionEvent e)
            {
                onDown(e);
                return true;
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                onFling(e1, e2, velocityX, velocityY);
                return true;
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                onScroll(e1, e2, distanceX, distanceY);
                return true;
            }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                onSingleTapUp(e);
                return true;
            }

            public override void OnLongPress(MotionEvent e)
            {
                onLongPress(e);
            }
        }
    }
}