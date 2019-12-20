using CoreGraphics;
using System;
using System.Linq;
using UIKit;

namespace Toggl.iOS.Views.EditDuration.Shapes.Caps
{
    public sealed class EndCap : Cap
    {
        private static readonly CGImage icon = UIImage.FromBundle("icEndLabel").CGImage;

        // The sizes are relative to the radius of the wheel.
        // The radius of the wheel in the design document is 128 points.
        private static readonly nfloat iconHeight = 10f / 128f;

        private static readonly nfloat iconWidth = 10f / 128f;

        public bool ShowOnlyBackground
        {
            set => Sublayers.Single().Hidden = value;
        }

        public EndCap(Func<nfloat, nfloat> scale)
            : base(icon, scale, iconHeight, iconWidth, ShadowDirection.Left)
        {
        }
    }
}
