using Foundation;
using System;
using Toggl.Core.Models;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class SiriShortcutReportPeriodCell : BaseTableViewCell<SelectableReportPeriodViewModel>
    {
        public static readonly string Identifier = nameof(SiriShortcutReportPeriodCell);
        public static readonly UINib Nib;

        static SiriShortcutReportPeriodCell()
        {
            Nib = UINib.FromName(nameof(SiriShortcutReportPeriodCell), NSBundle.MainBundle);
        }

        public SiriShortcutReportPeriodCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TextLabel.Text = string.Empty;
            SelectedImageView.Hidden = true;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            TextLabel.Text = Item.ReportPeriod.ToHumanReadableString();
            SelectedImageView.Hidden = !Item.Selected;
        }
    }
}

