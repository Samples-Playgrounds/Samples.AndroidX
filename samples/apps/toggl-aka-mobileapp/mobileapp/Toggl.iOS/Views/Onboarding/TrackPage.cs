using Foundation;
using ObjCRuntime;
using System;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS
{
    public sealed partial class TrackPage : UIView
    {
        public TrackPage(IntPtr handle) : base(handle)
        {
        }

        public static TrackPage Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(TrackPage), null, null);
            return Runtime.GetNSObject<TrackPage>(arr.ValueAt(0));
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            FirstCell.MockSuggestion();
            SecondCell.MockSuggestion();
            ThirdCell.MockSuggestion();
        }
    }
}
