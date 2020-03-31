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
	[Register ("TimeEntryMockView")]
	partial class TimeEntryMockView
	{
		[Outlet]
		UIKit.UIView ClientView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ClientWidthConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DescriptionWidthConstraint { get; set; }

		[Outlet]
		UIKit.UIView ProjectView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ProjectWidthConstraint { get; set; }

		[Outlet]
		UIKit.UIView RootView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ClientView != null) {
				ClientView.Dispose ();
				ClientView = null;
			}

			if (ClientWidthConstraint != null) {
				ClientWidthConstraint.Dispose ();
				ClientWidthConstraint = null;
			}

			if (DescriptionWidthConstraint != null) {
				DescriptionWidthConstraint.Dispose ();
				DescriptionWidthConstraint = null;
			}

			if (ProjectView != null) {
				ProjectView.Dispose ();
				ProjectView = null;
			}

			if (ProjectWidthConstraint != null) {
				ProjectWidthConstraint.Dispose ();
				ProjectWidthConstraint = null;
			}

			if (RootView != null) {
				RootView.Dispose ();
				RootView = null;
			}
		}
	}
}
