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
	[Register ("CalendarViewController")]
	partial class CalendarViewController
	{
		[Outlet]
		UIKit.UILabel DailyTrackedTimeLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DailyTrackedTimeLeadingConstraint { get; set; }

		[Outlet]
		UIKit.UIView DayViewContainer { get; set; }

		[Outlet]
		UIKit.UILabel SelectedDateLabel { get; set; }

		[Outlet]
		UIKit.UIButton SettingsButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SettingsButtonTrailingConstraint { get; set; }

		[Outlet]
		UIKit.UICollectionView WeekViewCollectionView { get; set; }

		[Outlet]
		UIKit.UIView WeekViewContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint WeekViewContainerWidthConstraint { get; set; }

		[Outlet]
		UIKit.UIView WeekViewDayHeaderContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DailyTrackedTimeLabel != null) {
				DailyTrackedTimeLabel.Dispose ();
				DailyTrackedTimeLabel = null;
			}

			if (DailyTrackedTimeLeadingConstraint != null) {
				DailyTrackedTimeLeadingConstraint.Dispose ();
				DailyTrackedTimeLeadingConstraint = null;
			}

			if (DayViewContainer != null) {
				DayViewContainer.Dispose ();
				DayViewContainer = null;
			}

			if (SelectedDateLabel != null) {
				SelectedDateLabel.Dispose ();
				SelectedDateLabel = null;
			}

			if (SettingsButton != null) {
				SettingsButton.Dispose ();
				SettingsButton = null;
			}

			if (SettingsButtonTrailingConstraint != null) {
				SettingsButtonTrailingConstraint.Dispose ();
				SettingsButtonTrailingConstraint = null;
			}

			if (WeekViewCollectionView != null) {
				WeekViewCollectionView.Dispose ();
				WeekViewCollectionView = null;
			}

			if (WeekViewContainer != null) {
				WeekViewContainer.Dispose ();
				WeekViewContainer = null;
			}

			if (WeekViewContainerWidthConstraint != null) {
				WeekViewContainerWidthConstraint.Dispose ();
				WeekViewContainerWidthConstraint = null;
			}

			if (WeekViewDayHeaderContainer != null) {
				WeekViewDayHeaderContainer.Dispose ();
				WeekViewDayHeaderContainer = null;
			}
		}
	}
}
