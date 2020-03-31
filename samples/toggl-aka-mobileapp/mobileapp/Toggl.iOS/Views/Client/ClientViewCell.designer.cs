// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

using Foundation;

namespace Toggl.iOS.Views.Client
{
	[Register ("ClientViewCell")]
	partial class ClientViewCell
	{
		[Outlet]
		UIKit.UILabel NameLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}
		}
	}
}
