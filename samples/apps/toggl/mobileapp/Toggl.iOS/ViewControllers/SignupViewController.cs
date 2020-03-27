using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.iOS.Extensions.LoginSignupViewExtensions;
using static Toggl.iOS.Extensions.ViewExtensions;
using AdjustBindingsiOS;
using CoreGraphics;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class SignupViewController : ReactiveViewController<SignupViewModel>
    {
        private const int iPhoneSeScreenHeight = 568;
        private const string adjustSignupEventToken = "b1qugc";

        private bool keyboardIsOpen = false;

        private const int topConstraintForBiggerScreens = 72;
        private const int topConstraintForBiggerScreensWithKeyboard = 40;

        private const int emailTopConstraint = 42;
        private const int emailTopConstraintWithKeyboard = 12;

        private const int tabletFormOffset = 246;
        private const int tabletLandscapeKeyboardOffset = 80;

        private UIButton showPasswordButton;

        public SignupViewController(SignupViewModel viewModel) : base(viewModel, nameof(SignupViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            EmailTextField.Placeholder = Resources.EmailAddress;
            PasswordTextField.Placeholder = Resources.Password;
            SignUpCardTitleLabel.Text = Resources.AlreadyHaveAnAccountQuestionMark;
            SignUpCardLoginLabel.Text = Resources.LoginTitle;
            OrLabel.Text = Resources.Or.ToUpper();
            GoogleSignupButton.SetTitle(Resources.GoogleSignUp, UIControlState.Normal);

            NavigationController.NavigationBarHidden = true;

            UIKeyboard.Notifications.ObserveWillShow(KeyboardWillShow);
            UIKeyboard.Notifications.ObserveWillHide(KeyboardWillHide);

            prepareViews();

            ViewModel.SuccessfulSignup
                .Subscribe(logAdjustSignupEvent)
                .DisposedBy(DisposeBag);

            //Text
            ViewModel.ErrorMessage
                .Subscribe(ErrorLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.Email
                .Subscribe(EmailTextField.Rx().TextObserver())
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

            ViewModel.IsLoading
                .Select(signupButtonTitle)
                .Subscribe(SignupButton.Rx().AnimatedTitle())
                .DisposedBy(DisposeBag);

            ViewModel.CountryButtonTitle
                .Subscribe(SelectCountryButton.Rx().AnimatedTitle())
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
                .Subscribe(showPasswordButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            PasswordTextField.FirstResponder
                .Subscribe(ViewModel.SetIsShowPasswordButtonVisible)
                .DisposedBy(DisposeBag);

            ViewModel.IsCountryErrorVisible
                .Subscribe(CountryNotSelectedImageView.Rx().AnimatedIsVisible())
                .DisposedBy(DisposeBag);

            //Commands
            LoginCard.Rx()
                .BindAction(ViewModel.Login)
                .DisposedBy(DisposeBag);

            SignupButton.Rx()
                .BindAction(ViewModel.Signup)
                .DisposedBy(DisposeBag);

            GoogleSignupButton.Rx()
                .BindAction(ViewModel.GoogleSignup)
                .DisposedBy(DisposeBag);

            showPasswordButton.Rx().Tap()
                .Subscribe(ViewModel.TogglePasswordVisibility)
                .DisposedBy(DisposeBag);

            SelectCountryButton.Rx()
                .BindAction(ViewModel.PickCountry)
                .DisposedBy(DisposeBag);

            //Color
            ViewModel.HasError
                .Select(signupButtonTintColor)
                .Subscribe(SignupButton.Rx().TintColor())
                .DisposedBy(DisposeBag);

            ViewModel.SignupEnabled
                .Select(signupButtonTitleColor)
                .Subscribe(SignupButton.Rx().TitleColor())
                .DisposedBy(DisposeBag);

            //Animation
            ViewModel.Shake
                .Subscribe(shakeTargets =>
                {
                    if (shakeTargets.HasFlag(SignupViewModel.ShakeTargets.Email))
                        EmailTextField.Shake();

                    if (shakeTargets.HasFlag(SignupViewModel.ShakeTargets.Password))
                        PasswordTextField.Shake();

                    if (shakeTargets.HasFlag(SignupViewModel.ShakeTargets.Country))
                        SelectCountryButton.Shake();
                })
                .DisposedBy(DisposeBag);

            UIColor signupButtonTintColor(bool hasError)
                => hasError ? UIColor.White : UIColor.Black;

            UIColor signupButtonTitleColor(bool enabled) => enabled
                ? Core.UI.Helper.Colors.Login.EnabledButtonColor.ToNativeColor()
                : Core.UI.Helper.Colors.Login.DisabledButtonColor.ToNativeColor();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (View.Frame.Height > iPhoneSeScreenHeight && !keyboardIsOpen)
                TopConstraint.Constant = topConstraintForBiggerScreens;

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && !keyboardIsOpen)
                TopConstraint.Constant = View.Frame.Height / 2 - tabletFormOffset;

            LoginCard.SetupBottomCard();
            GoogleSignupButton.SetupGoogleButton();
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

            showPasswordButton = new UIButton(new CGRect(0, 0, 40, PasswordTextField.Frame.Height));
            showPasswordButton.SetupShowPasswordButton();
            PasswordTextField.RightView = showPasswordButton;
            PasswordTextField.RightViewMode = UITextFieldViewMode.Always;

            ActivityIndicator.Alpha = 0;
            ActivityIndicator.StartSpinning();

            SignupButton.SetTitleColor(
                Core.UI.Helper.Colors.Login.DisabledButtonColor.ToNativeColor(),
                UIControlState.Disabled
            );

            EmailTextField.ShouldReturn += _ =>
            {
                PasswordTextField.BecomeFirstResponder();
                return false;
            };

            PasswordTextField.ShouldReturn += _ =>
            {
                ViewModel.Signup.Execute();
                PasswordTextField.ResignFirstResponder();
                return false;
            };

            setupKeyboardDismissingGestureRecognizers();
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

        private string signupButtonTitle(bool isLoading)
            => isLoading ? "" : Resources.SignUpTitle;

        private void logAdjustSignupEvent()
        {

#if USE_ANALYTICS
            var adjustEvent = AdjustBindingsiOS.ADJEvent.EventWithEventToken(adjustSignupEventToken);
            AdjustBindingsiOS.Adjust.TrackEvent(adjustEvent);
#endif
        }
    }
}

