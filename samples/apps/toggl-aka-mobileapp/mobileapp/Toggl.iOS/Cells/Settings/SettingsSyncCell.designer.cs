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
	[Register ("SettingsSyncCell")]
	partial class SettingsSyncCell
	{
		[Outlet]
		UIKit.UIView BottomSeparator { get; set; }

		[Outlet]
		Toggl.iOS.Views.ActivityIndicatorView LoadingIcon { get; set; }

		[Outlet]
		UIKit.UILabel StatusLabel { get; set; }

		[Outlet]
		UIKit.UIImageView SyncedIcon { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}

			if (SyncedIcon != null) {
				SyncedIcon.Dispose ();
				SyncedIcon = null;
			}

			if (LoadingIcon != null) {
				LoadingIcon.Dispose ();
				LoadingIcon = null;
			}

			if (BottomSeparator != null) {
				BottomSeparator.Dispose ();
				BottomSeparator = null;
			}
		}
	}
}
