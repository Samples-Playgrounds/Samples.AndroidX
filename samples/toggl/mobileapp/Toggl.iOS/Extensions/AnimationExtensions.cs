using CoreAnimation;
using CoreGraphics;
using System;
using System.Threading;
using UIKit;
using static Toggl.Core.UI.Helper.Animation;

namespace Toggl.iOS.Extensions
{
    public static class AnimationExtensions
    {
        public static CAMediaTimingFunction ToMediaTimingFunction(this CubicBezierCurve self)
            => new CAMediaTimingFunction(self.P0, self.P1, self.P2, self.P3);

        public static UICubicTimingParameters ToCubicTimingParameters(this CubicBezierCurve self)
            => new UICubicTimingParameters(
                new CGPoint(self.P0, self.P1),
                new CGPoint(self.P2, self.P3)
        );

        public static void Animate(double duration, CubicBezierCurve curve, Action changes, Action completion = null, CancellationToken? cancellationToken = null)
            => Animate(duration, 0.0f, curve, changes, completion, cancellationToken);

        public static void Animate(double duration, nfloat delay, CubicBezierCurve curve, Action changes, Action completion = null, CancellationToken? cancellationToken = null)
        {
            var propertyAnimator = new UIViewPropertyAnimator(duration, curve.ToCubicTimingParameters());
            propertyAnimator.AddAnimations(changes, delay);

            if (completion != null)
                propertyAnimator.AddCompletion(_ => completion());

            cancellationToken?.Register(() => propertyAnimator.StopAnimation(true));

            propertyAnimator.StartAnimation();
        }
    }
}
