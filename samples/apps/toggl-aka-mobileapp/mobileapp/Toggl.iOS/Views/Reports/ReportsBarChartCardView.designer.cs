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
	partial class ReportsBarChartCardView
	{
		[Outlet]
		UIKit.UIView BarChartCardView { get; set; }

		[Outlet]
		UIKit.UIView BarChartContainerView { get; set; }

		[Outlet]
		UIKit.UIStackView BarsStackView { get; set; }

		[Outlet]
		UIKit.UILabel BillableLegendLabel { get; set; }

		[Outlet]
		UIKit.UILabel ClockedHoursTitleLabel { get; set; }

		[Outlet]
		UIKit.UIView ColorsLegendContainerView { get; set; }

		[Outlet]
		UIKit.UILabel EndDateLabel { get; set; }

		[Outlet]
		UIKit.UILabel HalfHoursLabel { get; set; }

		[Outlet]
		UIKit.UIStackView HorizontalLegendStackView { get; set; }

		[Outlet]
		UIKit.UILabel MaximumHoursLabel { get; set; }

		[Outlet]
		UIKit.UILabel NonBillableLegendLabel { get; set; }

		[Outlet]
		UIKit.UILabel StartDateLabel { get; set; }

		[Outlet]
		UIKit.UILabel ZeroHoursLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BarChartCardView != null) {
				BarChartCardView.Dispose ();
				BarChartCardView = null;
			}

			if (BarChartContainerView != null) {
				BarChartContainerView.Dispose ();
				BarChartContainerView = null;
			}

			if (BarsStackView != null) {
				BarsStackView.Dispose ();
				BarsStackView = null;
			}

			if (BillableLegendLabel != null) {
				BillableLegendLabel.Dispose ();
				BillableLegendLabel = null;
			}

			if (ClockedHoursTitleLabel != null) {
				ClockedHoursTitleLabel.Dispose ();
				ClockedHoursTitleLabel = null;
			}

			if (ColorsLegendContainerView != null) {
				ColorsLegendContainerView.Dispose ();
				ColorsLegendContainerView = null;
			}

			if (EndDateLabel != null) {
				EndDateLabel.Dispose ();
				EndDateLabel = null;
			}

			if (HalfHoursLabel != null) {
				HalfHoursLabel.Dispose ();
				HalfHoursLabel = null;
			}

			if (HorizontalLegendStackView != null) {
				HorizontalLegendStackView.Dispose ();
				HorizontalLegendStackView = null;
			}

			if (MaximumHoursLabel != null) {
				MaximumHoursLabel.Dispose ();
				MaximumHoursLabel = null;
			}

			if (ZeroHoursLabel != null) {
				ZeroHoursLabel.Dispose ();
				ZeroHoursLabel = null;
			}

			if (NonBillableLegendLabel != null) {
				NonBillableLegendLabel.Dispose ();
				NonBillableLegendLabel = null;
			}

			if (StartDateLabel != null) {
				StartDateLabel.Dispose ();
				StartDateLabel = null;
			}
		}
	}
}
