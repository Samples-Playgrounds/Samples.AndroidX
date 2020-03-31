// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.iOS.ViewControllers
{
    [Register ("SettingsViewController")]
    partial class SettingsViewController
    {
        [Outlet]
        UIKit.UIView SendFeedbackSuccessView { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FeedbackToastTextLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FeedbackToastTitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FeedbackToastTextLabel != null) {
                FeedbackToastTextLabel.Dispose ();
                FeedbackToastTextLabel = null;
            }

            if (FeedbackToastTitleLabel != null) {
                FeedbackToastTitleLabel.Dispose ();
                FeedbackToastTitleLabel = null;
            }
        }
    }
}