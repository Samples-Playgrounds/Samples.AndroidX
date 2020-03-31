using System;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Reactive;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class NSConstraintExtensions
    {
        public static Action<nfloat> Constant(this IReactive<NSLayoutConstraint> reactive)
            => constant => reactive.Base.Constant = constant;

        public static Action<bool> Active(this IReactive<NSLayoutConstraint> reactive)
            => isActive => reactive.Base.Active = isActive;

        public static Action<nfloat> ConstantAnimated(this IReactive<NSLayoutConstraint> reactive)
            => constant =>
            {
                reactive.Base.Constant = constant;
                AnimationExtensions.Animate(
                    Animation.Timings.EnterTiming,
                    Animation.Curves.SharpCurve,
                    () => ((UIView)reactive.Base.FirstItem).Superview.LayoutIfNeeded());
            };
    }
}
