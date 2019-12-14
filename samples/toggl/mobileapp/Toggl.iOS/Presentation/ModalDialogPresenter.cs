using System;
using System.Collections.Generic;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Settings.Siri;
using Toggl.Core.UI.Views;
using Toggl.iOS.Presentation.Transition;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public sealed class ModalDialogPresenter : IosPresenter
    {
        private readonly ModalDialogTransitionDelegate modalTransitionDelegate = new ModalDialogTransitionDelegate();

        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(CalendarPermissionDeniedViewModel),
            typeof(NoWorkspaceViewModel),
            typeof(PasteFromClipboardViewModel),
            typeof(SelectDefaultWorkspaceViewModel),
            typeof(TermsOfServiceViewModel),
            typeof(January2020CampaignViewModel),
        };

        public ModalDialogPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            UIViewController viewController = ViewControllerLocator.GetViewController(viewModel);

            viewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            viewController.TransitioningDelegate = modalTransitionDelegate;

            UIViewController topmostViewController = FindPresentedViewController();
            topmostViewController.PresentViewController(viewController, true, null);
        }
    }
}
