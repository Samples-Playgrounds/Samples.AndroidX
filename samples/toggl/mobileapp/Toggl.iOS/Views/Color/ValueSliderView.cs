using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(ValueSliderView))]
    public class ValueSliderView : UIView
    {
        private readonly CAGradientLayer layer = new CAGradientLayer();

        private float hue;
        public float Hue
        {
            get => hue;
            set
            {
                if (hue == value) return;
                hue = value;
                updateValues();
            }
        }

        private float saturation;
        public float Saturation
        {
            get => saturation;
            set
            {
                if (saturation == value) return;
                saturation = value;
                updateValues();
            }
        }

        public ValueSliderView(IntPtr handle)
            : base(handle)
        {
            layer.BorderWidth = 1;
            layer.CornerRadius = 3;
            layer.StartPoint = new CGPoint(0, 0);
            layer.EndPoint = new CGPoint(1, 0);
            layer.Locations = new NSNumber[] { 0, 1.0 };
            layer.BorderColor = UIColor.Black.ColorWithAlpha(0.08f).CGColor;
            Layer.InsertSublayer(layer, 0);

            updateValues();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            layer.Frame = Bounds;
        }

        private void updateValues()
        {
            layer.Colors = new[]
            {
                UIColor.FromHSB(hue, saturation, 1.0f).CGColor,
                UIColor.FromHSB(hue, saturation, 0.0f).CGColor
            };
        }
    }
}
