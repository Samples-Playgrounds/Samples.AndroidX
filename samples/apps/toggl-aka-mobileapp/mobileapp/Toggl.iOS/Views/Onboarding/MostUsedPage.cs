using Foundation;
using ObjCRuntime;
using System;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS
{
    public sealed partial class MostUsedPage : UIView
    {
        public MostUsedPage(IntPtr handle) : base(handle)
        {
        }

        public static MostUsedPage Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(MostUsedPage), null, null);
            return Runtime.GetNSObject<MostUsedPage>(arr.ValueAt(0));
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
