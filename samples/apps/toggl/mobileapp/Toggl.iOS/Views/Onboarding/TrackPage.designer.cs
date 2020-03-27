// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS
{
	[Register ("TrackPage")]
	partial class TrackPage
	{
		[Outlet]
		UIKit.UIView FirstCell { get; set; }

		[Outlet]
		UIKit.UIImageView PlayIcon { get; set; }

		[Outlet]
		UIKit.UIView SecondCell { get; set; }

		[Outlet]
		UIKit.UIView ThirdCell { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (FirstCell != null) {
				FirstCell.Dispose ();
				FirstCell = null;
			}

			if (SecondCell != null) {
				SecondCell.Dispose ();
				SecondCell = null;
			}

			if (ThirdCell != null) {
				ThirdCell.Dispose ();
				ThirdCell = null;
			}

			if (PlayIcon != null) {
				PlayIcon.Dispose ();
				PlayIcon = null;
			}
		}
	}
}
