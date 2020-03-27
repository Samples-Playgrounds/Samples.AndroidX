using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using Constants = Toggl.Core.Helper.Constants;
using Math = System.Math;

namespace Toggl.iOS.Views.Calendar
{
    public sealed class CalendarCollectionViewCreateFromSpanHelper : CalendarCollectionViewAutoScrollHelper, IUIGestureRecognizerDelegate
    {
        private static readonly TimeSpan defaultDuration = Constants.CalendarItemViewDefaultDuration;

        private readonly CalendarCollectionViewSource dataSource;

        private UILongPressGestureRecognizer longPressGestureRecognizer;

        private CGPoint firstPoint;

        private TimeSpan previousDuration;
        private DateTimeOffset previousStart;

        private List<DateTimeOffset> allItemsStartAndEndTime;

        public CalendarCollectionViewCreateFromSpanHelper(
            UICollectionView collectionView,
            CalendarCollectionViewSource dataSource,
            CalendarCollectionViewLayout Layout) : base(collectionView, Layout)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;

            longPressGestureRecognizer = new UILongPressGestureRecognizer(onLongPress);
            longPressGestureRecognizer.Delegate = this;
            collectionView.AddGestureRecognizer(longPressGestureRecognizer);
        }

        public CalendarCollectionViewCreateFromSpanHelper(IntPtr handle) : base(handle)
        {
        }

        [Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
        public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
            if (gestureRecognizer == longPressGestureRecognizer)
                return otherGestureRecognizer is UILongPressGestureRecognizer;
            else
                return false;
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer gestureRecognizer, UITouch touch)
        {
            if (gestureRecognizer == longPressGestureRecognizer)
            {
                var point = touch.LocationInView(CollectionView);
                var thereIsNoItemAtPoint = dataSource.CalendarItemAtPoint(point) == null;
                var isNotEditing = dataSource.IsEditing == false;
                return thereIsNoItemAtPoint && isNotEditing;
            }

            return true;
        }

        private void onLongPress(UILongPressGestureRecognizer gesture)
        {
            var point = gesture.LocationInView(CollectionView);

            switch (gesture.State)
            {
                case UIGestureRecognizerState.Began when !dataSource.IsEditing && dataSource.CalendarItemAtPoint(point) == null:
                    longPressBegan(point);
                    break;

                case UIGestureRecognizerState.Changed when dataSource.IsEditing:
                    longPressChanged(point);
                    break;

                case UIGestureRecognizerState.Ended when dataSource.IsEditing:
                    longPressEnded(point);
                    break;

                case UIGestureRecognizerState.Failed:
                    break;

                case UIGestureRecognizerState.Cancelled:
                    break;
            }
        }

        private void longPressBegan(CGPoint point)
        {
            allItemsStartAndEndTime = dataSource.AllItemsStartAndEndTime();

            dataSource.StartEditing();
            firstPoint = point;
            LastPoint = point;
            var startTime = Layout.DateAtPoint(firstPoint).RoundDownToClosestQuarter();
            var duration = defaultDuration;
            var teGaps = dataSource.GapsBetweenTimeEntriesOf2HoursOrLess();
            if (teGaps.Any())
            {
                foreach (var gap in teGaps)
                {
                    if (startTime > gap.StartTime && startTime < gap.StartTime + gap.Duration)
                    {
                        duration = gap.Duration;
                        startTime = gap.StartTime;
                        break;
                    }
                }
            }

            dataSource.InsertItemView(startTime, duration);
            impactFeedback.ImpactOccurred();
            selectionFeedback.Prepare();
            previousDuration = duration;
            previousStart = startTime;
        }

        private bool isDraggingDown(CGPoint point) => firstPoint.Y < point.Y;

        private void longPressChanged(CGPoint point)
        {
            if (Math.Sqrt(Math.Pow(point.X - firstPoint.X, 2) + Math.Pow(point.Y - firstPoint.Y, 2)) < 30)
                return;
            
            LastPoint = point;

            DateTimeOffset startTime;
            DateTimeOffset endTime;

            if (isDraggingDown(point))
            {
                startTime = Layout.DateAtPoint(firstPoint).RoundDownToClosestQuarter();
                endTime = NewEndTimeWithDynamicDuration(point, allItemsStartAndEndTime);

                if (point.Y > BottomAutoScrollLine)
                    StartAutoScrolDown(longPressChanged);
                else
                    StopAutoScroll();
            }
            else
            {
                startTime = NewStartTimeWithDynamicDuration(point, allItemsStartAndEndTime);
                endTime = Layout.DateAtPoint(firstPoint).RoundDownToClosestQuarter();

                if (point.Y < TopAutoScrollLine)
                    StartAutoScrollUp(longPressChanged);
                else
                    StopAutoScroll();
            }

            var duration = endTime - startTime;

            dataSource.UpdateItemView(startTime, duration);

            if (duration != previousDuration)
            {
                selectionFeedback.SelectionChanged();
                previousDuration = duration;
                previousStart = startTime;
            }
        }

        private void longPressEnded(CGPoint point)
        {
            dataSource.StopEditing();
            StopAutoScroll();

            impactFeedback.ImpactOccurred();
        }
    }
}
