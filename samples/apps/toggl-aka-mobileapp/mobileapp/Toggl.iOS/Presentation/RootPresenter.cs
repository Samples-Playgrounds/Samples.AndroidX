using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.iOS.ViewControllers;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public sealed class RootPresenter : IosPresenter
    {
        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(MainTabBarViewModel),
            typeof(LoginViewModel),
            typeof(SignupViewModel),
            typeof(TokenResetViewModel),
            typeof(OutdatedAppViewModel),
        };

        private HashSet<Type> viewModelsNotWrappedInNavigationController { get; } = new HashSet<Type>
        {
            typeof(MainTabBarViewModel),
            typeof(OutdatedAppViewModel),
        };

        public RootPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView view)
        {
            var rootViewController = !viewModelsNotWrappedInNavigationController.Contains(viewModel.GetType())
                ? ViewControllerLocator.GetNavigationViewController(viewModel)
                : ViewControllerLocator.GetViewController(viewModel);

            var oldRootViewController = Window.RootViewController;
            Window.RootViewController = rootViewController;

            UIView.Transition(
                Window,
                Animation.Timings.EnterTiming,
                UIViewAnimationOptions.TransitionCrossDissolve,
                () => { },
                () => detachOldRootViewController(oldRootViewController)
            );
        }

        public override bool ChangePresentation(IPresentationChange presentationChange)
        {
            var rootViewController = Window.RootViewController;
            if (rootViewController is MainTabBarController mainTabBarController)
            {
                switch (presentationChange)
                {
                    case ShowReportsPresentationChange showReportsPresentationChange:
                        mainTabBarController.SelectedIndex = 1;
                        var navigationController = mainTabBarController.SelectedViewController as UINavigationController;
                        var reportsViewController = navigationController.ViewControllers.First() as ReportsViewController;
                        var reportsViewModel = reportsViewController.ViewModel;

                        var startDate = showReportsPresentationChange.StartDate;
                        var endDate = showReportsPresentationChange.EndDate;
                        var period = showReportsPresentationChange.Period;
                        var workspaceId = showReportsPresentationChange.WorkspaceId;

                        if (startDate.HasValue && endDate.HasValue)
                        {
                            reportsViewModel.LoadReport(workspaceId, startDate.Value, endDate.Value, ReportsSource.Other);
                        }
                        else if (period.HasValue)
                        {
                            reportsViewModel.LoadReport(workspaceId, period.Value);
                        }

                        return true;

                    case ShowCalendarPresentationChange _:
                        mainTabBarController.SelectedIndex = 2;
                        return true;
                }
            }

            return false;
        }

        private void detachOldRootViewController(UIViewController viewController)
        {
            var viewControllerToDetach = viewController is UINavigationController navigationController
                ? navigationController.ViewControllers.First()
                : viewController;

            switch (viewControllerToDetach)
            {
                case MainTabBarController mainTabBarController:
                    detachViewModel(mainTabBarController.ViewModel);
                    break;
                case LoginViewController loginViewController:
                    detachViewModel(loginViewController.ViewModel);
                    break;
                case SignupViewController signupViewController:
                    detachViewModel(signupViewController.ViewModel);
                    break;
                case TokenResetViewController tokenResetViewController:
                    detachViewModel(tokenResetViewController.ViewModel);
                    break;
                case OutdatedAppViewController outdatedAppViewController:
                    detachViewModel(outdatedAppViewController.ViewModel);
                    break;
            }
        }

        private void detachViewModel<TViewModel>(TViewModel viewModel)
            where TViewModel : IViewModel
        {
            viewModel?.DetachView();
            viewModel?.CloseWithDefaultResult();
            viewModel?.ViewDestroyed();
        }
    }
}
