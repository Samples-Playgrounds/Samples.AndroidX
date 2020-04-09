using CoreGraphics;
using Foundation;
using Toggl.iOS.Extensions;
using UIKit;
using static Toggl.Core.UI.Helper.Animation;

namespace Toggl.iOS.Presentation.Transition
{
    public sealed class FromBottomTransition : NSObject, IUIViewControllerAnimatedTransitioning
    {
        private readonly bool presenting;

        public FromBottomTransition(bool presenting)
        {
            this.presenting = presenting;
        }

        public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
            => presenting ? Timings.EnterTiming : Timings.LeaveTiming;

        public void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var toController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
            var fromController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            var animationDuration = TransitionDuration(transitionContext);

            if (presenting)
            {
                transitionContext.ContainerView.AddSubview(toController.View);

                var finalFrame = transitionContext.GetFinalFrameForViewController(toController);

                var frame = new CGRect(finalFrame.Location, finalFrame.Size);
                frame.Offset(0.0f, transitionContext.ContainerView.Frame.Height - 20);
                toController.View.Frame = frame;
                toController.View.Alpha = 0.5f;

                AnimationExtensions.Animate(animationDuration, Curves.CardInCurve, () =>
                {
                    toController.View.Frame = finalFrame;
                    toController.View.Alpha = 1.0f;
                },
                () => transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled));
            }
            else
            {
                var initialFrame = transitionContext.GetInitialFrameForViewController(fromController);
                initialFrame.Offset(0.0f, transitionContext.ContainerView.Frame.Height);
                var finalFrame = initialFrame;

                if (transitionContext.IsInteractive)
                {
                    UIView.Animate(
                        animationDuration,
                        () => fromController.View.Frame = finalFrame,
                        () => transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled)
                    );
                }
                else
                {
                    AnimationExtensions.Animate(animationDuration, Curves.CardOutCurve, () =>
                    {
                        fromController.View.Frame = finalFrame;
                        fromController.View.Alpha = 0.5f;
                    },
                    () => transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled));
                }
            }
        }
    }
}
