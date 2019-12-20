using Foundation;
using UIKit;

namespace Toggl.iOS.Presentation.Transition
{
    public class ModalDialogTransitionDelegate : NSObject, IUIViewControllerTransitioningDelegate
    {
        [Export("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
        public UIPresentationController GetPresentationControllerForPresentedViewController(UIViewController presented, UIViewController presenting, UIViewController source)
            => new ModalDialogPresentationController(presented, presenting);

        [Export("animationControllerForPresentedController:presentingController:sourceController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(
            UIViewController presented, UIViewController presenting, UIViewController source
        ) => new FromBottomTransition(true);

        [Export("animationControllerForDismissedController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
            => new FromBottomTransition(false);
    }
}
