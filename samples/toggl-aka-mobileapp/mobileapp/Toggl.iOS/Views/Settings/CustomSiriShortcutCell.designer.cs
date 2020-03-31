// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Views.Settings
{
	[Register ("CustomSiriShortcutCell")]
	partial class CustomSiriShortcutCell
	{
		[Outlet]
		UIKit.UIImageView BillableIcon { get; set; }

		[Outlet]
		UIKit.UILabel DetailsLabel { get; set; }

		[Outlet]
		UIKit.UILabel InvocationLabel { get; set; }

		[Outlet]
		UIKit.UIImageView TagsIcon { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (InvocationLabel != null) {
				InvocationLabel.Dispose ();
				InvocationLabel = null;
			}

			if (DetailsLabel != null) {
				DetailsLabel.Dispose ();
				DetailsLabel = null;
			}

			if (TagsIcon != null) {
				TagsIcon.Dispose ();
				TagsIcon = null;
			}

			if (BillableIcon != null) {
				BillableIcon.Dispose ();
				BillableIcon = null;
			}
		}
	}
}
