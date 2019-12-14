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
	[Register ("SuggestionView")]
	partial class SuggestionView
	{
		[Outlet]
		UIKit.UIImageView ArrowImage { get; set; }

		[Outlet]
		Toggl.iOS.Views.FadeView DescriptionFadeView { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel NoDescriptionLabel { get; set; }

		[Outlet]
		Toggl.iOS.Views.FadeView ProjectFadeView { get; set; }

		[Outlet]
		UIKit.UILabel ProjectLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ArrowImage != null) {
				ArrowImage.Dispose ();
				ArrowImage = null;
			}

			if (DescriptionFadeView != null) {
				DescriptionFadeView.Dispose ();
				DescriptionFadeView = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (NoDescriptionLabel != null) {
				NoDescriptionLabel.Dispose ();
				NoDescriptionLabel = null;
			}

			if (ProjectFadeView != null) {
				ProjectFadeView.Dispose ();
				ProjectFadeView = null;
			}

			if (ProjectLabel != null) {
				ProjectLabel.Dispose ();
				ProjectLabel = null;
			}
		}
	}
}
