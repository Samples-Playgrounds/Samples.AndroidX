using CoreAnimation;
using CoreGraphics;
using System;
using Toggl.iOS.Extensions;
using UIKit;
using Math = Toggl.Shared.Math;
using static System.Math;

namespace Toggl.iOS.Views.EditDuration.Shapes.Caps
{
    public abstract class Cap : CAShapeLayer
    {
        // The sizes are relative to the radius of the wheel.
        // The radius of the wheel in the design document is 128 points.
        private readonly nfloat outerRadius = 16.5f / 128f;
        private readonly nfloat innerRadius = 14.05f / 128f;

        private readonly CALayer imageLayer;
        private readonly ShadowDirection shadowDirection;

        protected enum ShadowDirection
        {
            Left = 1,
            Right = -1
        };

        public CGColor Color
        {
            set
            {
                FillColor = value;
                imageLayer.BackgroundColor = value;
            }
        }

        public double Angle
        {
            set
            {
                double direction = value + (double)shadowDirection * (PI / 2.0);
                double dx = 1.5 * Cos(direction);
                double dy = 1.5 * Sin(direction);
                ShadowOffset = new CGSize(dx, dy);
            }
        }

        protected Cap(
            CGImage icon,
            Func<nfloat, nfloat> scale,
            nfloat iconHeight,
            nfloat iconWidth,
            ShadowDirection shadowDirection)
        {
            this.shadowDirection = shadowDirection;
            var center = new CGPoint(0, 0);

            var outerPath = new UIBezierPath();
            outerPath.AddArc(center, scale(outerRadius), 0, (nfloat)Math.FullCircle, false);

            Path = outerPath.CGPath;

            var innerPath = new UIBezierPath();
            innerPath.AddArc(center, scale(innerRadius), 0, (nfloat)Math.FullCircle, false);

            var circleLayer = new CAShapeLayer { Path = innerPath.CGPath, FillColor = ColorAssets.Background.CGColor };

            var imageFrame = new CGRect(
                x: center.X - scale(iconWidth) / 2f,
                y: center.Y - scale(iconHeight) / 2f,
                width: scale(iconWidth),
                height: scale(iconHeight));

            var maskLayer = new CALayer { Contents = icon, Frame = new CGRect(0, 0, scale(iconWidth), scale(iconHeight)) };
            imageLayer = new CALayer { Mask = maskLayer, Frame = imageFrame };

            circleLayer.AddSublayer(imageLayer);

            AddSublayer(circleLayer);

            ShadowColor = ColorAssets.Separator.CGColor;
            ShadowRadius = 0.0f;
            ShadowPath = Path;
            ShadowOpacity = 1.0f;
        }
    }
}
