// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.TimerWidgetExtension
{
	[Register ("SuggestionTableViewCell")]
	partial class SuggestionTableViewCell
	{
		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIView DotView { get; set; }

		[Outlet]
		UIKit.UILabel ProjectLabel { get; set; }

		[Outlet]
		UIKit.UIImageView StartView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DotView != null) {
				DotView.Dispose ();
				DotView = null;
			}

			if (ProjectLabel != null) {
				ProjectLabel.Dispose ();
				ProjectLabel = null;
			}

			if (StartView != null) {
				StartView.Dispose ();
				StartView = null;
			}
		}
	}
}
