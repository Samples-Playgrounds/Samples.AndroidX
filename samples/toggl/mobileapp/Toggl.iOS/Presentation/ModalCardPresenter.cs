using System;
using System.Collections.Generic;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using Toggl.iOS.Presentation.Transition;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public sealed class ModalCardPresenter : IosPresenter
    {
        private readonly FromBottomTransitionDelegate fromBottomTransitionDelegate = new FromBottomTransitionDelegate();

        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(EditDurationViewModel),
            typeof(EditProjectViewModel),
            typeof(EditTimeEntryViewModel),
            typeof(SelectClientViewModel),
            typeof(SelectCountryViewModel),
            typeof(SelectProjectViewModel),
            typeof(SelectTagsViewModel),
            typeof(SelectColorViewModel),
            typeof(SelectDateTimeViewModel),
            typeof(SendFeedbackViewModel),
            typeof(StartTimeEntryViewModel),
            typeof(UpcomingEventsNotificationSettingsViewModel),
        };

        public ModalCardPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            UIViewController viewController = ViewControllerLocator.GetViewController(viewModel);

            viewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            viewController.TransitioningDelegate = fromBottomTransitionDelegate;

            UIViewController topmostViewController = FindPresentedViewController();
            topmostViewController.PresentViewController(viewController, true, null);
        }
    }
}
