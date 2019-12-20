using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class CurrentTimeSupplementaryView : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(CurrentTimeSupplementaryView));
        public static readonly UINib Nib;

        static CurrentTimeSupplementaryView()
        {
            Nib = UINib.FromName(nameof(CurrentTimeSupplementaryView), NSBundle.MainBundle);
        }

        protected CurrentTimeSupplementaryView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            UserInteractionEnabled = false;
        }
    }
}
