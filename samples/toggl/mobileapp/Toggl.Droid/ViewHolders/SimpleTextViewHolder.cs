using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace Toggl.Droid.ViewHolders
{
    public sealed class SimpleTextViewHolder<T> : BaseRecyclerViewHolder<T>
    {
        private TextView textView;
        private readonly int textViewResourceId;
        private readonly Func<T, string> transformFunction;

        public SimpleTextViewHolder(View itemView, int textViewResourceId, Func<T, string> transformFunction)
            : base(itemView)
        {
            this.textViewResourceId = textViewResourceId;
            this.transformFunction = transformFunction;
        }

        public SimpleTextViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            textView = ItemView.FindViewById<TextView>(textViewResourceId);
        }

        protected override void UpdateView()
        {
            textView.Text = transformFunction(Item);
        }
    }
}
