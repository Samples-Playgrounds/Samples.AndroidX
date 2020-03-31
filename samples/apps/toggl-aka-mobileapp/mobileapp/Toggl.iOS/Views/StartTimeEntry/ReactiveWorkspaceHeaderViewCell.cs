using Foundation;
using System;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.StartTimeEntry
{
    public sealed partial class ReactiveWorkspaceHeaderViewCell : UITableViewHeaderFooterView
    {
        public static readonly NSString Key = new NSString(nameof(ReactiveWorkspaceHeaderViewCell));
        public static readonly UINib Nib;

        private string workspaceName = "";
        public string WorkspaceName
        {
            get => workspaceName;
            set
            {
                workspaceName = value;
                updateView();
            }
        }

        static ReactiveWorkspaceHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(ReactiveWorkspaceHeaderViewCell), NSBundle.MainBundle);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ContentView.InsertSeparator();
        }

        protected ReactiveWorkspaceHeaderViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        private void updateView()
        {
            WorkspaceNameLabel.Text = WorkspaceName;
        }
    }
}

