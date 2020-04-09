using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.ViewHolders.Country
{
    public class CountrySelectionViewHolder : BaseRecyclerViewHolder<SelectableCountryViewModel>
    {
        private ImageView selectedImageView;
        private TextView nameTextView;

        public static CountrySelectionViewHolder Create(View itemView)
            => new CountrySelectionViewHolder(itemView);

        public CountrySelectionViewHolder(View itemView) : base(itemView)
        {
        }

        public CountrySelectionViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            selectedImageView = ItemView.FindViewById<ImageView>(Resource.Id.SelectedImageView);
            nameTextView = ItemView.FindViewById<TextView>(Resource.Id.NameTextView);
        }

        protected override void UpdateView()
        {
            nameTextView.Text = Item.Country.Name;
            selectedImageView.Visibility = Item.Selected ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}