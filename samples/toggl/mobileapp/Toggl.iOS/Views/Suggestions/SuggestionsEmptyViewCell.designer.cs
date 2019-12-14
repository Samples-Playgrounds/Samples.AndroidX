// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Views
{
    [Register ("SuggestionsEmptyViewCell")]
    partial class SuggestionsEmptyViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint DescriptionWidth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ProjectView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ProjectWidth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint TaskWidth { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DescriptionWidth != null) {
                DescriptionWidth.Dispose ();
                DescriptionWidth = null;
            }

            if (ProjectView != null) {
                ProjectView.Dispose ();
                ProjectView = null;
            }

            if (ProjectWidth != null) {
                ProjectWidth.Dispose ();
                ProjectWidth = null;
            }

            if (TaskWidth != null) {
                TaskWidth.Dispose ();
                TaskWidth = null;
            }
        }
    }
}
