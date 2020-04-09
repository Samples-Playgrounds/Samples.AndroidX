// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Views.Reports
{
	[Register ("ReportsLegendViewCell")]
	partial class ReportsLegendViewCell
	{
		[Outlet]
		UIKit.UIView CircleView { get; set; }

		[Outlet]
		UIKit.UILabel ClientLabel { get; set; }

		[Outlet]
		FadeView FadeView { get; set; }

		[Outlet]
		UIKit.UILabel PercentageLabel { get; set; }

		[Outlet]
		UIKit.UILabel ProjectLabel { get; set; }

		[Outlet]
		UIKit.UILabel TotalTimeLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CircleView != null) {
				CircleView.Dispose ();
				CircleView = null;
			}

			if (PercentageLabel != null) {
				PercentageLabel.Dispose ();
				PercentageLabel = null;
			}

			if (ProjectLabel != null) {
				ProjectLabel.Dispose ();
				ProjectLabel = null;
			}

			if (ClientLabel != null) {
				ClientLabel.Dispose ();
				ClientLabel = null;
			}

			if (TotalTimeLabel != null) {
				TotalTimeLabel.Dispose ();
				TotalTimeLabel = null;
			}

			if (FadeView != null) {
				FadeView.Dispose ();
				FadeView = null;
			}
		}
	}
}
