using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.ViewHolders
{
    public sealed class WorkspaceSelectionViewHolder : BaseRecyclerViewHolder<SelectableWorkspaceViewModel>
    {
        public static WorkspaceSelectionViewHolder Create(View itemView)
            => new WorkspaceSelectionViewHolder(itemView);

        private ImageView checkedImage;
        private TextView workspaceName;

        public WorkspaceSelectionViewHolder(View itemView)
            : base(itemView)
        {
        }

        public WorkspaceSelectionViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            checkedImage = ItemView.FindViewById<ImageView>(Resource.Id.SettingsWorkspaceCellCheckedImageView);
            workspaceName = ItemView.FindViewById<TextView>(Resource.Id.SettingsWorkspaceCellWorkspaceNameTextView);
        }

        protected override void UpdateView()
        {
            workspaceName.Text = Item.WorkspaceName;
            checkedImage.Visibility = Item.Selected.ToVisibility(useGone: false);
        }
    }
}
