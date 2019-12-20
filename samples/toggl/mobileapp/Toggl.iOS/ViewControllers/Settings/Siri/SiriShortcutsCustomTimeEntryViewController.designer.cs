// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.ViewControllers.Settings.Siri
{
	[Register ("SiriShortcutsCustomTimeEntryViewController")]
	partial class SiriShortcutsCustomTimeEntryViewController
	{
		[Outlet]
		UIKit.UIView AddProjectAndTaskView { get; set; }

		[Outlet]
		UIKit.UILabel AddProjectTaskLabel { get; set; }

		[Outlet]
		UIKit.UILabel AddTagsLabel { get; set; }

		[Outlet]
		UIKit.UIView AddTagsView { get; set; }

		[Outlet]
		UIKit.UIView AddToSiriWrapperView { get; set; }

		[Outlet]
		UIKit.UILabel BillabelLabel { get; set; }

		[Outlet]
		UIKit.UISwitch BillableSwitch { get; set; }

		[Outlet]
		UIKit.UIView BillableView { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionFromClipboardLabel { get; set; }

		[Outlet]
		Toggl.iOS.Views.TextViewWithPlaceholder DescriptionTextView { get; set; }

		[Outlet]
		UIKit.UIView DescriptionUsingClipboardWrapperView { get; set; }

		[Outlet]
		UIKit.UIView DescriptionView { get; set; }

		[Outlet]
		UIKit.UIButton PasteFromClipboardButton { get; set; }

		[Outlet]
		UIKit.UILabel PasteFromClipboardHintLabel { get; set; }

		[Outlet]
		UIKit.UIView PasteFromClipboardHintView { get; set; }

		[Outlet]
		UIKit.UILabel ProjectTaskClientLabel { get; set; }

		[Outlet]
		UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		UIKit.UIStackView ScrollViewContent { get; set; }

		[Outlet]
		UIKit.UIView SelectProjectView { get; set; }

		[Outlet]
		UIKit.UIView SelectTagsView { get; set; }

		[Outlet]
		UIKit.UITextView TagsTextView { get; set; }
		
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

			if (AddToSiriWrapperView != null) {
				AddToSiriWrapperView.Dispose ();
				AddToSiriWrapperView = null;
			}

			if (BillabelLabel != null) {
				BillabelLabel.Dispose ();
				BillabelLabel = null;
			}

			if (BillableSwitch != null) {
				BillableSwitch.Dispose ();
				BillableSwitch = null;
			}

			if (BillableView != null) {
				BillableView.Dispose ();
				BillableView = null;
			}

			if (DescriptionFromClipboardLabel != null) {
				DescriptionFromClipboardLabel.Dispose ();
				DescriptionFromClipboardLabel = null;
			}

			if (DescriptionTextView != null) {
				DescriptionTextView.Dispose ();
				DescriptionTextView = null;
			}

			if (DescriptionUsingClipboardWrapperView != null) {
				DescriptionUsingClipboardWrapperView.Dispose ();
				DescriptionUsingClipboardWrapperView = null;
			}

			if (DescriptionView != null) {
				DescriptionView.Dispose ();
				DescriptionView = null;
			}

			if (PasteFromClipboardButton != null) {
				PasteFromClipboardButton.Dispose ();
				PasteFromClipboardButton = null;
			}

			if (PasteFromClipboardHintLabel != null) {
				PasteFromClipboardHintLabel.Dispose ();
				PasteFromClipboardHintLabel = null;
			}

			if (PasteFromClipboardHintView != null) {
				PasteFromClipboardHintView.Dispose ();
				PasteFromClipboardHintView = null;
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

			if (SelectProjectView != null) {
				SelectProjectView.Dispose ();
				SelectProjectView = null;
			}

			if (SelectTagsView != null) {
				SelectTagsView.Dispose ();
				SelectTagsView = null;
			}

			if (TagsTextView != null) {
				TagsTextView.Dispose ();
				TagsTextView = null;
			}
		}
	}
}
