using System;
using System.Collections.Generic;
using Foundation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public sealed class PageSheetPresenter : IosPresenter
    {
        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(SettingsViewModel),
        };

        public PageSheetPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            UIViewController viewController = ViewControllerLocator.GetNavigationViewController(viewModel);

            viewController.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
            viewController.PresentationController.Delegate = new PresentationControllerDelegate(
                () => viewModel.CloseWithDefaultResult());

            UIViewController topmostViewController = FindPresentedViewController();
            topmostViewController.PresentViewController(viewController, true, null);
        }
    }

    public sealed class PresentationControllerDelegate : NSObject, IUIAdaptivePresentationControllerDelegate
    {
        private Action closedAction;

        public PresentationControllerDelegate(Action closedAction)
        {
            this.closedAction = closedAction;
        }

        [Export("presentationControllerDidDismiss:")]
        public void DidDismiss(UIPresentationController presentationController)
        {
            closedAction.Invoke();
        }
    }
}
