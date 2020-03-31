using Foundation;
using System;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    public sealed partial class ReportsCalendarQuickSelectViewCell : ReactiveCollectionViewCell<ReportsCalendarBaseQuickSelectShortcut>
    {
        public static readonly NSString Key = new NSString(nameof(ReportsCalendarQuickSelectViewCell));
        public static readonly UINib Nib;

        private ReportsDateRangeParameter currentDateRange;

        static ReportsCalendarQuickSelectViewCell()
        {
            Nib = UINib.FromName(nameof(ReportsCalendarQuickSelectViewCell), NSBundle.MainBundle);
        }

        public ReportsCalendarQuickSelectViewCell(IntPtr handle)
            : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TitleLabel.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Medium);
        }

        public void UpdateSelectedDateRange(ReportsDateRangeParameter dateRange)
        {
            currentDateRange = dateRange;
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;

            ContentView.BackgroundColor = Item.IsSelected(currentDateRange)
                ? ColorAssets.CustomGray
                : ColorAssets.CustomGray5;

            TitleLabel.TextColor = Item.IsSelected(currentDateRange)
                ? ColorAssets.InverseText
                : ColorAssets.Text2;
        }
    }
}
