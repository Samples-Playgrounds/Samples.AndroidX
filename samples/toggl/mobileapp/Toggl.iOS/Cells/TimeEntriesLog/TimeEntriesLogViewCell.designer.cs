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
	[Register ("TimeEntriesLogViewCell")]
	partial class TimeEntriesLogViewCell
	{
		[Outlet]
		UIKit.UILabel AddDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIView BillableIcon { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton ContinueButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView ContinueImageView { get; set; }

		[Outlet]
		FadeView DescriptionFadeView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIView GroupSizeBackground { get; set; }

		[Outlet]
		UIKit.UIView GroupSizeContainer { get; set; }

		[Outlet]
		UIKit.UILabel GroupSizeLabel { get; set; }

		[Outlet]
		FadeView ProjectTaskClientFadeView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel ProjectTaskClientLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView SyncErrorImageView { get; set; }

		[Outlet]
		UIKit.UIView TagIcon { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TimeEntryContentLeadingConstraint { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel TimeLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView UnsyncedImageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AddDescriptionLabel != null) {
				AddDescriptionLabel.Dispose ();
				AddDescriptionLabel = null;
			}

			if (BillableIcon != null) {
				BillableIcon.Dispose ();
				BillableIcon = null;
			}

			if (ContinueButton != null) {
				ContinueButton.Dispose ();
				ContinueButton = null;
			}

			if (ContinueImageView != null) {
				ContinueImageView.Dispose ();
				ContinueImageView = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DescriptionFadeView != null) {
				DescriptionFadeView.Dispose ();
				DescriptionFadeView = null;
			}

			if (ProjectTaskClientFadeView != null) {
				ProjectTaskClientFadeView.Dispose ();
				ProjectTaskClientFadeView = null;
			}

			if (GroupSizeBackground != null) {
				GroupSizeBackground.Dispose ();
				GroupSizeBackground = null;
			}

			if (GroupSizeContainer != null) {
				GroupSizeContainer.Dispose ();
				GroupSizeContainer = null;
			}

			if (GroupSizeLabel != null) {
				GroupSizeLabel.Dispose ();
				GroupSizeLabel = null;
			}

			if (ProjectTaskClientLabel != null) {
				ProjectTaskClientLabel.Dispose ();
				ProjectTaskClientLabel = null;
			}

			if (SyncErrorImageView != null) {
				SyncErrorImageView.Dispose ();
				SyncErrorImageView = null;
			}

			if (TagIcon != null) {
				TagIcon.Dispose ();
				TagIcon = null;
			}

			if (TimeEntryContentLeadingConstraint != null) {
				TimeEntryContentLeadingConstraint.Dispose ();
				TimeEntryContentLeadingConstraint = null;
			}

			if (TimeLabel != null) {
				TimeLabel.Dispose ();
				TimeLabel = null;
			}

			if (UnsyncedImageView != null) {
				UnsyncedImageView.Dispose ();
				UnsyncedImageView = null;
			}
		}
	}
}
