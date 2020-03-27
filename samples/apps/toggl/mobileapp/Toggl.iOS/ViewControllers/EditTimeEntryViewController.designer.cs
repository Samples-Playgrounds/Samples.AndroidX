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
	[Register ("EditViewController")]
	partial class EditTimeEntryViewController
	{
		[Outlet]
		UIKit.UIStackView AddProjectAndTaskView { get; set; }

		[Outlet]
		UIKit.UILabel AddProjectTaskLabel { get; set; }

		[Outlet]
		UIKit.UILabel AddTagsLabel { get; set; }

		[Outlet]
		UIKit.UIStackView AddTagsView { get; set; }

		[Outlet]
		UIKit.UILabel BillableLabel { get; set; }

		[Outlet]
		UIKit.UISwitch BillableSwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView BillableView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ButtonsContainerBottomConstraint { get; set; }

		[Outlet]
		UIKit.UIView CategorizeWithProjectsBubbleView { get; set; }

		[Outlet]
		UIKit.UILabel CategorizeWithProjectsLabel { get; set; }

		[Outlet]
		UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		UIKit.UIButton ConfirmButton { get; set; }

		[Outlet]
		UIKit.UIButton DeleteButton { get; set; }

		[Outlet]
		Toggl.iOS.Views.TextViewWithPlaceholder DescriptionTextView { get; set; }

		[Outlet]
		UIKit.UIView DescriptionView { get; set; }

		[Outlet]
		UIKit.UILabel DurationDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		UIKit.UIView DurationView { get; set; }

		[Outlet]
		UIKit.UILabel EndDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel EndTimeLabel { get; set; }

		[Outlet]
		UIKit.UIView EndTimeView { get; set; }

		[Outlet]
		UIKit.UILabel ErrorMessageLabel { get; set; }

		[Outlet]
		UIKit.UILabel ErrorMessageTitleLabel { get; set; }

		[Outlet]
		UIKit.UIView ErrorView { get; set; }

		[Outlet]
		UIKit.UILabel GroupDuration { get; set; }

		[Outlet]
		UIKit.UILabel ProjectTaskClientLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIStackView ScrollViewContent { get; set; }

		[Outlet]
		UIKit.UIView SelectProject { get; set; }

		[Outlet]
		UIKit.UILabel StartDateDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel StartDateLabel { get; set; }

		[Outlet]
		UIKit.UIView StartDateView { get; set; }

		[Outlet]
		UIKit.UILabel StartDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel StartTimeLabel { get; set; }

		[Outlet]
		UIKit.UIView StartTimeView { get; set; }

		[Outlet]
		UIKit.UIButton StopButton { get; set; }

		[Outlet]
		UIKit.UIView TagsContainerView { get; set; }

		[Outlet]
		UIKit.UITextView TagsTextView { get; set; }

		[Outlet]
		UIKit.UIStackView TimeEntryTimes { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AddProjectAndTaskView != null) {
				AddProjectAndTaskView.Dispose ();
				AddProjectAndTaskView = null;
			}

			if (AddProjectTaskLabel != null) {
				AddProjectTaskLabel.Dispose ();
				AddProjectTaskLabel = null;
			}

			if (AddTagsLabel != null) {
				AddTagsLabel.Dispose ();
				AddTagsLabel = null;
			}

			if (AddTagsView != null) {
				AddTagsView.Dispose ();
				AddTagsView = null;
			}

			if (BillableLabel != null) {
				BillableLabel.Dispose ();
				BillableLabel = null;
			}

			if (BillableSwitch != null) {
				BillableSwitch.Dispose ();
				BillableSwitch = null;
			}

			if (BillableView != null) {
				BillableView.Dispose ();
				BillableView = null;
			}

			if (CategorizeWithProjectsBubbleView != null) {
				CategorizeWithProjectsBubbleView.Dispose ();
				CategorizeWithProjectsBubbleView = null;
			}

			if (CategorizeWithProjectsLabel != null) {
				CategorizeWithProjectsLabel.Dispose ();
				CategorizeWithProjectsLabel = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (ConfirmButton != null) {
				ConfirmButton.Dispose ();
				ConfirmButton = null;
			}

			if (DeleteButton != null) {
				DeleteButton.Dispose ();
				DeleteButton = null;
			}

			if (ButtonsContainerBottomConstraint != null) {
				ButtonsContainerBottomConstraint.Dispose ();
				ButtonsContainerBottomConstraint = null;
			}

			if (DescriptionTextView != null) {
				DescriptionTextView.Dispose ();
				DescriptionTextView = null;
			}

			if (DescriptionView != null) {
				DescriptionView.Dispose ();
				DescriptionView = null;
			}

			if (DurationDescriptionLabel != null) {
				DurationDescriptionLabel.Dispose ();
				DurationDescriptionLabel = null;
			}

			if (DurationLabel != null) {
				DurationLabel.Dispose ();
				DurationLabel = null;
			}

			if (DurationView != null) {
				DurationView.Dispose ();
				DurationView = null;
			}

			if (EndDescriptionLabel != null) {
				EndDescriptionLabel.Dispose ();
				EndDescriptionLabel = null;
			}

			if (EndTimeLabel != null) {
				EndTimeLabel.Dispose ();
				EndTimeLabel = null;
			}

			if (EndTimeView != null) {
				EndTimeView.Dispose ();
				EndTimeView = null;
			}

			if (ErrorMessageLabel != null) {
				ErrorMessageLabel.Dispose ();
				ErrorMessageLabel = null;
			}

			if (ErrorMessageTitleLabel != null) {
				ErrorMessageTitleLabel.Dispose ();
				ErrorMessageTitleLabel = null;
			}

			if (ErrorView != null) {
				ErrorView.Dispose ();
				ErrorView = null;
			}

			if (GroupDuration != null) {
				GroupDuration.Dispose ();
				GroupDuration = null;
			}

			if (ProjectTaskClientLabel != null) {
				ProjectTaskClientLabel.Dispose ();
				ProjectTaskClientLabel = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (ScrollViewContent != null) {
				ScrollViewContent.Dispose ();
				ScrollViewContent = null;
			}

			if (SelectProject != null) {
				SelectProject.Dispose ();
				SelectProject = null;
			}

			if (StartDateDescriptionLabel != null) {
				StartDateDescriptionLabel.Dispose ();
				StartDateDescriptionLabel = null;
			}

			if (StartDateLabel != null) {
				StartDateLabel.Dispose ();
				StartDateLabel = null;
			}

			if (StartDateView != null) {
				StartDateView.Dispose ();
				StartDateView = null;
			}

			if (StartDescriptionLabel != null) {
				StartDescriptionLabel.Dispose ();
				StartDescriptionLabel = null;
			}

			if (StartTimeLabel != null) {
				StartTimeLabel.Dispose ();
				StartTimeLabel = null;
			}

			if (StartTimeView != null) {
				StartTimeView.Dispose ();
				StartTimeView = null;
			}

			if (StopButton != null) {
				StopButton.Dispose ();
				StopButton = null;
			}

			if (TagsContainerView != null) {
				TagsContainerView.Dispose ();
				TagsContainerView = null;
			}

			if (TagsTextView != null) {
				TagsTextView.Dispose ();
				TagsTextView = null;
			}

			if (TimeEntryTimes != null) {
				TimeEntryTimes.Dispose ();
				TimeEntryTimes = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}
		}
	}
}
