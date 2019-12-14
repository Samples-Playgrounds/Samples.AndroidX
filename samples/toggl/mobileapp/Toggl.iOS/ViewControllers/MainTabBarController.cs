using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Extensions;
using Toggl.iOS.Presentation;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public class MainTabBarController : UITabBarController
    {
        public MainTabBarViewModel ViewModel { get; set; }

        private static readonly Dictionary<Type, string> imageNameForType = new Dictionary<Type, string>
        {
            { typeof(MainViewModel), "icTime" },
            { typeof(ReportsViewModel), "icReports" },
            { typeof(CalendarViewModel), "icCalendar" },
            { typeof(SettingsViewModel), "icSettings" }
        };

        private static readonly Dictionary<Type, string> accessibilityLabels = new Dictionary<Type, string>
        {
            { typeof(MainViewModel), Resources.Timer },
            { typeof(ReportsViewModel), Resources.Reports },
            { typeof(CalendarViewModel), Resources.Calendar },
            { typeof(SettingsViewModel), Resources.Settings }
        };

        public MainTabBarController(MainTabBarViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewControllers = ViewModel.Tabs.Select(createTabFor).ToArray();

            UIViewController createTabFor(ViewModel childViewModel)
            {
                var viewController = ViewControllerLocator.GetNavigationViewController(childViewModel);
                var childViewModelType = childViewModel.GetType();
                var item = new UITabBarItem();
                item.Title = "";
                item.Image = UIImage.FromBundle(imageNameForType[childViewModelType]);
                item.AccessibilityLabel = accessibilityLabels[childViewModelType];
                viewController.TabBarItem = item;
                return viewController;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            recalculateTabBarInsets();
            setupAppearance();
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            recalculateTabBarInsets();
            setupAppearance();
        }

        public override void ItemSelected(UITabBar tabbar, UITabBarItem item)
        {
            var targetViewController = ViewControllers.Single(vc => vc.TabBarItem == item);

            if (targetViewController == SelectedViewController
                && tryGetScrollableController() is IScrollableToTop scrollable)
            {
                scrollable.ScrollToTop();
            }
            else if (targetViewController is ReactiveNavigationController navigationController)
            {
                if (navigationController.TopViewController is IReactiveViewController reactiveViewController)
                    reactiveViewController.DismissFromNavigationController();
            }

            UIViewController tryGetScrollableController()
            {
                if (targetViewController is IScrollableToTop)
                    return targetViewController;

                if (targetViewController is UINavigationController nav)
                    return nav.TopViewController;

                return null;
            }
        }

        private void recalculateTabBarInsets()
        {
            ViewControllers.ToList()
                           .ForEach(vc =>
            {
                if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact && !UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    // older devices render tab bar item insets weirdly
                    vc.TabBarItem.ImageInsets = new UIEdgeInsets(6, 0, -6, 0);
                }
                else
                {
                    vc.TabBarItem.ImageInsets = new UIEdgeInsets(0, 0, 0, 0);
                }
            });
        }

        private void setupAppearance()
        {
            TabBar.BackgroundImage = ImageExtension.ImageWithColor(ColorAssets.Background);
            TabBar.SelectedImageTintColor = Colors.TabBar.SelectedImageTintColor.ToNativeColor();
            TabBarItem.TitlePositionAdjustment = new UIOffset(0, 200);
        }
    }
}
