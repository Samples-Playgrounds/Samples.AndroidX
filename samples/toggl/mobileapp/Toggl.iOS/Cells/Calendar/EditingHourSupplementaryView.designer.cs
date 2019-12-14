// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;

namespace Toggl.iOS.Cells.Calendar
{
    [Register("EditingHourSupplementaryView")]
    partial class EditingHourSupplementaryView
    {
        [Outlet]
        UIKit.UILabel HourLabel { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (HourLabel != null)
            {
                HourLabel.Dispose();
                HourLabel = null;
            }
        }
    }
}

