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
	[Register ("LicensesViewCell")]
	partial class LicensesViewCell
	{
		[Outlet]
		UIKit.UIView GrayBackground { get; set; }

		[Outlet]
		UIKit.UILabel LicenseLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (LicenseLabel != null) {
				LicenseLabel.Dispose ();
				LicenseLabel = null;
			}

			if (GrayBackground != null) {
				GrayBackground.Dispose ();
				GrayBackground = null;
			}
		}
	}
}
