// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Views.CountrySelection
{
    [Register ("CountryViewCell")]
    partial class CountryViewCell
    {
        [Outlet]
        UIKit.UIImageView CheckBoxImageView { get; set; }


        [Outlet]
        UIKit.UILabel NameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CheckBoxImageView != null) {
                CheckBoxImageView.Dispose ();
                CheckBoxImageView = null;
            }

            if (NameLabel != null) {
                NameLabel.Dispose ();
                NameLabel = null;
            }
        }
    }
}
