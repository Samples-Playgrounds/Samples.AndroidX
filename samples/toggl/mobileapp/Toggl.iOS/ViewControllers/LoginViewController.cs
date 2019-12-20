using Foundation;
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
    public sealed partial class LoginViewController : ReactiveViewController<LoginViewModel>
    {
        private const int iPhoneSeScreenHeight = 568;

        private bool keyboardIsOpen = false;

        private const int topConstraintForBiggerScreens = 72;
        private const int topConstraintForBiggerScreensWithKeyboard = 40;

        private const int emailTopConstraint = 42;
        private const int emailTopConstraintWithKeyboard = 12;

        private const int tabletFormOffset = 246;
        private const int tabletLandscapeKeyboardOffset = 90;

        public static LoginViewController NewInstance(LoginViewModel viewModel)
        {
            var storyboard = UIStoryboard.FromName("Login", null);
            var instance = storyboard.InstantiateViewController(nameof(LoginViewController)) as LoginViewController;
            instance.ViewModel = viewModel;
            return instance;
        }

        public LoginViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            EmailTextField.Placeholder = Resources.EmailAddress;
            PasswordTextField.Placeholder = Resources.Password;
            OrLabel.Text = Resources.Or.ToUpper();
            LoginButton.SetTitle(Resources.LoginTitle, UIControlState.Normal);
            GoogleLoginButton.SetTitle(Resources.GoogleLogin, UIControlState.Normal);
            DontHaveAnAccountLabel.Text = Resources.DoNotHaveAnAccountWithQuestionMark;
            SignUpForFreeLabel.Text = Resources.SignUpTitle;

            NavigationController.NavigationBarHidden = true;

            UIKeyboard.Notifications.ObserveWillShow(KeyboardWillShow);
            UIKeyboard.Notifications.ObserveWillHide(KeyboardWillHide);

            //Text
            ViewModel.Email
                .Subscribe(EmailTextField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.ErrorMessage
                .Subscribe(ErrorLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.Password
                .Subscribe(PasswordTextField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            EmailTextField.Rx().Text()
                .Select(Email.From)
                .Subscribe(ViewModel.SetEmail)
                .DisposedBy(DisposeBag);

            PasswordTextField.Rx().Text()
                .Select(Password.From)
                .Subscribe(ViewModel.SetPassword)
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading.Select(loginButtonTitle)
                .Subscribe(LoginButton.Rx().AnimatedTitle())
                .DisposedBy(DisposeBag);

            //Visibility
            ViewModel.HasError
                .Subscribe(ErrorLabel.Rx().AnimatedIsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Subscribe(ActivityIndicator.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);

            ViewModel.IsPasswordMasked
                .Skip(1)
                .Subscribe(PasswordTextField.Rx().SecureTextEntry())
                .DisposedBy(DisposeBag);

            ViewModel.IsShowPasswordButtonVisible
                .Subscribe(ShowPasswordButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            PasswordTextField.FirstResponder
                .Subscribe(ViewModel.SetIsShowPasswordButtonVisible)
                .DisposedBy(DisposeBag);

            //Commands
            SignupCard.Rx()
                .BindAction(ViewModel.Signup)
                .DisposedBy(DisposeBag);

            LoginButton.Rx().Tap()
                .Subscribe(ViewModel.Login)
                .DisposedBy(DisposeBag);

            GoogleLoginButton.Rx().Tap()
                .Subscribe(ViewModel.GoogleLogin)
                .DisposedBy(DisposeBag);

            ForgotPasswordButton.Rx()
                .BindAction(ViewModel.ForgotPassword)
                .DisposedBy(DisposeBag);

            ShowPasswordButton.Rx().Tap()
                .Subscribe(ViewModel.TogglePasswordVisibility)
                .DisposedBy(DisposeBag);

            //Color
            ViewModel.HasError
                .Select(loginButtonTintColor)
                .Subscribe(LoginButton.Rx().TintColor())
                .DisposedBy(DisposeBag);

            ViewModel.LoginEnabled
                .Select(loginButtonTitleColor)
                .Subscribe(LoginButton.Rx().TitleColor())
                .DisposedBy(DisposeBag);

            //Animation
            ViewModel.Shake
                .Subscribe(shakeTargets =>
                {
                    if (shakeTargets.HasFlag(LoginViewModel.ShakeTargets.Email))
                        EmailTextField.Shake();

                    if (shakeTargets.HasFlag(LoginViewModel.ShakeTargets.Password))
                        PasswordTextField.Shake();
                })
                .DisposedBy(DisposeBag);

            prepareViews();

            UIColor loginButtonTintColor(bool hasError)
                => hasError ? UIColor.White : UIColor.Black;

            UIColor loginButtonTitleColor(bool enabled) => enabled
                ? UIColor.White
                : UIColor.White.ColorWithAlpha((nfloat) 0.5);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController?.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController?.SetNavigationBarHidden(false, true);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (View.Frame.Height > iPhoneSeScreenHeight && !keyboardIsOpen)
                TopConstraint.Constant = topConstraintForBiggerScreens;

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && !keyboardIsOpen)
                TopConstraint.Constant = View.Frame.Height / 2 - tabletFormOffset;

            SignupCard.SetupBottomCard();
            GoogleLoginButton.SetupGoogleButton();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ActivityIndicator.Alpha = 0;
            ActivityIndicator.StartSpinning();
            PasswordTextField.ResignFirstResponder();
        }

        private void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            keyboardIsOpen = true;
            if (View.Frame.Height <= iPhoneSeScreenHeight)
            {
                EmailFieldTopConstraint.Constant = emailTopConstraintWithKeyboard;
            }
            else if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                var keyboardOffset = UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait ||
                                     UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown
                                     ? 0
                                     : tabletLandscapeKeyboardOffset;
                TopConstraint.Constant = View.Frame.Height / 2 - tabletFormOffset - keyboardOffset;
            }
            else
            {
                TopConstraint.Constant = topConstraintForBiggerScreensWithKeyboard;
            }
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        private void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            keyboardIsOpen = false;
            if (View.Frame.Height <= iPhoneSeScreenHeight)
            {
                EmailFieldTopConstraint.Constant = emailTopConstraint;
            }
            else if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                TopConstraint.Constant = View.Frame.Height / 2 - tabletFormOffset;
            }
            else
            {
                TopConstraint.Constant = topConstraintForBiggerScreens;
            }
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        private void prepareViews()
        {
            NavigationController.NavigationBarHidden = true;

            LoginButton.SetTitleColor(
                UIColor.White.ColorWithAlpha((nfloat)0.5),
                UIControlState.Disabled
            );

            EmailTextField.ShouldReturn += _ =>
            {
                PasswordTextField.BecomeFirstResponder();
                return false;
            };

            PasswordTextField.ShouldReturn += _ =>
            {
                ViewModel.Login();
                PasswordTextField.ResignFirstResponder();
                return false;
            };

            setupKeyboardDismissingGestureRecognizers();

            prepareForgotPasswordButton();
            ShowPasswordButton.SetupShowPasswordButton();
        }

        private void setupKeyboardDismissingGestureRecognizers()
        {
            void dismissKeyboard()
            {
                EmailTextField.ResignFirstResponder();
                PasswordTextField.ResignFirstResponder();
            }

            View.AddGestureRecognizer(new UITapGestureRecognizer(dismissKeyboard));

            View.AddGestureRecognizer(new UIPanGestureRecognizer((recognizer) =>
            {
                if (recognizer.TranslationInView(View).Y > 0)
                    dismissKeyboard();
            }));
        }

        private void prepareForgotPasswordButton()
        {
            var boldFont = UIFont.SystemFontOfSize(12, UIFontWeight.Medium);
            var color = ColorAssets.Text1;
            var text = new NSMutableAttributedString(
                Resources.LoginForgotPassword, foregroundColor: color);
            var boldText = new NSAttributedString(
                Resources.LoginGetHelpLoggingIn,
                foregroundColor: color,
                font: boldFont);
            text.Append(boldText);
            ForgotPasswordButton.SetAttributedTitle(text, UIControlState.Normal);
        }

        private string loginButtonTitle(bool isLoading)
            => isLoading ? "" : Resources.LoginTitle;
    }
}
