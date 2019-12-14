// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.SiriExtension.UI
{
	[Register ("IntentViewController")]
	partial class IntentViewController
	{
		[Outlet]
		ConfirmationView confirmationView { get; set; }

		[Outlet]
		EntryInfoView entryInfoView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (entryInfoView != null) {
				entryInfoView.Dispose ();
				entryInfoView = null;
			}

			if (confirmationView != null) {
				confirmationView.Dispose ();
				confirmationView = null;
			}
		}
	}
}
