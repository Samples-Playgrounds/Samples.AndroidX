#if !USE_PRODUCTION_API
using System;
using System.Reactive.Disposables;
using CoreGraphics;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.DebugHelpers
{
    public class ErrorTriggeringViewController : UIViewController
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Layer.CornerRadius = 3;
            View.BackgroundColor = UIColor.White;

            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                // ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = 233
            };

            var tokenReset = createButton("Token reset error");
            var noWorkspace = createButton("No workspace error");
            var noDefaultWorkspace = createButton("No default workspace error");
            var outdatedApp = createButton("Outdated client error");
            var outdatedApi = createButton("Outdated API error");
            var outdatedAppPermanently = createButton("Permanent outdated client error");
            var outdatedApiPermanently = createButton("Permanent outdated API error");

            tokenReset.Rx().Tap()
                .Subscribe(dismissAndThenRun(tokenResetErrorTriggered))
                .DisposedBy(disposeBag);

            noWorkspace.Rx().Tap()
                .Subscribe(dismissAndThenRun(noWorkspaceErrorTriggered))
                .DisposedBy(disposeBag);

            noDefaultWorkspace.Rx().Tap()
                .Subscribe(dismissAndThenRun(noDefaultWorkspaceErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedApp.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedAppErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedApi.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedApiErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedAppPermanently.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedAppPermanentlyErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedApiPermanently.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedApiPermanentlyErrorTriggered))
                .DisposedBy(disposeBag);

            var buttons = new[]
            {
                tokenReset,
                noWorkspace,
                noDefaultWorkspace,
                outdatedApp,
                outdatedApi,
                outdatedAppPermanently,
                outdatedApiPermanently
            };

            View.Add(new UIStackView(buttons)
            {
                BackgroundColor = UIColor.Red,
                Axis = UILayoutConstraintAxis.Vertical,
                Distribution = UIStackViewDistribution.FillEqually,
                Frame = new CGRect(20, 20, PreferredContentSize.Width - 40, PreferredContentSize.Height - 40)
            });
        }

        private UIButton createButton(string text)
        {
            var button = new UIButton();
            button.SetTitle(text, UIControlState.Normal);
            button.SetTitleColor(UIColor.Black, UIControlState.Normal);
            return button;
        }

        private Action dismissAndThenRun(Action callback)
            => () =>
            {
                this.Dismiss();
                callback?.Invoke();
            };

        private void tokenResetErrorTriggered()
        {
            var container = IosDependencyContainer.Instance;
            container.NavigationService.Navigate<TokenResetViewModel>(null);
        }

        private void noWorkspaceErrorTriggered()
        {
            var container = IosDependencyContainer.Instance;
            container.NavigationService.Navigate<NoWorkspaceViewModel>(null);
        }

        private void noDefaultWorkspaceErrorTriggered()
        {
            var container = IosDependencyContainer.Instance;
            container.NavigationService.Navigate<SelectDefaultWorkspaceViewModel>(null);
        }

        private void outdatedAppErrorTriggered()
        {
            var container = IosDependencyContainer.Instance;
            container.NavigationService.Navigate<OutdatedAppViewModel>(null);
        }

        private void outdatedApiErrorTriggered()
        {
            var container = IosDependencyContainer.Instance;
            container.NavigationService.Navigate<OutdatedAppViewModel>(null);
        }

        private void outdatedAppPermanentlyErrorTriggered()
        {
            var container = IosDependencyContainer.Instance;
            container.AccessRestrictionStorage.SetApiOutdated();

            outdatedAppErrorTriggered();
        }

        private void outdatedApiPermanentlyErrorTriggered()
        {
            var container = IosDependencyContainer.Instance;
            container.AccessRestrictionStorage.SetClientOutdated();
            outdatedApiErrorTriggered();
        }
    }
}
#endif
