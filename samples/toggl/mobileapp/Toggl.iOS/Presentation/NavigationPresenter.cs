using System;
using System.Collections.Generic;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public sealed class NavigationPresenter : IosPresenter
    {
        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(AboutViewModel),
            typeof(CalendarSettingsViewModel),
            typeof(ForgotPasswordViewModel),
            typeof(LicensesViewModel),
            typeof(NotificationSettingsViewModel),
            typeof(SiriShortcutsCustomTimeEntryViewModel),
            typeof(SiriShortcutsSelectReportPeriodViewModel),
            typeof(SiriShortcutsViewModel),
            typeof(SiriWorkflowsViewModel),
            typeof(SyncFailuresViewModel),
        };

        public NavigationPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView view)
        {
            UIViewController viewController = ViewControllerLocator.GetViewController(viewModel);

            var presentedViewController = FindPresentedViewController();

            if (tryPushOnViewController(presentedViewController, viewController))
                return;

            if (presentedViewController is UITabBarController tabBarController)
            {
                var selectedController = tabBarController.SelectedViewController;

                if (!tryPushOnViewController(selectedController, viewController))
                    throw new Exception($"Failed to find a navigation controller to present view controller of type {viewController.GetType().Name}");
            }
        }

        private bool tryPushOnViewController(UIViewController parentViewController,
            UIViewController childViewController)
        {
            if (parentViewController is UINavigationController navigationController)
            {
                navigationController.PushViewController(childViewController, true);
                return true;
            }

            return false;
        }
    }
}
