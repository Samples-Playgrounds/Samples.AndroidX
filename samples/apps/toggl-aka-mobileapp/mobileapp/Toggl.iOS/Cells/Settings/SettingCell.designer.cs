// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Cells.Settings
{
	[Register ("SettingCell")]
	partial class SettingCell
	{
		[Outlet]
		UIKit.UILabel DetailLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint RightConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DetailLabel != null) {
				DetailLabel.Dispose ();
				DetailLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (RightConstraint != null) {
				RightConstraint.Dispose ();
				RightConstraint = null;
			}
		}
	}
}
