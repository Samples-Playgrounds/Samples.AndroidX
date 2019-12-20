using Foundation;
using System;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views
{
    public partial class TimeEntriesLogHeaderView : BaseTableHeaderFooterView<DaySummaryViewModel>
    {
        public static readonly string Identifier = "timeEntryLogHeaderCell";

        public static readonly NSString Key = new NSString(nameof(TimeEntriesLogHeaderView));
        public static readonly UINib Nib;

        static TimeEntriesLogHeaderView()
        {
            Nib = UINib.FromName(nameof(TimeEntriesLogHeaderView), NSBundle.MainBundle);
        }

        protected TimeEntriesLogHeaderView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ContentView.InsertSeparator();

            ContentView.BackgroundColor = ColorAssets.TableBackground;
            DateLabel.TextColor = ColorAssets.Text2;
            DurationLabel.TextColor = ColorAssets.Text2;

            IsAccessibilityElement = true;
            AccessibilityTraits = UIAccessibilityTrait.Header;
            DurationLabel.Font = DurationLabel.Font.GetMonospacedDigitFont();
        }

        protected override void UpdateView()
        {
            DateLabel.Text = Item.Title;
            DurationLabel.Text = Item.TotalTrackedTime;
            updateAccessibilityProperties();
        }

        private void updateAccessibilityProperties()
        {
            AccessibilityLabel = $"{Item.Title}, {Resources.TrackedTime}: {Item.TotalTrackedTime}";
        }
    }
}
