using Foundation;
using System;
using Toggl.Core.Extensions;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public sealed partial class UpcomingEventsOptionCell : BaseTableViewCell<SelectableCalendarNotificationsOptionViewModel>
    {
        public static readonly string Identifier = nameof(UpcomingEventsOptionCell);
        public static readonly NSString Key = new NSString(nameof(UpcomingEventsOptionCell));
        public static readonly UINib Nib;

        static UpcomingEventsOptionCell()
        {
            Nib = UINib.FromName(nameof(UpcomingEventsOptionCell), NSBundle.MainBundle);
        }

        public UpcomingEventsOptionCell(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SelectionStyle = UITableViewCellSelectionStyle.None;
            this.InsertSeparator();
        }

        protected override void UpdateView()
        {
            SelectedImageView.Hidden = !Item.Selected;
            TitleLabel.Text = Item.Option.Title();
        }
    }
}
