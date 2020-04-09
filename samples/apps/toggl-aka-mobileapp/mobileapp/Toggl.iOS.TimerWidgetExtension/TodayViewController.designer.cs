// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.TimerWidgetExtension
{
	[Register ("TodayViewController")]
	partial class TodayViewController
	{
		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIView DotView { get; set; }

		[Outlet]
		UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		UIKit.UILabel ErrorMessageLabel { get; set; }

		[Outlet]
		UIKit.UILabel ProjectNameLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint RunningTimerContainerCompactBottomConstraint { get; set; }

		[Outlet]
		UIKit.UIView RunningTimerContainerView { get; set; }

		[Outlet]
		UIKit.UIButton ShowAllTimeEntriesButton { get; set; }

		[Outlet]
		UIKit.UIButton StartButton { get; set; }

		[Outlet]
		UIKit.UIButton StopButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SuggestionsContainerExpandedBottomConstraint { get; set; }

		[Outlet]
		UIKit.UIView SuggestionsContainerView { get; set; }

		[Outlet]
		UIKit.UILabel SuggestionsLabel { get; set; }

		[Outlet]
		UIKit.UITableView SuggestionsTableView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SuggestionsTableViewHeightConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DotView != null) {
				DotView.Dispose ();
				DotView = null;
			}

			if (DurationLabel != null) {
				DurationLabel.Dispose ();
				DurationLabel = null;
			}

			if (ErrorMessageLabel != null) {
				ErrorMessageLabel.Dispose ();
				ErrorMessageLabel = null;
			}

			if (ProjectNameLabel != null) {
				ProjectNameLabel.Dispose ();
				ProjectNameLabel = null;
			}

			if (RunningTimerContainerCompactBottomConstraint != null) {
				RunningTimerContainerCompactBottomConstraint.Dispose ();
				RunningTimerContainerCompactBottomConstraint = null;
			}

			if (RunningTimerContainerView != null) {
				RunningTimerContainerView.Dispose ();
				RunningTimerContainerView = null;
			}

			if (ShowAllTimeEntriesButton != null) {
				ShowAllTimeEntriesButton.Dispose ();
				ShowAllTimeEntriesButton = null;
			}

			if (StartButton != null) {
				StartButton.Dispose ();
				StartButton = null;
			}

			if (StopButton != null) {
				StopButton.Dispose ();
				StopButton = null;
			}

			if (SuggestionsContainerExpandedBottomConstraint != null) {
				SuggestionsContainerExpandedBottomConstraint.Dispose ();
				SuggestionsContainerExpandedBottomConstraint = null;
			}

			if (SuggestionsContainerView != null) {
				SuggestionsContainerView.Dispose ();
				SuggestionsContainerView = null;
			}

			if (SuggestionsLabel != null) {
				SuggestionsLabel.Dispose ();
				SuggestionsLabel = null;
			}

			if (SuggestionsTableView != null) {
				SuggestionsTableView.Dispose ();
				SuggestionsTableView = null;
			}

			if (SuggestionsTableViewHeightConstraint != null) {
				SuggestionsTableViewHeightConstraint.Dispose ();
				SuggestionsTableViewHeightConstraint = null;
			}
		}
	}
}
