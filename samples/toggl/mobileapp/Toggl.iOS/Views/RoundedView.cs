using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(RoundedView))]
    public sealed class RoundedView : UIView
    {
        private readonly CALayer maskingLayer = new CALayer();

        private bool roundLeft;
        public bool RoundLeft
        {
            get => roundLeft;
            set
            {
                if (roundLeft == value) return;
                roundLeft = value;
                updateMaskingLayer();
            }
        }

        private bool roungRight;
        public bool RoundRight
        {
            get => roungRight;
            set
            {
                if (roungRight == value) return;
                roungRight = value;
                updateMaskingLayer();
            }
        }

        private int cornerRadius;
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                if (cornerRadius == value) return;
                cornerRadius = value;
                updateMaskingLayer();
            }
        }

        public override UIColor BackgroundColor
        {
            get => base.BackgroundColor;
            set
            {
                base.BackgroundColor = value;
                updateMaskingLayer();
            }
        }

        public RoundedView(IntPtr handle) : base(handle)
        {
        }

        public RoundedView(CGRect frame) : base(frame)
        {
            Layer.AddSublayer(maskingLayer);

            updateMaskingLayer();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Layer.AddSublayer(maskingLayer);

            updateMaskingLayer();
        }

        private void updateMaskingLayer()
        {
            // iOS tries to animate things without our consent
            // Trying to create a Core Animation context caused deadlocks
            // which froze the app. Speeding it up fixes the problem.
            maskingLayer.Speed = 999;

            if (!RoundLeft && !RoundRight)
            {
                Layer.CornerRadius = 0;
                maskingLayer.Opacity = 0;
                return;
            }

            Layer.CornerRadius = CornerRadius;

            if (RoundLeft && RoundRight)
            {
                maskingLayer.Opacity = 0;
                return;
            }

            maskingLayer.Opacity = 1;
            maskingLayer.BackgroundColor = BackgroundColor.CGColor;
            var maskingLayerWidth = Bounds.Width - CornerRadius;
            maskingLayer.Frame = new CGRect
            {
                X = RoundLeft ? maskingLayerWidth : 0,
                Y = 0,
                Width = maskingLayerWidth,
                Height = Bounds.Height
            };
        }
    }
}
