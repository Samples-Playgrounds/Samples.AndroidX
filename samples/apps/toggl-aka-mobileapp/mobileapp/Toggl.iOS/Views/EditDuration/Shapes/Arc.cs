using CoreAnimation;
using CoreGraphics;
using System;
using UIKit;
using Math = Toggl.Shared.Math;

namespace Toggl.iOS.Views.EditDuration.Shapes
{
    public sealed class Arc : CAShapeLayer
    {
        public CGColor Color
        {
            set => StrokeColor = value;
        }

        public Arc(
            CGRect bounds,
            CGPoint center,
            nfloat radius,
            nfloat width)
        {
            var durationArc = new UIBezierPath();
            durationArc.AddArc(center, radius, 0, (nfloat)Math.FullCircle, clockWise: true);

            Path = durationArc.CGPath;
            FillColor = UIColor.Clear.CGColor;
            LineWidth = width;
            StrokeStart = 0;
            Position = center;
            Bounds = bounds;
        }

        public void Update(nfloat startAngle, nfloat endAngle)
        {
            var diffAngle = endAngle - startAngle + (endAngle < startAngle ? Math.FullCircle : 0);
            Transform = CATransform3D.MakeRotation(startAngle, 0, 0, 1f);
            StrokeEnd = (nfloat)(diffAngle / Math.FullCircle);
        }
    }
}
