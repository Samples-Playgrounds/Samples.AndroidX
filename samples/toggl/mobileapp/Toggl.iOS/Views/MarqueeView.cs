using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(MarqueeView))]
    public sealed class MarqueeView : UIScrollView
    {
        private const int marqueeVelocity = 60;
        private const int pausedSecondsInAnimationCycle = 2;
        private const int initialAnimationDelaySeconds = 1;
        //3 << 16 because using UIViewKeyframeAnimationOptions.CalculationModeLinear doesn't work.
        //Thanks Apple
        private const UIViewKeyframeAnimationOptions linearCurveOption
            = (UIViewKeyframeAnimationOptions)(3 << 16);

        public MarqueeView(CGRect frame) : base(frame) { }

        public MarqueeView(IntPtr handle) : base(handle) { }

        public CAGradientLayer mask;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            Layer.RemoveAllAnimations();
            ContentOffset = CGPoint.Empty;

            if (ContentSize.Width < Bounds.Width)
                centerTheContent();
            else
                startAnimation();
        }

        private void startAnimation()
        {
            var contentWidth = ContentSize.Width;
            var end = new CGPoint(contentWidth - Bounds.Width, 0);

            var duration = contentWidth / marqueeVelocity;
            var durationWithPause = duration + pausedSecondsInAnimationCycle;

            var relativeStart = 1 / durationWithPause;
            var relativeLength = 1 - 2 * relativeStart;

            AnimateKeyframes(
                duration: durationWithPause,
                delay: initialAnimationDelaySeconds,
                options: UIViewKeyframeAnimationOptions.Repeat
                    | UIViewKeyframeAnimationOptions.Autoreverse
                    | linearCurveOption,
                animations: () =>
                {
                    AddKeyframeWithRelativeStartTime(relativeStart, relativeLength, () => ContentOffset = end);
                },
                completion: _ => { } //Because this can't be null
            );
        }

        private void centerTheContent()
        {
            var xOffset = -(Bounds.Width - ContentSize.Width) / 2;
            ContentOffset = new CGPoint(xOffset, 0);
        }
    }
}
