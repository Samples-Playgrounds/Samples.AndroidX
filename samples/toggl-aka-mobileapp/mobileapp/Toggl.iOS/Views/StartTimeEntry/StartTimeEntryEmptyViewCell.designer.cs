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
    [Register ("StartTimeEntryEmptyViewCell")]
    partial class StartTimeEntryEmptyViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DescriptionLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DescriptionLabel != null) {
                DescriptionLabel.Dispose ();
                DescriptionLabel = null;
            }
        }
    }
}
