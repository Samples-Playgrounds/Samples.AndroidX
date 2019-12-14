using CoreGraphics;
using System;
using UIKit;

namespace Toggl.iOS.Views.EditDuration
{
    public abstract class BaseWheelView : UIView
    {
        // The sizes are relative to the radius of the wheel.
        // The radius of the wheel in the design document is 128 points.
        private readonly nfloat wheelThickness = 33f / 128f;

        protected new CGPoint Center { get; set; }

        protected nfloat Radius { get; set; }

        protected nfloat SmallRadius { get; set; }

        protected nfloat Thickness { get; set; }

        public BaseWheelView(IntPtr handle) : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            Radius = 0.5f * Frame.Size.Width;
            SmallRadius = Radius - Resize(wheelThickness);
            Center = new CGPoint(Radius, Radius);
            Thickness = Radius - SmallRadius;
        }

        protected CGAffineTransform CreateTranslationTransform(nfloat radius, nfloat angle)
        {
            var tx = radius * (nfloat)Math.Sin(angle);
            var ty = radius * (nfloat)Math.Cos(angle);
            return CGAffineTransform.MakeTranslation(Center.X + tx, Center.Y - ty);
        }

        protected nfloat Resize(nfloat originalSize)
            => originalSize * Radius;

        protected void RemoveSublayers()
        {
            var sublayers = Layer.Sublayers;
            if (sublayers != null)
            {
                for (var i = 0; i < sublayers.Length; i++)
                {
                    sublayers[i].RemoveFromSuperLayer();
                    sublayers[i] = null;
                }
            }

            for (var i = 0; i < Subviews.Length; i++)
            {
                Subviews[i].RemoveFromSuperview();
                Subviews[i] = null;
            }
        }
    }
}
