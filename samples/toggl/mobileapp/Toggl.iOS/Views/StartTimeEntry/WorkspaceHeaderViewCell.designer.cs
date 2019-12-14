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
    [Register ("WorkspaceHeaderViewCell")]
    partial class WorkspaceHeaderViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TopSeparator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel WorkspaceLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TopSeparator != null) {
                TopSeparator.Dispose ();
                TopSeparator = null;
            }

            if (WorkspaceLabel != null) {
                WorkspaceLabel.Dispose ();
                WorkspaceLabel = null;
            }
        }
    }
}
