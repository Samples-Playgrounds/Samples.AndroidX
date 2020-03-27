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
	[Register ("TaskSuggestionViewCell")]
	partial class TaskSuggestionViewCell
	{
		[Outlet]
		FadeView FadeView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel TaskNameLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (TaskNameLabel != null) {
				TaskNameLabel.Dispose ();
				TaskNameLabel = null;
			}

			if (FadeView != null) {
				FadeView.Dispose ();
				FadeView = null;
			}
		}
	}
}
