using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class HourSupplementaryView : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(HourSupplementaryView));
        public static readonly UINib Nib;

        static HourSupplementaryView()
        {
            Nib = UINib.FromName(nameof(HourSupplementaryView), NSBundle.MainBundle);
        }

        protected HourSupplementaryView(IntPtr handle) : base(handle)
        {
        }

        public void SetLabel(string label)
        {
            HourLabel.Text = label;
        }
    }
}
