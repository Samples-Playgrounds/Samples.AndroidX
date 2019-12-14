using Foundation;
using System;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.ReportsCalendar;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    public sealed partial class ReportsCalendarViewCell : ReactiveCollectionViewCell<ReportsCalendarDayViewModel>
    {
        private const int cornerRadius = 16;

        public static readonly NSString Key = new NSString(nameof(ReportsCalendarViewCell));
        public static readonly UINib Nib;

        private ReportsDateRangeParameter dateRange;

        static ReportsCalendarViewCell()
        {
            Nib = UINib.FromName(nameof(ReportsCalendarViewCell), NSBundle.MainBundle);
        }

        public ReportsCalendarViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            prepareViews();
        }

        private void prepareViews()
        {
            //Background view
            BackgroundView.CornerRadius = cornerRadius;

            //Today background indicator
            TodayBackgroundView.CornerRadius = cornerRadius;
            TodayBackgroundView.RoundLeft = true;
            TodayBackgroundView.RoundRight = true;
            TodayBackgroundView.BackgroundColor = ColorAssets.CustomGray2;
        }

        private readonly UIColor otherMonthColor = ColorAssets.Text4;
        private readonly UIColor thisMonthColor = ColorAssets.Text2;
        private readonly UIColor selectedColor = ColorAssets.InverseText;
        private readonly UIColor todayColor = ColorAssets.InverseText;

        protected override void UpdateView()
        {
            Text.Text = Item.Day.ToString();
            Text.TextColor = selectTextColor();

            BackgroundView.BackgroundColor =
                Item.IsSelected(dateRange)
                    ? ColorAssets.CustomGray
                    : Colors.Common.Transparent.ToNativeColor();

            BackgroundView.RoundLeft = Item.IsStartOfSelectedPeriod(dateRange);
            BackgroundView.RoundRight = Item.IsEndOfSelectedPeriod(dateRange);

            TodayBackgroundView.Hidden = !Item.IsToday;
        }

        public void UpdateDateRange(ReportsDateRangeParameter dateRange)
        {
            this.dateRange = dateRange;
        }

        private UIColor selectTextColor()
        {
            if (Item.IsToday)
            {
                return Item.IsSelected(dateRange)
                    ? selectedColor
                    : todayColor;
            }

            if (Item.IsSelected(dateRange))
            {
                return selectedColor;
            }

            if (Item.IsInCurrentMonth)
            {
                return thisMonthColor;
            }

            return otherMonthColor;
        }
    }
}
