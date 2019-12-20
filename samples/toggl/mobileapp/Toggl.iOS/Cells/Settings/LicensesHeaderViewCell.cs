using Foundation;
using System;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public sealed partial class LicensesHeaderViewCell : BaseTableHeaderFooterView<License>
    {
        public static readonly string Identifier = nameof(LicensesHeaderViewCell);
        public static readonly NSString Key = new NSString(nameof(LicensesHeaderViewCell));
        public static readonly UINib Nib;

        static LicensesHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(LicensesHeaderViewCell), NSBundle.MainBundle);
        }

        public LicensesHeaderViewCell(IntPtr handle) : base(handle)
        {
        }

        protected override void UpdateView()
        {
            HeaderLabel.Text = Item.LibraryName;
        }


        public static UIView HeaderConfiguration(UITableView tableView, nint section, License header)
        {
            var headerView = tableView.DequeueReusableHeaderFooterView(LicensesHeaderViewCell.Identifier) as LicensesHeaderViewCell;
            headerView.Item = header;
            return headerView;
        }
    }
}
