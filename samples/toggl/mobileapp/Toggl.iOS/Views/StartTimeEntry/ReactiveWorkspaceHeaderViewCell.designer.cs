// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Views.StartTimeEntry
{
	[Register ("ReactiveWorkspaceHeaderViewCell")]
	partial class ReactiveWorkspaceHeaderViewCell
	{
		[Outlet]
		UIKit.UILabel WorkspaceNameLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (WorkspaceNameLabel != null) {
				WorkspaceNameLabel.Dispose ();
				WorkspaceNameLabel = null;
			}
		}
	}
}
