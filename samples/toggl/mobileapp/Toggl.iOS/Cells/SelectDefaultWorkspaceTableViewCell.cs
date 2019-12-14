using Foundation;
using System;
using Toggl.Core.UI.ViewModels;
using UIKit;

namespace Toggl.iOS.Cells
{
    public sealed partial class SelectDefaultWorkspaceTableViewCell : BaseTableViewCell<SelectableWorkspaceViewModel>
    {
        public static readonly UINib Nib;
        public static readonly string Identifier = "SelectDefaultWorkspaceTableViewCell";

        static SelectDefaultWorkspaceTableViewCell()
        {
            Nib = UINib.FromName(nameof(SelectDefaultWorkspaceTableViewCell), NSBundle.MainBundle);
        }

        protected SelectDefaultWorkspaceTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            Label.Text = Item.WorkspaceName;
        }
    }
}

