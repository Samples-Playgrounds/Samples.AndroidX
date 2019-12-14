using Foundation;
using UIKit;

namespace Toggl.iOS.Presentation.Transition
{
    public sealed class FromBottomTransitionDelegate : NSObject, IUIViewControllerTransitioningDelegate
    {
        [Export("animationControllerForPresentedController:presentingController:sourceController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(
            UIViewController presented, UIViewController presenting, UIViewController source
        ) => new FromBottomTransition(true);

        [Export("animationControllerForDismissedController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
            => new FromBottomTransition(false);

        [Export("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
        public UIPresentationController GetPresentationControllerForPresentedViewController(
            UIViewController presented, UIViewController presenting, UIViewController source
        ) => new ModalPresentationController(presented, presenting);
    }
}
