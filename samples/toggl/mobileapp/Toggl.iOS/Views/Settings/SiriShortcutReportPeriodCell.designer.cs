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
	[Register ("SiriShortcutReportPeriodCell")]
	partial class SiriShortcutReportPeriodCell
	{
		[Outlet]
		UIKit.UIImageView SelectedImageView { get; set; }

		[Outlet]
		UIKit.UILabel TextLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SelectedImageView != null) {
				SelectedImageView.Dispose ();
				SelectedImageView = null;
			}

			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}
		}
	}
}
