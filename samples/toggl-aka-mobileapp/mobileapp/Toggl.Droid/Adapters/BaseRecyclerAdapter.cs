using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AndroidX.RecyclerView.Widget;
using Toggl.Core.UI.Interfaces;
using Toggl.Droid.Adapters.DiffingStrategies;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;
using Handler = Android.OS.Handler;

namespace Toggl.Droid.Adapters
{
    public abstract class BaseRecyclerAdapter<T> : RecyclerView.Adapter
        where T : IEquatable<T>
    {
        private readonly IDiffingStrategy<T> diffingStrategy;

        public IObservable<T> ItemTapObservable => itemTapSubject.AsObservable();

        private Subject<T> itemTapSubject = new Subject<T>();

        private IImmutableList<T> items = ImmutableList<T>.Empty;

        private IImmutableList<T> nextUpdate;

        private bool isUpdateRunning;

        private readonly object updateLock = new object();

        public virtual IImmutableList<T> Items
        {
            get => items;
            set => SetItems(value ?? ImmutableList<T>.Empty);
        }

        protected BaseRecyclerAdapter(IDiffingStrategy<T> diffingStrategy = null)
        {
            this.diffingStrategy = normalizeDiffingStrategy(diffingStrategy);

            HasStableIds = this.diffingStrategy.HasStableIds;
        }

        private IDiffingStrategy<T> normalizeDiffingStrategy(IDiffingStrategy<T> diffingStrategy)
        {
            if (diffingStrategy != null)
                return diffingStrategy;

            if (typeof(T).ImplementsOrDerivesFrom<IDiffableByIdentifier<T>>())
                return new IdentifierEqualityDiffingStrategy<T>();

            return new EquatableDiffingStrategy<T>();
        }

        protected BaseRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var viewHolder = CreateViewHolder(parent, inflater, viewType);
            viewHolder.TappedSubject = itemTapSubject;
            return viewHolder;
        }

        protected abstract BaseRecyclerViewHolder<T> CreateViewHolder(ViewGroup parent, LayoutInflater inflater,
            int viewType);

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = GetItem(position);
            ((BaseRecyclerViewHolder<T>)holder).Item = item;
        }

        public override int ItemCount => items.Count;

        public override long GetItemId(int position)
        {
            var item = items[position];
            return diffingStrategy.GetItemId(item);
        }

        public virtual T GetItem(int viewPosition)
            => items[viewPosition];

        protected virtual void SetItems(IImmutableList<T> newItems)
        {
            lock (updateLock)
            {
                if (!isUpdateRunning)
                {
                    isUpdateRunning = true;
                    processUpdate(newItems);
                }
                else
                {
                    nextUpdate = newItems;
                }
            }
        }

        private void processUpdate(IImmutableList<T> newItems)
        {
            var oldItems = items;
            var handler = new Handler(Looper.MainLooper);
            Task.Run(() =>
            {
                var diffResult = DiffUtil.CalculateDiff(new BaseDiffCallBack(oldItems, newItems, diffingStrategy));
                handler.Post(() => dispatchUpdates(newItems, diffResult));
            });
        }

        private void dispatchUpdates(IImmutableList<T> newItems, DiffUtil.DiffResult diffResult)
        {
            items = newItems;
            diffResult.DispatchUpdatesTo(this);
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

        private sealed class BaseDiffCallBack : DiffUtil.Callback
        {
            private IImmutableList<T> oldItems;
            private IImmutableList<T> newItems;
            private IDiffingStrategy<T> diffingStrategy;

            public BaseDiffCallBack(IImmutableList<T> oldItems, IImmutableList<T> newItems, IDiffingStrategy<T> diffingStrategy)
            {
                this.oldItems = oldItems;
                this.newItems = newItems;
                this.diffingStrategy = diffingStrategy;
            }

            public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
            {
                var oldItem = oldItems[oldItemPosition];
                var newItem = newItems[newItemPosition];

                return diffingStrategy.AreContentsTheSame(oldItem, newItem);
            }

            public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
            {
                var oldItem = oldItems[oldItemPosition];
                var newItem = newItems[newItemPosition];

                return diffingStrategy.AreItemsTheSame(oldItem, newItem);
            }

            public override int NewListSize => newItems.Count;
            public override int OldListSize => oldItems.Count;
        }
    }
}
