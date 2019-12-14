using System;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    internal sealed class BarSegmentView : UIView
    {
        private const float maximumWidth = 40;

        private readonly UIView internalView;

        private bool shouldSetupConstraints = true;

        public BarSegmentView(UIColor color)
        {
            TranslatesAutoresizingMaskIntoConstraints = false;

            internalView = new UIView { BackgroundColor = color, TranslatesAutoresizingMaskIntoConstraints = false };
            AddSubview(internalView);
        }

        public BarSegmentView(IntPtr handle) : base(handle)
        {
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            if (!shouldSetupConstraints) return;

            internalView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            internalView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;

            // The internal view should fill the whole view unless it would
            // be wider than the maximum defiend width. In that case the width
            // of the internal view should be equal to the maximum width and it
            // should be centered inside of the view. This behavior is achieved
            // using the priorities of the constraints (width <= max has higher
            // priority than the filling the view horizontally from the leading
            // to the trailing edge.
            var leading = internalView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor);
            leading.Priority = 750;
            leading.Active = true;
            var trailing = internalView.TrailingAnchor.ConstraintEqualTo(TrailingAnchor);
            trailing.Priority = 750;
            trailing.Active = true;

            internalView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            internalView.WidthAnchor.ConstraintLessThanOrEqualTo(maximumWidth).Active = true;

            shouldSetupConstraints = false;
        }
    }
}
