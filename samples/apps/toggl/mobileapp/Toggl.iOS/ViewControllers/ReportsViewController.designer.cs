// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using Toggl.iOS.Views;

namespace Toggl.iOS.ViewControllers
{
	[Register ("ReportsViewController")]
	partial class ReportsViewController
	{
		[Outlet]
		UIKit.UIView BarChartsContainerView { get; set; }

		[Outlet]
		UIKit.UIView CalendarContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ContentWidthConstraint { get; set; }

		[Outlet]
		UIKit.UIView OverviewContainerView { get; set; }

		[Outlet]
		UIKit.UITableView ReportsTableView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TopCalendarConstraint { get; set; }

		[Outlet]
		UIKit.UIView WorkspaceButton { get; set; }

		[Outlet]
		FadeView WorkspaceFadeView { get; set; }

		[Outlet]
		UIKit.UILabel WorkspaceLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BarChartsContainerView != null) {
				BarChartsContainerView.Dispose ();
				BarChartsContainerView = null;
			}

			if (CalendarContainer != null) {
				CalendarContainer.Dispose ();
				CalendarContainer = null;
			}

			if (OverviewContainerView != null) {
				OverviewContainerView.Dispose ();
				OverviewContainerView = null;
			}

			if (ReportsTableView != null) {
				ReportsTableView.Dispose ();
				ReportsTableView = null;
			}

			if (TopCalendarConstraint != null) {
				TopCalendarConstraint.Dispose ();
				TopCalendarConstraint = null;
			}

			if (WorkspaceButton != null) {
				WorkspaceButton.Dispose ();
				WorkspaceButton = null;
			}

			if (WorkspaceFadeView != null) {
				WorkspaceFadeView.Dispose ();
				WorkspaceFadeView = null;
			}

			if (WorkspaceLabel != null) {
				WorkspaceLabel.Dispose ();
				WorkspaceLabel = null;
			}

			if (ContentWidthConstraint != null) {
				ContentWidthConstraint.Dispose ();
				ContentWidthConstraint = null;
			}
		}
	}
}
