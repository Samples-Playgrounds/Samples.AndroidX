using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.Adapters
{
    public sealed class SelectClientRecyclerAdapter : BaseRecyclerAdapter<SelectableClientBaseViewModel>
    {
        private const int selectableClientViewType = 1;
        private const int selectableClientCreationViewType = 2;

        public SelectClientRecyclerAdapter()
        {
        }

        public SelectClientRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override int GetItemViewType(int position)
        {
            var item = GetItem(position);
            switch (item)
            {
                case SelectableClientViewModel _:
                    return selectableClientViewType;
                case SelectableClientCreationViewModel _:
                    return selectableClientCreationViewType;
                default:
                    throw new Exception("Invalid item type");
            }
        }

        protected override BaseRecyclerViewHolder<SelectableClientBaseViewModel> CreateViewHolder(ViewGroup parent, LayoutInflater inflater, int viewType)
        {
            switch (viewType)
            {
                case selectableClientViewType:
                    var inflatedView = inflater.Inflate(Resource.Layout.SelectClientActivityCell, parent, false);
                    return new ClientSelectionViewHolder(inflatedView);
                case selectableClientCreationViewType:
                    var inflatedCreationView = inflater.Inflate(Resource.Layout.AbcCreateEntityCell, parent, false);
                    return new ClientCreationSelectionViewHolder(inflatedCreationView);
                default:
                    throw new Exception("Unsupported view type");
            }
        }
    }
}
