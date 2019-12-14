// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in the XCode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.ViewControllers.Settings
{
	[Register ("SiriShortcutsSelectReportPeriodViewController")]
	partial class SiriShortcutsSelectReportPeriodViewController
	{
		[Outlet]
		UIKit.UIView AddToSiriWrapperView { get; set; }

		[Outlet]
		UIKit.UILabel SelectWorkspaceCellLabel { get; set; }

		[Outlet]
		UIKit.UILabel SelectWorkspaceNameLabel { get; set; }

		[Outlet]
		UIKit.UIView SelectWorkspaceView { get; set; }

		[Outlet]
		UIKit.UIView TableFooterView { get; set; }

		[Outlet]
		UIKit.UITableView TableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AddToSiriWrapperView != null) {
				AddToSiriWrapperView.Dispose ();
				AddToSiriWrapperView = null;
			}

			if (SelectWorkspaceCellLabel != null) {
				SelectWorkspaceCellLabel.Dispose ();
				SelectWorkspaceCellLabel = null;
			}

			if (SelectWorkspaceNameLabel != null) {
				SelectWorkspaceNameLabel.Dispose ();
				SelectWorkspaceNameLabel = null;
			}

			if (SelectWorkspaceView != null) {
				SelectWorkspaceView.Dispose ();
				SelectWorkspaceView = null;
			}

			if (TableFooterView != null) {
				TableFooterView.Dispose ();
				TableFooterView = null;
			}

			if (TableView != null) {
				TableView.Dispose ();
				TableView = null;
			}

		}
	}
}
