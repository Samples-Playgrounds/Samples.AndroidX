// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Cells
{
	[Register ("SyncFailureCell")]
	partial class SyncFailureCell
	{
		[Outlet]
		UIKit.UILabel errorMessageLabel { get; set; }

		[Outlet]
		UIKit.UILabel nameLabel { get; set; }

		[Outlet]
		UIKit.UILabel syncStatusLabel { get; set; }

		[Outlet]
		UIKit.UILabel typeLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (typeLabel != null) {
				typeLabel.Dispose ();
				typeLabel = null;
			}

			if (nameLabel != null) {
				nameLabel.Dispose ();
				nameLabel = null;
			}

			if (syncStatusLabel != null) {
				syncStatusLabel.Dispose ();
				syncStatusLabel = null;
			}

			if (errorMessageLabel != null) {
				errorMessageLabel.Dispose ();
				errorMessageLabel = null;
			}
		}
	}
}
