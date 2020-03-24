using System;
using System.Linq;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public class ReactiveNavigationController : UINavigationController
    {
        private UIColor barBackgroundColor = ColorAssets.Background;

        private static UITextAttributes barButtonTextAttributes = new UITextAttributes
        {
            Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium),
            TextColor = ColorAssets.Text2
        };

        private static UIStringAttributes titleTextAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium),
            ForegroundColor = ColorAssets.Text
        };

        public ReactiveNavigationController(UIViewController rootViewController) : base(rootViewController)
        {
        }

        public static UIBarButtonItem CreateSystemItem(UIBarButtonSystemItem systemItem, Action action)
        {
            var button = new UIBarButtonItem(
                systemItem,
                (sender, args) => action()
            );

            button.SetTitleTextAttributes(barButtonTextAttributes, UIControlState.Normal);
            button.SetTitleTextAttributes(barButtonTextAttributes, UIControlState.Highlighted);

            return button;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            setupAppearance();
        }

        public override UIViewController PopViewController(bool animated)
        {
            var viewControllerToPop = ViewControllers.Last();
            if (viewControllerToPop is IReactiveViewController reactiveViewController)
            {
                reactiveViewController.ViewcontrollerWasPopped();
            }

            return base.PopViewController(animated);
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            viewController.NavigationItem.BackBarButtonItem = new UIBarButtonItem();

            var backButton = new UIBarButtonItem(
                "Back",
                 UIBarButtonItemStyle.Done,
                (sender, args) => { viewController.NavigationController.PopViewController(true); }
            );

            backButton.SetTitleTextAttributes(barButtonTextAttributes, UIControlState.Normal);
            backButton.SetTitleTextAttributes(barButtonTextAttributes, UIControlState.Highlighted);

            viewController.NavigationItem.BackBarButtonItem = backButton;
            viewController.NavigationItem.BackBarButtonItem?.SetBackgroundVerticalPositionAdjustment(6, UIBarMetrics.Default);
        }

        public void SetBackgroundColor(UIColor color)
        {
            barBackgroundColor = color;
            setupAppearance();
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            setupAppearance();
        }

        private void setupAppearance()
        {
            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.BarTintColor = barBackgroundColor;
            NavigationBar.BackgroundColor = barBackgroundColor;
            NavigationBar.TintColor = ColorAssets.Text2;
            NavigationBar.SetBackgroundImage(ImageExtension.ImageWithColor(barBackgroundColor), UIBarMetrics.Default);

            NavigationBar.TitleTextAttributes = titleTextAttributes;

            var image = UIImage.FromBundle("icBackNoPadding");
            NavigationBar.BackIndicatorImage = image;
            NavigationBar.BackIndicatorTransitionMaskImage = image;
        }
    }
}
