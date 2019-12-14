using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.ViewHolders
{
    public sealed class SelectDefaultWorkspaceViewHolder : BaseRecyclerViewHolder<SelectableWorkspaceViewModel>
    {
        public static SelectDefaultWorkspaceViewHolder Create(View itemView)
            => new SelectDefaultWorkspaceViewHolder(itemView);

        private TextView workspaceNameTextView;
        private RadioButton radioButton;

        public SelectDefaultWorkspaceViewHolder(View itemView) : base(itemView)
        {
        }

        public SelectDefaultWorkspaceViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            radioButton = ItemView.FindViewById<RadioButton>(Resource.Id.SelectDefaultWorkspaceFragmentCellRadioButton);
            workspaceNameTextView = ItemView.FindViewById<TextView>(Resource.Id.SelectDefaultWorkspaceFragmentCellTextView);
        }

        protected override void UpdateView()
        {
            workspaceNameTextView.Text = Item.WorkspaceName;
        }
    }
}
