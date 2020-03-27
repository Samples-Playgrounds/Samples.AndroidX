// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    [Register("SiriShortcutsViewController")]
    partial class SiriShortcutsViewController
    {
        [Outlet]
        UIKit.UILabel DescriptionLabel { get; set; }

        [Outlet]
        UIKit.UIView HeaderView { get; set; }

        [Outlet]
        UIKit.UITableView TableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }

            if (HeaderView != null) {
                HeaderView.Dispose ();
                HeaderView = null;
            }

            if (DescriptionLabel != null) {
                DescriptionLabel.Dispose ();
                DescriptionLabel = null;
            }
        }
    }
}
