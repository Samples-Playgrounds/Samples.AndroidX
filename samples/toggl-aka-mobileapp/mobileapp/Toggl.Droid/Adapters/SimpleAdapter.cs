using Android.Runtime;
using Android.Views;
using System;
using Toggl.Droid.Adapters.DiffingStrategies;
using Toggl.Droid.ViewHolders;
using Toggl.Shared;

namespace Toggl.Droid.Adapters
{
    public sealed class SimpleAdapter<T> : BaseRecyclerAdapter<T>
        where T : IEquatable<T>
    {
        private readonly int layoutId;
        private readonly Func<View, BaseRecyclerViewHolder<T>> createViewHolder;

        public SimpleAdapter(int layoutId, Func<View, BaseRecyclerViewHolder<T>> createViewHolder, IDiffingStrategy<T> diffingStrategy = null)
            : base(diffingStrategy)
        {
            Ensure.Argument.IsNotNull(createViewHolder, nameof(createViewHolder));

            this.layoutId = layoutId;
            this.createViewHolder = createViewHolder;
        }

        public SimpleAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override BaseRecyclerViewHolder<T> CreateViewHolder(ViewGroup parent, LayoutInflater inflater, int viewType)
        {
            var itemView = inflater.Inflate(layoutId, parent, false);
            return createViewHolder(itemView);
        }
    }
}
