// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.iOS.ViewControllers.Settings
{
    [Register ("CalendarSettingsViewController")]
    partial class CalendarSettingsViewController
    {
        [Outlet]
        UIKit.UITableView UserCalendarsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (UserCalendarsTableView != null) {
                UserCalendarsTableView.Dispose ();
                UserCalendarsTableView = null;
            }
        }
    }
}
