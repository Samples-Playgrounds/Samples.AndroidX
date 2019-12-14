using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class EditingHourSupplementaryView : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(EditingHourSupplementaryView));
        public static readonly UINib Nib;

        static EditingHourSupplementaryView()
        {
            Nib = UINib.FromName(nameof(EditingHourSupplementaryView), NSBundle.MainBundle);
        }

        protected EditingHourSupplementaryView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void SetLabel(string label)
        {
            HourLabel.Text = label;
        }
    }
}

