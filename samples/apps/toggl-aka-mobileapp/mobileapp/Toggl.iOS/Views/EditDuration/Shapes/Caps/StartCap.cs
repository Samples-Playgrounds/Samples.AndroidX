using AVFoundation;
using CoreGraphics;
using System;
using UIKit;

namespace Toggl.iOS.Views.EditDuration.Shapes.Caps
{
    public sealed class StartCap : Cap
    {
        private static readonly CGImage icon = UIImage.FromBundle("icStartLabel").CGImage;

        // The sizes are relative to the radius of the wheel.
        // The radius of the wheel in the design document is 128 points.
        private static readonly nfloat iconCenterHorizontalCorrection = 1.4f / 128f;

        private static readonly nfloat iconWidth = 9f / 128f;

        private static readonly nfloat iconHeight = 10f / 128f;

        public StartCap(Func<nfloat, nfloat> scale)
            : base(icon, scale, iconHeight, iconWidth, ShadowDirection.Right)
        {
            // the triangle icon needs to be offset a bit to the right because when it is centered normally, it feels
            // off and it doesn't match the design
            Frame = new CGRect(Frame.X + scale(iconCenterHorizontalCorrection), Frame.Y, Frame.Width, Frame.Height);
        }
    }
}
