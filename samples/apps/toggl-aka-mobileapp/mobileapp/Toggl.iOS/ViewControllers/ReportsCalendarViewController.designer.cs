// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.ViewControllers
{
	[Register ("ReportsCalendarViewController")]
	partial class ReportsCalendarViewController
	{
		[Outlet]
		UIKit.UICollectionView CalendarCollectionView { get; set; }

		[Outlet]
		UIKit.UILabel CurrentMonthLabel { get; set; }

		[Outlet]
		UIKit.UILabel DayHeader0 { get; set; }

		[Outlet]
		UIKit.UILabel DayHeader1 { get; set; }

		[Outlet]
		UIKit.UILabel DayHeader2 { get; set; }

		[Outlet]
		UIKit.UILabel DayHeader3 { get; set; }

		[Outlet]
		UIKit.UILabel DayHeader4 { get; set; }

		[Outlet]
		UIKit.UILabel DayHeader5 { get; set; }

		[Outlet]
		UIKit.UILabel DayHeader6 { get; set; }

		[Outlet]
		UIKit.UICollectionView QuickSelectCollectionView { get; set; }

		[Outlet]
		UIKit.UICollectionViewFlowLayout QuickSelectCollectionViewLayout { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarCollectionView != null) {
				CalendarCollectionView.Dispose ();
				CalendarCollectionView = null;
			}

			if (CurrentMonthLabel != null) {
				CurrentMonthLabel.Dispose ();
				CurrentMonthLabel = null;
			}

			if (DayHeader0 != null) {
				DayHeader0.Dispose ();
				DayHeader0 = null;
			}

			if (DayHeader1 != null) {
				DayHeader1.Dispose ();
				DayHeader1 = null;
			}

			if (DayHeader2 != null) {
				DayHeader2.Dispose ();
				DayHeader2 = null;
			}

			if (DayHeader3 != null) {
				DayHeader3.Dispose ();
				DayHeader3 = null;
			}

			if (DayHeader4 != null) {
				DayHeader4.Dispose ();
				DayHeader4 = null;
			}

			if (DayHeader5 != null) {
				DayHeader5.Dispose ();
				DayHeader5 = null;
			}

			if (DayHeader6 != null) {
				DayHeader6.Dispose ();
				DayHeader6 = null;
			}

			if (QuickSelectCollectionView != null) {
				QuickSelectCollectionView.Dispose ();
				QuickSelectCollectionView = null;
			}

			if (QuickSelectCollectionViewLayout != null) {
				QuickSelectCollectionViewLayout.Dispose ();
				QuickSelectCollectionViewLayout = null;
			}
		}
	}
}
