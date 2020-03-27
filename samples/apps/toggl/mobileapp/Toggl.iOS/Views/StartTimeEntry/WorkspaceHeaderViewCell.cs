using Foundation;
using System;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Views.Interfaces;
using UIKit;

namespace Toggl.iOS.Views
{
    public sealed partial class WorkspaceHeaderViewCell : BaseTableHeaderFooterView<string>, IHeaderViewCellWithHideableTopSeparator
    {
        public static readonly string Identifier = nameof(WorkspaceHeaderViewCell);
        public static readonly UINib Nib;

        public bool TopSeparatorHidden
        {
            get => TopSeparator.Hidden;
            set => TopSeparator.Hidden = value;
        }

        static WorkspaceHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(WorkspaceHeaderViewCell), NSBundle.MainBundle);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.InsertSeparator();
        }

        public WorkspaceHeaderViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            WorkspaceLabel.Text = Item;
        }
    }
}
