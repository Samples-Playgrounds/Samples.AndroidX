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
	[Register ("ReportsHeaderView")]
	partial class ReportsHeaderView
	{
		[Outlet]
		UIKit.UIView BarChartContainerView { get; set; }

		[Outlet]
		UIKit.UILabel EmptyStateDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel EmptyStateTitleLabel { get; set; }

		[Outlet]
		UIKit.UIView EmptyStateView { get; set; }

		[Outlet]
		Toggl.iOS.Views.Reports.LoadingPieChartView LoadingPieChartView { get; set; }

		[Outlet]
		UIKit.UIView OverviewContainerView { get; set; }

		[Outlet]
		UIKit.UIView PieChartBackground { get; set; }

		[Outlet]
		UIKit.UIView PieChartContainer { get; set; }

		[Outlet]
		Toggl.iOS.Views.Reports.PieChartView PieChartView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BarChartContainerView != null) {
				BarChartContainerView.Dispose ();
				BarChartContainerView = null;
			}

			if (EmptyStateDescriptionLabel != null) {
				EmptyStateDescriptionLabel.Dispose ();
				EmptyStateDescriptionLabel = null;
			}

			if (EmptyStateTitleLabel != null) {
				EmptyStateTitleLabel.Dispose ();
				EmptyStateTitleLabel = null;
			}

			if (EmptyStateView != null) {
				EmptyStateView.Dispose ();
				EmptyStateView = null;
			}

			if (LoadingPieChartView != null) {
				LoadingPieChartView.Dispose ();
				LoadingPieChartView = null;
			}

			if (OverviewContainerView != null) {
				OverviewContainerView.Dispose ();
				OverviewContainerView = null;
			}

			if (PieChartContainer != null) {
				PieChartContainer.Dispose ();
				PieChartContainer = null;
			}

			if (PieChartView != null) {
				PieChartView.Dispose ();
				PieChartView = null;
			}

			if (PieChartBackground != null) {
				PieChartBackground.Dispose ();
				PieChartBackground = null;
			}
		}
	}
}
