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
	[Register ("January2020CampaignViewController")]
	partial class January2020CampaignViewController
	{
		[Outlet]
		UIKit.UIView BlueRopeView { get; set; }

		[Outlet]
		UIKit.UIButton DismissButton { get; set; }

		[Outlet]
		UIKit.UIButton LearnMoreButton { get; set; }

		[Outlet]
		UIKit.UILabel TextLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BlueRopeView != null) {
				BlueRopeView.Dispose ();
				BlueRopeView = null;
			}

			if (DismissButton != null) {
				DismissButton.Dispose ();
				DismissButton = null;
			}

			if (LearnMoreButton != null) {
				LearnMoreButton.Dispose ();
				LearnMoreButton = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}
		}
	}
}
