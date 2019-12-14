using Foundation;
using ObjCRuntime;
using System;
using System.Reactive;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS
{
    public sealed partial class CalendarSettingsTableViewHeader : UIView
    {
        public IObservable<Unit> EnableCalendarAccessTapped { get; private set; }

        public CalendarSettingsTableViewHeader(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TextLabel.Text = Resources.AllowCalendarAccess;
            DescriptionLabel.Text = Resources.CalendarAccessExplanation;
            EnableCalendarAccessTapped = EnableCalendarAccessView.Rx().Tap();

            EnableCalendarAccessView.InsertSeparator();
        }

        public static CalendarSettingsTableViewHeader Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(CalendarSettingsTableViewHeader), null, null);
            return Runtime.GetNSObject<CalendarSettingsTableViewHeader>(arr.ValueAt(0));
        }

        public void SetCalendarPermissionStatus(bool enabled)
        {
            CalendarPermissionStatusLabel.Text = enabled
                ? Resources.On
                : Resources.Off;
        }
    }
}
