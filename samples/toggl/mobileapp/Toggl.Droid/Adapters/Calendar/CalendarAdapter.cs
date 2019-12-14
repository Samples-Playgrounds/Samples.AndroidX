using Android.Content;
using Android.OS;
using Android.Views;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.RecyclerView.Widget;
using Toggl.Core;
using Toggl.Core.Calendar;
using Toggl.Core.Helper;
using Toggl.Core.UI.Calendar;
using Toggl.Core.UI.Collections;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Adapters.Calendar
{
    public class CalendarAdapter : RecyclerView.Adapter
    {
        private const int anchorViewType = 1;
        private const int anchoredViewType = 2;
        private const int anchorCount = Constants.HoursPerDay;
        private readonly LayoutAnchorsCalculator layoutAnchorsCalculator;
        private CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly ISubject<CalendarItem> firstTapSubject = new Subject<CalendarItem>();
        private readonly ISubject<CalendarItem> calendarItemTappedSubject = new Subject<CalendarItem>();

        private readonly object updateLock = new object();
        private bool isUpdateRunning;
        private IList<CalendarItem> nextUpdate;
        private bool nextUpdateHasTwoColumns;

        private IReadOnlyList<Anchor> anchors;
        private IList<CalendarItem> items = new List<CalendarItem>();

        public IObservable<CalendarItem> CalendarItemTappedObservable
            => calendarItemTappedSubject.AsObservable();

        private CalendarItem? calendarItemInEditMode;

        public CalendarAdapter(Context context, ITimeService timeService, int screenWidth)
        {
            var layoutCalculator = new CalendarLayoutCalculator(timeService);
            layoutAnchorsCalculator = new LayoutAnchorsCalculator(context, screenWidth, anchorCount, layoutCalculator);

            anchors = Enumerable.Range(0, 24).Select(_ => new Anchor(56.DpToPixels(context), new AnchorData[0])).ToList();

            firstTapSubject.Subscribe(onFirstTap)
                .DisposedBy(disposeBag);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is AnchorViewHolder)
            {
                holder.ItemView.Tag = anchors[position];
            }

            if (holder is CalendarEntryViewHolder calendarEntryViewHolder)
            {
                var calendarItem = items[position - anchorCount];
                var firstAnchor = getCalendarItemFirstAnchor(calendarItem);

                var anchorData = firstAnchor.AnchoredData.FirstOrDefault(data => data.AdapterPosition == position);
                if (anchorData.AdapterPosition == 0) return;

                calendarEntryViewHolder.UpdateAnchoringInfo(anchorData);
                calendarEntryViewHolder.Item = calendarItem;
                calendarEntryViewHolder.SetIsInEditMode(calendarItemIsInEditMode(calendarItem));
            }
        }

        private Anchor getCalendarItemFirstAnchor(CalendarItem calendarItem)
        {
            var startingHour = calendarItem.StartTime.ToLocalTime().Hour;
            return anchors[startingHour];
        }

        private bool calendarItemIsInEditMode(CalendarItem calendarItem)
        {
            return calendarItem.Source == CalendarItemSource.TimeEntry
                   && calendarItem.Id == calendarItemInEditMode?.Id;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case anchorViewType:
                    return new AnchorViewHolder(new View(parent.Context));

                case anchoredViewType:
                    return new CalendarEntryViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CalendarEntryCell, parent, false))
                    {
                        TappedSubject = firstTapSubject
                    };

                default:
                    throw new InvalidOperationException($"Invalid view type {viewType}");
            }
        }

        public override int GetItemViewType(int position)
        {
            if (position < anchorCount)
                return anchorViewType;

            return anchoredViewType;
        }

        public override int ItemCount => anchorCount + items.Count;

        public bool NeedsToClearItemInEditMode()
        {
            if (calendarItemInEditMode.HasValue)
            {
                clearItemInEditMode();
                return true;
            }

            return false;
        }

        public void UpdateItems(ObservableGroupedOrderedCollection<CalendarItem> calendarItems, bool hasCalendarsLinked)
        {
            lock (updateLock)
            {
                var newItems = calendarItems.IsEmpty
                    ? new List<CalendarItem>()
                    : calendarItems[0].ToList();

                if (!isUpdateRunning)
                {
                    isUpdateRunning = true;
                    processUpdate(newItems, hasCalendarsLinked);
                }
                else
                {
                    nextUpdate = newItems;
                    nextUpdateHasTwoColumns = hasCalendarsLinked;
                }
            }
        }

        private void onFirstTap(CalendarItem calendarItem)
        {
            if (calendarItem.Source == CalendarItemSource.Calendar)
            {
                if (calendarItemInEditMode.HasValue)
                {
                    clearItemInEditMode();
                    return;
                }

                calendarItemTappedSubject.OnNext(calendarItem);
                return;
            }

            if (calendarItemInEditMode == null || touchesOtherItem(calendarItem))
            {
                updateCalendarItemInEditMode(calendarItem);
                return;
            }

            calendarItemTappedSubject.OnNext(calendarItem);
        }

        private bool touchesOtherItem(CalendarItem calendarItem)
            => calendarItem.Id != calendarItemInEditMode?.Id
               || calendarItem.Source != calendarItemInEditMode?.Source;

        private void clearItemInEditMode()
        {
            updateCalendarItemInEditMode(null);
        }

        private void updateCalendarItemInEditMode(CalendarItem? newCalendarItemInEditMode)
        {
            var oldPosition = findCalendarItemInEditModeAdapterPosition();
            calendarItemInEditMode = newCalendarItemInEditMode;

            if (oldPosition >= 0)
            {
                NotifyItemChanged(oldPosition);
            }

            var newPosition = findCalendarItemInEditModeAdapterPosition();
            if (newPosition >= 0)
            {
                NotifyItemChanged(newPosition);
            }
        }

        private int findCalendarItemInEditModeAdapterPosition()
        {
            if (items.Count <= 0 || !calendarItemInEditMode.HasValue)
                return -1;

            var calendarItemIndex = items.IndexOf(calendarItemInEditMode.Value);

            return calendarItemIndex >= 0
                ? calendarItemIndex + anchorCount
                : calendarItemIndex;
        }

        private void processUpdate(IList<CalendarItem> calendarItems, bool hasCalendarsLinked)
        {
            var handler = new Handler(Looper.MainLooper);
            new Thread(() =>
            {
                var oldItems = items;
                var oldAnchors = anchors;
                var newItems = calendarItems;
                var newAnchors = layoutAnchorsCalculator.CalculateAnchors(newItems, hasCalendarsLinked);

                var diffResult = DiffUtil.CalculateDiff(new AnchorDiffCallback(oldItems, oldAnchors, newItems, newAnchors));

                handler.Post(() => dispatchUpdates(newItems, newAnchors, diffResult));
            }).Start();
        }

        private void dispatchUpdates(IList<CalendarItem> newItems, List<Anchor> newAnchors, DiffUtil.DiffResult diffResult)
        {
            var calendarItemInEditModeBeforeUpdate = calendarItemInEditMode;

            updateCalendarItemInEditMode(null);

            items = newItems;
            anchors = newAnchors;
            diffResult.DispatchUpdatesTo(this);

            if (calendarItemInEditModeBeforeUpdate.HasValue)
            {
                fixCurrentCalendarItemInEditMode(calendarItemInEditModeBeforeUpdate.Value);
            }

            lock (updateLock)
            {
                if (nextUpdate != null)
                {
                    var updateTarget = nextUpdate;
                    nextUpdate = null;
                    processUpdate(updateTarget, nextUpdateHasTwoColumns);
                }
                else
                {
                    isUpdateRunning = false;
                }
            }
        }

        private void fixCurrentCalendarItemInEditMode(CalendarItem calendarItemInEditModeBeforeUpdate)
        {
            var newCalendarItemInEditMode = items.FirstOrDefault(calendarItem =>
                    calendarItem.Source == CalendarItemSource.TimeEntry
                    && calendarItem.Id == calendarItemInEditModeBeforeUpdate.Id);

            var defaultCalendarItem = default(CalendarItem);
            if (newCalendarItemInEditMode.Source != defaultCalendarItem.Source && newCalendarItemInEditMode.Id != defaultCalendarItem.Id)
            {
                updateCalendarItemInEditMode(newCalendarItemInEditMode);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            disposeBag?.Dispose();
        }

        private sealed class AnchorDiffCallback : DiffUtil.Callback
        {
            private readonly IList<CalendarItem> oldItems;
            private readonly IReadOnlyList<Anchor> oldAnchors;
            private readonly IList<CalendarItem> newItems;
            private readonly IReadOnlyList<Anchor> newAnchors;

            public AnchorDiffCallback(IList<CalendarItem> oldItems, IReadOnlyList<Anchor> oldAnchors, IList<CalendarItem> newItems, IReadOnlyList<Anchor> newAnchors)
            {
                this.oldItems = oldItems;
                this.oldAnchors = oldAnchors;
                this.newItems = newItems;
                this.newAnchors = newAnchors;
            }

            public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
            {
                if (oldItemPosition < anchorCount && newItemPosition < anchorCount)
                {
                    return areAnchorsContentsTheSame(oldItemPosition, newItemPosition);
                }

                if (oldItemPosition >= anchorCount && oldItems.Count > 0 && newItemPosition >= anchorCount && newItems.Count > 0)
                {
                    return areAnchoredItemsContentsTheSame(oldItems[oldItemPosition - anchorCount], newItems[newItemPosition - anchorCount]);
                }

                return false;
            }

            private bool areAnchorsContentsTheSame(int oldItemPosition, int newItemPosition)
            {
                return Enumerable.SequenceEqual(oldAnchors[oldItemPosition].AnchoredData, newAnchors[newItemPosition].AnchoredData);
            }

            private bool areAnchoredItemsContentsTheSame(CalendarItem oldItem, CalendarItem newItem)
            {
                return anchoredItemHashCode(oldItem) == anchoredItemHashCode(newItem);
            }

            private int anchoredItemHashCode(CalendarItem item)
                => HashCode.Combine(
                    item.Source,
                    item.Id,
                    item.StartTime,
                    item.Color,
                    item.Duration ?? TimeSpan.Zero,
                    item.Description,
                    item.IconKind);

            public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
            {
                if (oldItemPosition < anchorCount && newItemPosition < anchorCount)
                    return oldItemPosition == newItemPosition;

                if (oldItemPosition >= anchorCount && oldItems.Count > 0 && newItemPosition >= anchorCount && newItems.Count > 0)
                    return compareAnchoredItemIdentity(oldItems[oldItemPosition - anchorCount], newItems[newItemPosition - anchorCount]);

                return false;
            }

            private bool compareAnchoredItemIdentity(CalendarItem oldItem, CalendarItem newItem)
                => oldItem.Source == newItem.Source && oldItem.Id == newItem.Id;

            public override int NewListSize => anchorCount + newItems.Count;
            public override int OldListSize => anchorCount + oldItems.Count;
        }
    }
}
