using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders
{
    public class LicenseViewHolder : BaseRecyclerViewHolder<License>
    {
        private TextView license;
        private TextView libraryName;

        public static LicenseViewHolder Create(View itemView)
            => new LicenseViewHolder(itemView);

        public LicenseViewHolder(View itemView)
            : base(itemView)
        {
        }

        public LicenseViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            license = ItemView.FindViewById<TextView>(Resource.Id.License);
            libraryName = ItemView.FindViewById<TextView>(Resource.Id.LibraryName);
        }

        protected override void UpdateView()
        {
            license.Text = Item.Text;
            libraryName.Text = Item.LibraryName;
        }
    }
}
