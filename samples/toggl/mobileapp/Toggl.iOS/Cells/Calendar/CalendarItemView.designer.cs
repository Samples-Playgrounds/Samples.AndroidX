// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Cells.Calendar
{
	[Register ("CalendarItemView")]
	partial class CalendarItemView
	{
		[Outlet]
		UIKit.UIView BottomDragIndicator { get; set; }

		[Outlet]
		UIKit.UIView BottomLine { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarIconBaselineConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarIconCenterVerticallyConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarIconHeightConstrarint { get; set; }

		[Outlet]
		UIKit.UIImageView CalendarIconImageView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarIconLeadingConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarIconWidthConstrarint { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DescriptionLabelBottomConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DescriptionLabelLeadingConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DescriptionLabelTopConstraint { get; set; }

		[Outlet]
		UIKit.UIView TopDragIndicator { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BottomDragIndicator != null) {
				BottomDragIndicator.Dispose ();
				BottomDragIndicator = null;
			}

			if (CalendarIconBaselineConstraint != null) {
				CalendarIconBaselineConstraint.Dispose ();
				CalendarIconBaselineConstraint = null;
			}

			if (CalendarIconCenterVerticallyConstraint != null) {
				CalendarIconCenterVerticallyConstraint.Dispose ();
				CalendarIconCenterVerticallyConstraint = null;
			}

			if (CalendarIconHeightConstrarint != null) {
				CalendarIconHeightConstrarint.Dispose ();
				CalendarIconHeightConstrarint = null;
			}

			if (CalendarIconImageView != null) {
				CalendarIconImageView.Dispose ();
				CalendarIconImageView = null;
			}

			if (CalendarIconLeadingConstraint != null) {
				CalendarIconLeadingConstraint.Dispose ();
				CalendarIconLeadingConstraint = null;
			}

			if (CalendarIconWidthConstrarint != null) {
				CalendarIconWidthConstrarint.Dispose ();
				CalendarIconWidthConstrarint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DescriptionLabelBottomConstraint != null) {
				DescriptionLabelBottomConstraint.Dispose ();
				DescriptionLabelBottomConstraint = null;
			}

			if (DescriptionLabelLeadingConstraint != null) {
				DescriptionLabelLeadingConstraint.Dispose ();
				DescriptionLabelLeadingConstraint = null;
			}

			if (DescriptionLabelTopConstraint != null) {
				DescriptionLabelTopConstraint.Dispose ();
				DescriptionLabelTopConstraint = null;
			}

			if (TopDragIndicator != null) {
				TopDragIndicator.Dispose ();
				TopDragIndicator = null;
			}

			if (BottomLine != null) {
				BottomLine.Dispose ();
				BottomLine = null;
			}

		}
	}
}
