using Android.Runtime;
using Android.Views;
using System;
using AndroidX.RecyclerView.Widget;

namespace Toggl.Droid.ViewHolders
{
    public class AnchorViewHolder : RecyclerView.ViewHolder
    {
        public AnchorViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnchorViewHolder(View itemView) : base(itemView)
        {
        }
    }
}
