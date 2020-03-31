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
	[Register ("PasteFromClipboardViewController")]
	partial class PasteFromClipboardViewController
	{
		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIButton DoNotShowAgainButton { get; set; }

		[Outlet]
		UIKit.UIButton OkayButton { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DoNotShowAgainButton != null) {
				DoNotShowAgainButton.Dispose ();
				DoNotShowAgainButton = null;
			}

			if (OkayButton != null) {
				OkayButton.Dispose ();
				OkayButton = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}
		}
	}
}
