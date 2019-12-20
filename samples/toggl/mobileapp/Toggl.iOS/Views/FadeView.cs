using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(FadeView))]
    public sealed class FadeView : UIView
    {
        private readonly CAGradientLayer fadeLayer = new CAGradientLayer();

        private nfloat fadeWidth = 8;
        public nfloat FadeWidth
        {
            get => fadeWidth;
            set
            {
                if (fadeWidth == value) return;
                fadeWidth = value;
                updateFade();
            }
        }

        private bool fadeLeft;
        public bool FadeLeft
        {
            get => fadeLeft;
            set
            {
                if (fadeLeft == value) return;
                fadeLeft = value;
                updateFade();
            }
        }

        private bool fadeRight;
        public bool FadeRight
        {
            get => fadeRight;
            set
            {
                if (fadeRight == value) return;
                fadeRight = value;
                updateFade();
            }
        }

        public FadeView(IntPtr handle) : base(handle)
        {
            Layer.Mask = fadeLayer;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (fadeLayer?.Bounds == Bounds) return;

            updateFade();
        }

        private void updateFade()
        {
            var relativeFadingStart
                = FadeWidth / Bounds.Width;

            var transparentColor = new UIColor(0, 0).CGColor;
            var opaqueColor = new UIColor(0, 1).CGColor;

            fadeLayer.Frame = Bounds;

            fadeLayer.StartPoint = new CGPoint(0, 0);
            fadeLayer.EndPoint = new CGPoint(1, 0);

            fadeLayer.Colors = new CGColor[]
            {
                FadeLeft ? transparentColor : opaqueColor,
                opaqueColor,
                opaqueColor,
                FadeRight ? transparentColor : opaqueColor
            };

            fadeLayer.Locations = new NSNumber[]
            {
                0,
                new NSNumber(relativeFadingStart),
                new NSNumber(1 - relativeFadingStart),
                1
            };
        }
    }
}
