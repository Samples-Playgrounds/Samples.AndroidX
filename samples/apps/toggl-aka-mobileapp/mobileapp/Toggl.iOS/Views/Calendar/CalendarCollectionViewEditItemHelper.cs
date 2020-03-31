using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core;
using Toggl.Core.Calendar;
using Toggl.Core.Extensions;
using Toggl.Core.Helper;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.iOS.Cells.Calendar;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Calendar
{
    public sealed class CalendarCollectionViewEditItemHelper : CalendarCollectionViewAutoScrollHelper, IUIGestureRecognizerDelegate
    {
        enum EditAction
        {
            ChangeOffset,
            ChangeStartTime,
            ChangeEndTime,
            None
        }

        private static readonly TimeSpan defaultDuration = Constants.CalendarItemViewDefaultDuration;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        private readonly ITimeService timeService;
        private readonly CalendarCollectionViewSource dataSource;

        private readonly UIPanGestureRecognizer panGestureRecognizer;

        private CalendarItem originalCalendarItem;
        private CalendarItem calendarItem;
        private List<DateTimeOffset> allItemsStartAndEndTime;

        private nfloat verticalOffset;
        private CGPoint firstPoint;
        private CGPoint previousPoint;

        private bool isActive;
        private EditAction action;

        private bool didDragUp;
        private bool didDragDown;

        private DateTimeOffset? previousStartTime;
        private DateTimeOffset? previousEndTime;

        private readonly ISubject<CalendarItem> itemUpdatedSubject = new Subject<CalendarItem>();

        public IObservable<CalendarItem> ItemUpdated  => itemUpdatedSubject.AsObservable();

        public InputAction<CalendarItem?> StartEditingItem { get; }

        public CalendarCollectionViewEditItemHelper(
            UICollectionView CollectionView,
            ITimeService timeService,
            IRxActionFactory rxActionFactory,
            CalendarCollectionViewSource dataSource,
            CalendarCollectionViewLayout Layout) : base(CollectionView, Layout)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.timeService = timeService;
            this.dataSource = dataSource;

            StartEditingItem = rxActionFactory.FromAction<CalendarItem?>(startEditingItem);

            panGestureRecognizer = new UIPanGestureRecognizer(onPan)
            {
                Delegate = this
            };

            Layout.ScalingEnded
                .Subscribe(onLayoutScalingEnded)
                .DisposedBy(disposeBag);
        }

        public CalendarCollectionViewEditItemHelper(IntPtr handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            disposeBag.Dispose();
        }

        private void startEditingItem(CalendarItem? calendarItem)
        {
            if (isActive)
                stopEditingCurrentCell();

            if (calendarItem == null)
                return;

            if (!calendarItem.Value.IsEditable())
                return;

            allItemsStartAndEndTime = dataSource.AllItemsStartAndEndTime();

            this.calendarItem = calendarItem.Value;
            originalCalendarItem = this.calendarItem;

            dataSource.StartEditing(this.calendarItem);
            itemUpdatedSubject.OnNext(this.calendarItem);
            becomeActive();

            selectionFeedback.Prepare();
        }

        public void DiscardChanges()
        {
            if (!isActive)
                return;

            if (string.IsNullOrEmpty(originalCalendarItem.Id))
                return;

            itemUpdatedSubject.OnNext(originalCalendarItem);
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer gestureRecognizer, UITouch touch)
        {
            if (gestureRecognizer == panGestureRecognizer)
            {
                if (!isActive)
                    return true;

                var point = touch.LocationInView(CollectionView);
                var thereIsAnItemAtPoint = dataSource.CalendarItemAtPoint(point) != null;
                return thereIsAnItemAtPoint;
            }

            return true;
        }

        private void onPan(UIPanGestureRecognizer gesture)
        {
            var point = gesture.LocationInView(CollectionView);

            switch (gesture.State)
            {
                case UIGestureRecognizerState.Began:
                    panBegan(point);
                    break;

                case UIGestureRecognizerState.Changed:
                    panChanged(point);
                    break;

                case UIGestureRecognizerState.Ended:
                    panEnded();
                    break;
            }
        }

        private void panBegan(CGPoint point)
        {
            if (!isActive)
                return;

            allItemsStartAndEndTime = dataSource.AllItemsStartAndEndTime();

            firstPoint = point;
            LastPoint = point;

            previousStartTime = calendarItem.StartTime;
            previousEndTime = calendarItem.EndTime;

            var itemIndexPath = dataSource.IndexPathForEditingItem();
            var cell = CollectionView.CellForItem(itemIndexPath) as CalendarItemView;
            if (cell == null)
            {
                stopEditingCurrentCell();
                return;
            }
            var topDragRect = CollectionView.ConvertRectFromView(cell.TopDragTouchArea, cell);
            var bottomDragRect = CollectionView.ConvertRectFromView(cell.BottomDragTouchArea, cell);

            if (topDragRect.Contains(point))
                action = EditAction.ChangeStartTime;
            else if (bottomDragRect.Contains(point))
                action = EditAction.ChangeEndTime;
            else if (cell.Frame.Contains(point))
                action = EditAction.ChangeOffset;
            else
                action = EditAction.None;

            selectionFeedback.Prepare();
        }

        private void panChanged(CGPoint point)
        {
            onCurrentPointChanged(point);
            switch (action)
            {
                case EditAction.ChangeOffset:
                    changeOffset(point);
                    break;
                case EditAction.ChangeStartTime:
                    changeStartTime(point);
                    break;
                case EditAction.ChangeEndTime:
                    changeEndTime(point);
                    break;
            }
            previousPoint = point;
        }

        private void panEnded()
        {
            previousStartTime = previousEndTime = null;

            onCurrentPointChanged(null);
            StopAutoScroll();
            stopEditingCurrentCellIfNotVisible();
        }

        private void changeOffset(CGPoint point)
        {
            if (!isActive)
                return;

            var currentPointWithOffest = new CGPoint(point.X, point.Y - verticalOffset);

            var newStartTime = NewStartTimeWithStaticDuration(currentPointWithOffest, allItemsStartAndEndTime, calendarItem.Duration);

            LastPoint = point;
            var now = timeService.CurrentDateTime;

            if (newStartTime + calendarItem.Duration > newStartTime.Date.AddDays(1))
                return;

            calendarItem = calendarItem
                .WithStartTime(newStartTime);

            itemUpdatedSubject.OnNext(calendarItem);

            if (previousStartTime != newStartTime)
            {
                selectionFeedback.SelectionChanged();
                previousStartTime = newStartTime;
            }

            var topY = Layout.PointAtDate(calendarItem.StartTime).Y;
            var bottomY = Layout.PointAtDate(calendarItem.EndTime(now)).Y;
            if (topY < TopAutoScrollLine && !CollectionView.IsAtTop() && didDragUp)
                StartAutoScrollUp(changeOffset);
            else if (bottomY > BottomAutoScrollLine && !CollectionView.IsAtBottom() && didDragDown)
                StartAutoScrolDown(changeOffset);
            else
                StopAutoScroll();
        }

        private void changeStartTime(CGPoint point)
        {
            if (!isActive)
                return;

            if (point.Y < 0 || point.Y >= Layout.ContentViewHeight)
                return;

            LastPoint = point;
            var now = timeService.CurrentDateTime;

            var newStartTime = NewStartTimeWithDynamicDuration(point, allItemsStartAndEndTime);

            var newDuration = calendarItem.Duration.HasValue ? calendarItem.EndTime(now) - newStartTime : null as TimeSpan?;

            if (newDuration != null && newDuration <= TimeSpan.Zero ||
                newDuration == null && newStartTime > now)
                return;

            calendarItem = calendarItem
                .WithStartTime(newStartTime)
                .WithDuration(newDuration);

            itemUpdatedSubject.OnNext(calendarItem);

            if (previousStartTime != newStartTime)
            {
                selectionFeedback.SelectionChanged();
                previousStartTime = newStartTime;
            }

            if (point.Y < TopAutoScrollLine && !CollectionView.IsAtTop())
                StartAutoScrollUp(changeStartTime);
            else if (point.Y > BottomAutoScrollLine && calendarItem.Duration > defaultDuration)
                StartAutoScrolDown(changeStartTime);
            else
                StopAutoScroll();
        }

        private void changeEndTime(CGPoint point)
        {
            if (calendarItem.Duration == null || !isActive)
                return;

            if (point.Y < 0 || point.Y >= Layout.ContentViewHeight)
                return;

            LastPoint = point;

            var newEndTime = NewEndTimeWithDynamicDuration(point, allItemsStartAndEndTime);

            var newDuration = newEndTime - calendarItem.StartTime;

            if (newDuration <= TimeSpan.Zero)
                return;

            calendarItem = calendarItem
                .WithDuration(newDuration);

            itemUpdatedSubject.OnNext(calendarItem);

            if (previousEndTime != newEndTime)
            {
                selectionFeedback.SelectionChanged();
                previousEndTime = newEndTime;
            }

            if (point.Y > BottomAutoScrollLine && !CollectionView.IsAtBottom())
                StartAutoScrolDown(changeEndTime);
            else if (point.Y < TopAutoScrollLine && calendarItem.Duration > defaultDuration)
                StartAutoScrollUp(changeEndTime);
            else
                StopAutoScroll();
        }

        private void becomeActive()
        {
            isActive = true;
            CollectionView.AddGestureRecognizer(panGestureRecognizer);
        }

        private void resignActive()
        {
            isActive = false;
            CollectionView.RemoveGestureRecognizer(panGestureRecognizer);
        }

        private void onCurrentPointChanged(CGPoint? currentPoint)
        {
            if (currentPoint == null)
            {
                didDragUp = false;
                didDragDown = false;
                return;
            }

            if (currentPoint.Value.Y > previousPoint.Y)
            {
                didDragDown = true;
                didDragUp = false;
            }

            if (currentPoint.Value.Y < previousPoint.Y)
            {
                didDragUp = true;
                didDragDown = false;
            }
        }

        private void onLayoutScalingEnded()
        {
            if (!isActive) return;

            stopEditingCurrentCellIfNotVisible();
        }

        public void stopEditingCurrentCell()
        {
            resignActive();
            dataSource.StopEditing();
        }

        private void stopEditingCurrentCellIfNotVisible()
        {
            var itemIndexPath = dataSource.IndexPathForEditingItem();
            var cellNotVisible = CollectionView.CellForItem(itemIndexPath) == null;
            if (cellNotVisible)
            {
                stopEditingCurrentCell();
            }
        }
    }
}
