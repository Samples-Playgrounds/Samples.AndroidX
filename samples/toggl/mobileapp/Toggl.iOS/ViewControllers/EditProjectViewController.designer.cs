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
	[Register ("EditProjectViewController")]
	partial class EditProjectViewController
	{
		[Outlet]
		UIKit.UILabel ClientLabel { get; set; }

		[Outlet]
		UIKit.UIView ClientView { get; set; }

		[Outlet]
		UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		UIKit.UIView ColorCircleView { get; set; }

		[Outlet]
		UIKit.UIView ColorPickerOpeningView { get; set; }

		[Outlet]
		UIKit.UIButton DoneButton { get; set; }

		[Outlet]
		UIKit.UILabel ErrorLabel { get; set; }

		[Outlet]
		UIKit.UITextField NameTextField { get; set; }

		[Outlet]
		UIKit.UIView NameView { get; set; }

		[Outlet]
		UIKit.UILabel PrivateProjectLabel { get; set; }

		[Outlet]
		UIKit.UISwitch PrivateProjectSwitch { get; set; }

		[Outlet]
		UIKit.UIView PrivateProjectSwitchContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ProjectNameUsedErrorTextHeight { get; set; }

		[Outlet]
		UIKit.UIView ProjectNameUsedErrorView { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel WorkspaceLabel { get; set; }

		[Outlet]
		UIKit.UIView WorkspaceView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ClientLabel != null) {
				ClientLabel.Dispose ();
				ClientLabel = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (ColorCircleView != null) {
				ColorCircleView.Dispose ();
				ColorCircleView = null;
			}

			if (ColorPickerOpeningView != null) {
				ColorPickerOpeningView.Dispose ();
				ColorPickerOpeningView = null;
			}

			if (DoneButton != null) {
				DoneButton.Dispose ();
				DoneButton = null;
			}

			if (ErrorLabel != null) {
				ErrorLabel.Dispose ();
				ErrorLabel = null;
			}

			if (NameTextField != null) {
				NameTextField.Dispose ();
				NameTextField = null;
			}

			if (PrivateProjectLabel != null) {
				PrivateProjectLabel.Dispose ();
				PrivateProjectLabel = null;
			}

			if (PrivateProjectSwitch != null) {
				PrivateProjectSwitch.Dispose ();
				PrivateProjectSwitch = null;
			}

			if (PrivateProjectSwitchContainer != null) {
				PrivateProjectSwitchContainer.Dispose ();
				PrivateProjectSwitchContainer = null;
			}

			if (ProjectNameUsedErrorTextHeight != null) {
				ProjectNameUsedErrorTextHeight.Dispose ();
				ProjectNameUsedErrorTextHeight = null;
			}

			if (ProjectNameUsedErrorView != null) {
				ProjectNameUsedErrorView.Dispose ();
				ProjectNameUsedErrorView = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (WorkspaceLabel != null) {
				WorkspaceLabel.Dispose ();
				WorkspaceLabel = null;
			}

			if (NameView != null) {
				NameView.Dispose ();
				NameView = null;
			}

			if (WorkspaceView != null) {
				WorkspaceView.Dispose ();
				WorkspaceView = null;
			}

			if (ClientView != null) {
				ClientView.Dispose ();
				ClientView = null;
			}
		}
	}
}
