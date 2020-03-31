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
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		UIKit.UILabel CreatedFirstTimeEntryLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		Toggl.iOS.Views.AccessibilityAwareView CurrentTimeEntryCard { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel CurrentTimeEntryDescriptionLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel CurrentTimeEntryElapsedTimeLabel { get; set; }

		[Outlet]
		UIKit.UILabel CurrentTimeEntryProjectTaskClientLabel { get; set; }

		[Outlet]
		UIKit.UILabel FeedbackSentDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel FeedbackSentSuccessTitleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		Toggl.iOS.Views.FadeView RunningEntryDescriptionFadeView { get; set; }

		[Outlet]
		UIKit.UIView SendFeedbackSuccessView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton StartTimeEntryButton { get; set; }

		[Outlet]
		UIKit.UIView StartTimeEntryOnboardingBubbleView { get; set; }

		[Outlet]
		UIKit.UILabel StartTimerBubbleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton StopTimeEntryButton { get; set; }

		[Outlet]
		UIKit.UIView StopTimeEntryOnboardingBubbleView { get; set; }

		[Outlet]
		UIKit.UIView TableShadow { get; set; }

		[Outlet]
		UIKit.UIView TapToEditBubbleView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TapToEditBubbleViewTopConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TapToEditItLabel { get; set; }

		[Outlet]
		UIKit.UILabel TapToStopTimerLabel { get; set; }

		[Outlet]
		Toggl.iOS.Views.TimeEntriesLogTableView TimeEntriesLogTableView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TopConstraint { get; set; }

		[Outlet]
		UIKit.UILabel WelcomeBackDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel WelcomeBackLabel { get; set; }

		[Outlet]
		UIKit.UIView WelcomeBackView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CreatedFirstTimeEntryLabel != null) {
				CreatedFirstTimeEntryLabel.Dispose ();
				CreatedFirstTimeEntryLabel = null;
			}

			if (CurrentTimeEntryCard != null) {
				CurrentTimeEntryCard.Dispose ();
				CurrentTimeEntryCard = null;
			}

			if (CurrentTimeEntryDescriptionLabel != null) {
				CurrentTimeEntryDescriptionLabel.Dispose ();
				CurrentTimeEntryDescriptionLabel = null;
			}

			if (CurrentTimeEntryElapsedTimeLabel != null) {
				CurrentTimeEntryElapsedTimeLabel.Dispose ();
				CurrentTimeEntryElapsedTimeLabel = null;
			}

			if (CurrentTimeEntryProjectTaskClientLabel != null) {
				CurrentTimeEntryProjectTaskClientLabel.Dispose ();
				CurrentTimeEntryProjectTaskClientLabel = null;
			}

			if (FeedbackSentDescriptionLabel != null) {
				FeedbackSentDescriptionLabel.Dispose ();
				FeedbackSentDescriptionLabel = null;
			}

			if (FeedbackSentSuccessTitleLabel != null) {
				FeedbackSentSuccessTitleLabel.Dispose ();
				FeedbackSentSuccessTitleLabel = null;
			}

			if (RunningEntryDescriptionFadeView != null) {
				RunningEntryDescriptionFadeView.Dispose ();
				RunningEntryDescriptionFadeView = null;
			}

			if (SendFeedbackSuccessView != null) {
				SendFeedbackSuccessView.Dispose ();
				SendFeedbackSuccessView = null;
			}

			if (StartTimeEntryButton != null) {
				StartTimeEntryButton.Dispose ();
				StartTimeEntryButton = null;
			}

			if (StartTimeEntryOnboardingBubbleView != null) {
				StartTimeEntryOnboardingBubbleView.Dispose ();
				StartTimeEntryOnboardingBubbleView = null;
			}

			if (StartTimerBubbleLabel != null) {
				StartTimerBubbleLabel.Dispose ();
				StartTimerBubbleLabel = null;
			}

			if (StopTimeEntryButton != null) {
				StopTimeEntryButton.Dispose ();
				StopTimeEntryButton = null;
			}

			if (StopTimeEntryOnboardingBubbleView != null) {
				StopTimeEntryOnboardingBubbleView.Dispose ();
				StopTimeEntryOnboardingBubbleView = null;
			}

			if (TapToEditBubbleView != null) {
				TapToEditBubbleView.Dispose ();
				TapToEditBubbleView = null;
			}

			if (TapToEditBubbleViewTopConstraint != null) {
				TapToEditBubbleViewTopConstraint.Dispose ();
				TapToEditBubbleViewTopConstraint = null;
			}

			if (TapToEditItLabel != null) {
				TapToEditItLabel.Dispose ();
				TapToEditItLabel = null;
			}

			if (TapToStopTimerLabel != null) {
				TapToStopTimerLabel.Dispose ();
				TapToStopTimerLabel = null;
			}

			if (TimeEntriesLogTableView != null) {
				TimeEntriesLogTableView.Dispose ();
				TimeEntriesLogTableView = null;
			}

			if (TopConstraint != null) {
				TopConstraint.Dispose ();
				TopConstraint = null;
			}

			if (WelcomeBackDescriptionLabel != null) {
				WelcomeBackDescriptionLabel.Dispose ();
				WelcomeBackDescriptionLabel = null;
			}

			if (WelcomeBackLabel != null) {
				WelcomeBackLabel.Dispose ();
				WelcomeBackLabel = null;
			}

			if (WelcomeBackView != null) {
				WelcomeBackView.Dispose ();
				WelcomeBackView = null;
			}

			if (TableShadow != null) {
				TableShadow.Dispose ();
				TableShadow = null;
			}
		}
	}
}
