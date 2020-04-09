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
	[Register ("TimeEntriesEmptyLogView")]
	partial class TimeEntriesEmptyLogView
	{
		[Outlet]
		UIKit.UIView HeaderContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HeaderContainer != null) {
				HeaderContainer.Dispose ();
				HeaderContainer = null;
			}
		}
	}
}
