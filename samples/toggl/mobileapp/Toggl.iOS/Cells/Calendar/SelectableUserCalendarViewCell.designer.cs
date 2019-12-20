// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using Toggl.iOS.Views;

namespace Toggl.iOS.Cells.Calendar
{
	[Register ("SelectableUserCalendarViewCell")]
	partial class SelectableUserCalendarViewCell
	{
		[Outlet]
		UIKit.UILabel CalendarNameLabel { get; set; }

		[Outlet]
		FadeView FadeView { get; set; }

		[Outlet]
		UIKit.UISwitch IsSelectedSwitch { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CalendarNameLabel != null) {
				CalendarNameLabel.Dispose ();
				CalendarNameLabel = null;
			}

			if (IsSelectedSwitch != null) {
				IsSelectedSwitch.Dispose ();
				IsSelectedSwitch = null;
			}

			if (FadeView != null) {
				FadeView.Dispose ();
				FadeView = null;
			}
		}
	}
}
