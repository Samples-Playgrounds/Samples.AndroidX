using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class TokenResetViewController : ReactiveViewController<TokenResetViewModel>
    {
        public TokenResetViewController(TokenResetViewModel viewModel)
            : base(viewModel, nameof(TokenResetViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.LoginTitle;
            ResetSuccessLabel.Text = Resources.APITokenResetSuccess;
            InstructionLabel.Text = Resources.TokenResetInstruction;
            PasswordTextField.Placeholder = Resources.Password;
            SignOutButton.SetTitle(Resources.OrSignOut, UIControlState.Normal);

            EmailLabel.Text = ViewModel.Email.ToString();

            ViewModel.Error
                .Subscribe(ErrorLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.Password
                .Subscribe(PasswordTextField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            PasswordTextField.Rx().Text()
                .Subscribe(ViewModel.Password)
                .DisposedBy(DisposeBag);

            SignOutButton.Rx()
                .BindAction(ViewModel.SignOut)
                .DisposedBy(DisposeBag);

            ViewModel.SignOut.Elements
                .Subscribe(IosDependencyContainer.Instance.IntentDonationService.ClearAll)
                .DisposedBy(DisposeBag);

            ShowPasswordButton.Rx().Tap()
                .Subscribe(_ =>
                {
                    PasswordTextField.ResignFirstResponder();
                    PasswordTextField.SecureTextEntry = !PasswordTextField.SecureTextEntry;
                    PasswordTextField.BecomeFirstResponder();
                })
                .DisposedBy(DisposeBag);

            LoginButton.Rx().Tap()
                .Subscribe(ViewModel.Done.Inputs)
                .DisposedBy(DisposeBag);

            PasswordTextField.Rx().ShouldReturn()
                .Subscribe(ViewModel.Done.Inputs)
                .DisposedBy(DisposeBag);

            //Enabled
            ViewModel.NextIsEnabled
                .Subscribe(LoginButton.Rx().Enabled())
                .DisposedBy(DisposeBag);

            //Visibility
            ErrorLabel.Hidden = true;
            ViewModel.HasError
                .Subscribe(ErrorLabel.Rx().AnimatedIsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.Done.Executing
                .Invert()
                .Subscribe(ShowPasswordButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.Done.Executing
                .Subscribe(ActivityIndicatorView.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);
            
            ViewModel.Done.Executing.Select(loginButtonTitle)
                .Subscribe(LoginButton.Rx().AnimatedTitle())
                .DisposedBy(DisposeBag);

            PasswordTextField.BecomeFirstResponder();
            ShowPasswordButton.SetupShowPasswordButton();
            
            //Color
            ViewModel.HasError
                .Select(loginButtonTintColor)
                .Subscribe(LoginButton.Rx().TintColor())
                .DisposedBy(DisposeBag);

            ViewModel.NextIsEnabled
                .Select(loginButtonTitleColor)
                .Subscribe(LoginButton.Rx().TitleColor())
                .DisposedBy(DisposeBag);
            
            UIColor loginButtonTintColor(bool hasError)
                => hasError ? UIColor.White : UIColor.Black;

            UIColor loginButtonTitleColor(bool enabled) => enabled
                ? Core.UI.Helper.Colors.Login.EnabledButtonColor.ToNativeColor()
                : Core.UI.Helper.Colors.Login.DisabledButtonColor.ToNativeColor();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ActivityIndicatorView.Alpha = 0;
            ActivityIndicatorView.StartSpinning();
            PasswordTextField.ResignFirstResponder();
        }

        private string loginButtonTitle(bool isLoading)
            => isLoading ? "" : Resources.LoginTitle;
    }
}

