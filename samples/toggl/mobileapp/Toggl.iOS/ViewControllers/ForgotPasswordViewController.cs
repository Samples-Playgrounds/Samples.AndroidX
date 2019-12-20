using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class ForgotPasswordViewController
        : KeyboardAwareViewController<ForgotPasswordViewModel>
    {
        private const int backButtonFontSize = 14;
        private const int resetButtonBottomSpacing = 32;

        private bool viewInitialized;

        public ForgotPasswordViewController(ForgotPasswordViewModel viewModel)
            : base(viewModel, nameof(ForgotPasswordViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.LoginForgotPassword;
            ResetPasswordButton.SetTitle(Resources.GetPasswordResetLink, UIControlState.Normal);
            EmailTextField.Placeholder = Resources.EmailAddress;
            SuccessMessageLabel.Text = Resources.PasswordResetSuccess;

            prepareViews();

            //Text
            ViewModel.ErrorMessage
                .Subscribe(errorMessage =>
                {
                    ErrorLabel.Text = errorMessage;
                    ErrorLabel.Hidden = string.IsNullOrEmpty(errorMessage);
                })
                .DisposedBy(DisposeBag);

            EmailTextField.Rx().Text()
                .Select(Email.From)
                .Subscribe(ViewModel.Email.OnNext)
                .DisposedBy(DisposeBag);

            ViewModel.Reset.Executing
                .Subscribe(loading =>
                {
                    UIView.Transition(
                        ResetPasswordButton,
                        Animation.Timings.EnterTiming,
                        UIViewAnimationOptions.TransitionCrossDissolve,
                        () => ResetPasswordButton.SetTitle(loading ? "" : Resources.GetPasswordResetLink, UIControlState.Normal),
                        null
                    );
                })
                .DisposedBy(DisposeBag);

            //Visibility
            ViewModel.PasswordResetSuccessful
                .Subscribe(DoneCard.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);

            ViewModel.PasswordResetSuccessful
                .Invert()
                .Subscribe(ResetPasswordButton.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);

            ViewModel.PasswordResetSuccessful
                .Where(s => s == false)
                .Subscribe(_ => EmailTextField.BecomeFirstResponder())
                .DisposedBy(DisposeBag);

            ViewModel.PasswordResetSuccessful
                .Where(successful => successful)
                .Subscribe(_ => EmailTextField.ResignFirstResponder())
                .DisposedBy(DisposeBag);

            ViewModel.Reset.Executing
                .Subscribe(ActivityIndicator.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);

            //Colors
            ViewModel.EmailValid
                .Select(resetButtonTitleColor)
                .Subscribe(ResetPasswordButton.Rx().TitleColor());

            //Commands
            ResetPasswordButton.Rx()
                .BindAction(ViewModel.Reset)
                .DisposedBy(DisposeBag);

            ViewModel.PasswordResetWithInvalidEmail
                .Subscribe(_ => EmailTextField.Shake())
                .DisposedBy(DisposeBag);

                UIColor resetButtonTitleColor(bool enabled) => enabled
                    ? Colors.Login.EnabledButtonColor.ToNativeColor()
                    : Colors.Login.DisabledButtonColor.ToNativeColor();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (viewInitialized) return;

            viewInitialized = true;
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            ResetPasswordButtonBottomConstraint.Constant = e.FrameEnd.Height + resetButtonBottomSpacing;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            ResetPasswordButtonBottomConstraint.Constant = resetButtonBottomSpacing;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        private void prepareViews()
        {
            NavigationController.NavigationBarHidden = false;

            ResetPasswordButton.SetTitleColor(
                Core.UI.Helper.Colors.Login.DisabledButtonColor.ToNativeColor(),
                UIControlState.Disabled
            );

            EmailTextField.BecomeFirstResponder();
            EmailTextField.Rx().ShouldReturn()
                .Subscribe(ViewModel.Reset.Inputs)
                .DisposedBy(DisposeBag);

            ActivityIndicator.StartSpinning();

            ErrorLabel.Hidden = true;
        }
    }
}
