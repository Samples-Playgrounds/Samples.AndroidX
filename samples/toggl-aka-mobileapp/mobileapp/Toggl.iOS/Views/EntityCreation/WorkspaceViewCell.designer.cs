// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Views
{
	[Register ("WorkspaceViewCell")]
	partial class WorkspaceViewCell
	{
		[Outlet]
		UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		UIKit.UIImageView SelectedImage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (SelectedImage != null) {
				SelectedImage.Dispose ();
				SelectedImage = null;
			}
		}
	}
}
