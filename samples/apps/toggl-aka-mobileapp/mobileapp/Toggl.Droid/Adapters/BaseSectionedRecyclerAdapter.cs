using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AndroidX.RecyclerView.Widget;
using Toggl.Core.UI.Collections;
using Toggl.Droid.ViewHolders;
using Toggl.Shared;

namespace Toggl.Droid.Adapters
{
    public abstract class BaseSectionedRecyclerAdapter<TSection, TItem> : RecyclerView.Adapter
        where TItem : IEquatable<TItem>
        where TSection : IEquatable<TSection>
    {
        public const int HeaderViewType = 0;
        public const int ItemViewType = 1;

        private readonly object updateLock = new object();
        private bool isUpdateRunning;

        private IImmutableList<Either<TSection, TItem>> currentItems = ImmutableList<Either<TSection, TItem>>.Empty;
        private IImmutableList<Either<TSection, TItem>> nextUpdate;

        private readonly Subject<TItem> itemTapSubject = new Subject<TItem>();
        private readonly Subject<TSection> headerTapSubject = new Subject<TSection>();

        private readonly Subject<Unit> itemsUpdateCompletedSubject = new Subject<Unit>();
        public IObservable<Unit> ItemsUpdateCompleted { get; }

        private IImmutableList<SectionModel<TSection, TItem>> items;
        public IImmutableList<SectionModel<TSection, TItem>> Items
        {
            get => items;
            set => setItems(value ?? ImmutableList<SectionModel<TSection, TItem>>.Empty);
        }

        public IObservable<TItem> ItemTapObservable => itemTapSubject.AsObservable();

        protected virtual HashSet<int> ItemViewTypes { get; } = new HashSet<int> { ItemViewType };
        protected virtual HashSet<int> HeaderViewTypes { get; } = new HashSet<int> { HeaderViewType };

        public override int ItemCount => currentItems.Count;

        protected BaseSectionedRecyclerAdapter()
        {
            HasStableIds = false;
            ItemsUpdateCompleted = itemsUpdateCompletedSubject.AsObservable();
        }

        protected BaseSectionedRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override sealed int GetItemViewType(int position)
        {
            var item = currentItems[position];
            var isHeader = item.IsLeft;
            if (isHeader)
            {
                var headerViewType = SelectHeaderViewType(item.Left);
                if (!HeaderViewTypes.Contains(headerViewType))
                    throw new InvalidOperationException("A header view type not included in the HeaderViewTypes property was returned");

                return headerViewType;
            }

            var itemViewType = SelectItemViewType(item.Right);
            if (!ItemViewTypes.Contains(itemViewType))
                throw new InvalidOperationException("An item view type not included in the ItemViewTypes property was returned");

            return itemViewType;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            if (ItemViewTypes.Contains(viewType))
            {
                var itemViewHolder = CreateItemViewHolder(inflater, parent, viewType);
                itemViewHolder.TappedSubject = itemTapSubject;
                return itemViewHolder;
            }

            var headerViewHolder = CreateHeaderViewHolder(inflater, parent, viewType); ;
            headerViewHolder.TappedSubject = headerTapSubject;
            return headerViewHolder;
        }

        protected virtual int SelectItemViewType(TItem headerItem)
            => ItemViewType;

        protected virtual int SelectHeaderViewType(TSection headerItem)
            => HeaderViewType;

        protected abstract BaseRecyclerViewHolder<TSection> CreateHeaderViewHolder(LayoutInflater inflater, ViewGroup parent, int viewType);

        protected abstract BaseRecyclerViewHolder<TItem> CreateItemViewHolder(LayoutInflater inflater, ViewGroup parent, int viewType);

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (holder)
            {
                case BaseRecyclerViewHolder<TItem> itemViewHolder:
                    itemViewHolder.Item = currentItems[position].Right;
                    break;
                case BaseRecyclerViewHolder<TSection> sectionViewHolder:
                    sectionViewHolder.Item = currentItems[position].Left;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected view holder of type {holder.GetType().Name}");
            }
        }

        private void setItems(IImmutableList<SectionModel<TSection, TItem>> newItems)
        {
            lock (updateLock)
            {
                var flatNewItems = flattenItems(newItems);
                if (!isUpdateRunning)
                {
                    isUpdateRunning = true;
                    processUpdate(flatNewItems);
                }
                else
                {
                    nextUpdate = flatNewItems;
                }
            }
        }

        private IImmutableList<Either<TSection, TItem>> flattenItems(IImmutableList<SectionModel<TSection, TItem>> newItems)
        {
            var flattenedItems = new List<Either<TSection, TItem>>();

            if (newItems == null)
                return flattenedItems.ToImmutableList();

            var hasMultipleSections = newItems.Count > 1;

            foreach (var section in newItems)
            {
                var shouldIncludeHeader = hasMultipleSections && !(section.Header?.Equals(default(TSection)) ?? true);
                if (shouldIncludeHeader)
                    flattenedItems.Add(Either<TSection, TItem>.WithLeft(section.Header));

                foreach (var item in section.Items)
                {
                    flattenedItems.Add(Either<TSection, TItem>.WithRight(item));
                }
            }

            return flattenedItems.ToImmutableList();
        }

        private void processUpdate(IImmutableList<Either<TSection, TItem>> newItems)
        {
            var oldItems = currentItems;
            var handler = new Handler();
            Task.Run(() =>
            {
                var diffResult = DiffUtil.CalculateDiff(new BaseSectionedDiffCallBack(oldItems, newItems));
                handler.Post(() =>
                {
                    dispatchUpdates(newItems, diffResult);
                });
            });
        }

        private void dispatchUpdates(IImmutableList<Either<TSection, TItem>> newItems, DiffUtil.DiffResult diffResult)
        {
            currentItems = newItems;
            diffResult.DispatchUpdatesTo(this);
            itemsUpdateCompletedSubject.OnNext(Unit.Default);
            lock (updateLock)
            {
                if (nextUpdate != null)
                {
                    processUpdate(nextUpdate);
                    nextUpdate = null;
                }
                else
                {
                    isUpdateRunning = false;
                }
            }
        }

        private sealed class BaseSectionedDiffCallBack : DiffUtil.Callback
        {
            private IImmutableList<Either<TSection, TItem>> oldItems;
            private IImmutableList<Either<TSection, TItem>> newItems;

            public BaseSectionedDiffCallBack(IImmutableList<Either<TSection, TItem>> oldItems, IImmutableList<Either<TSection, TItem>> newItems)
            {
                this.oldItems = oldItems;
                this.newItems = newItems;
            }

            public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
            {
                var oldItem = oldItems[oldItemPosition];
                var newItem = newItems[newItemPosition];

                if (oldItem.IsLeft && newItem.IsLeft)
                {
                    if (oldItem.Left == null || newItem.Left == null)
                        return false;

                    return oldItem.Left.Equals(newItem.Left);
                }

                if (oldItem.IsRight && newItem.IsRight)
                {
                    if (oldItem.Right == null || newItem.Right == null)
                        return false;

                    return oldItem.Right.Equals(newItem.Right);
                }

                return false;
            }

            public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
            {
                var oldItem = oldItems[oldItemPosition];
                var newItem = newItems[newItemPosition];

                if (oldItem.IsLeft && newItem.IsLeft)
                {
                    if (oldItem.Left == null || newItem.Left == null)
                        return false;

                    return oldItem.Left.Equals(newItem.Left);
                }

                if (oldItem.IsRight && newItem.IsRight)
                {
                    if (oldItem.Right == null || newItem.Right == null)
                        return false;

                    return oldItem.Right.Equals(newItem.Right);
                }

                return false;
            }

            public override int NewListSize => newItems.Count;
            public override int OldListSize => oldItems.Count;
        }
    }
}
