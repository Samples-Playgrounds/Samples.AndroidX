using Foundation;
using System;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class UserCalendarListHeaderViewCell : BaseTableHeaderFooterView<UserCalendarSourceViewModel>
    {
        public static readonly string Identifier = nameof(UserCalendarListHeaderViewCell);
        public static readonly NSString Key = new NSString(nameof(UserCalendarListHeaderViewCell));
        public static readonly UINib Nib;

        static UserCalendarListHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(UserCalendarListHeaderViewCell), NSBundle.MainBundle);
        }

        protected UserCalendarListHeaderViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Name;
        }
    }
}
