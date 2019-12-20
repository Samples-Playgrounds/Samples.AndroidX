using System;
using Toggl.Core.UI.Helper;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class LayoutConstraintExtensions
    {
        public static void AnimateSetConstant(this NSLayoutConstraint self, nfloat constant, UIView containerView)
        {
            Ensure.Argument.IsNotNull(self, nameof(self));
            Ensure.Argument.IsNotNull(constant, nameof(constant));

            self.Constant = constant;

            Ensure.Argument.IsNotNull(containerView, nameof(containerView));

            UIView.Animate(Animation.Timings.EnterTiming, () => containerView.LayoutIfNeeded());
        }
    }
}
