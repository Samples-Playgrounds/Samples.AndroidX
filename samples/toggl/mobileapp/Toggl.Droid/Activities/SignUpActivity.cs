using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class SignUpActivity : ReactiveActivity<SignupViewModel>
    {
        public SignUpActivity() : base(
            Resource.Layout.SignUpActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        { }

        public SignUpActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }
        
        protected override void InitializeBindings()
        {
            ViewModel.Email.FirstAsync()
                .SubscribeOn(AndroidDependencyContainer.Instance.SchedulerProvider.MainScheduler)
                .Subscribe(emailEditText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Password.FirstAsync()
                .SubscribeOn(AndroidDependencyContainer.Instance.SchedulerProvider.MainScheduler)
                .Subscribe(passwordEditText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            //Text
            ViewModel.ErrorMessage
                .Subscribe(errorTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            emailEditText.Rx().Text()
                .Select(Email.From)
                .Subscribe(ViewModel.SetEmail)
                .DisposedBy(DisposeBag);

            passwordEditText.Rx().Text()
                .Select(Password.From)
                .Subscribe(ViewModel.SetPassword)
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Select(signupButtonTitle)
                .Subscribe(signupButton.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.CountryButtonTitle
                .Subscribe(countryNameTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            //Visibility
            ViewModel.HasError
                .Subscribe(errorTextView.Rx().IsVisible(useGone: false))
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Subscribe(progressBar.Rx().IsVisible(useGone: false))
                .DisposedBy(DisposeBag);

            ViewModel.SignupEnabled
                .Subscribe(signupButton.Rx().Enabled())
                .DisposedBy(DisposeBag);

            ViewModel.IsCountryErrorVisible
                .Subscribe(countryErrorView.Rx().IsVisible(useGone: false))
                .DisposedBy(DisposeBag);

            //Commands
            loginCard.Rx().Tap()
                .Subscribe(ViewModel.Login.Inputs)
                .DisposedBy(DisposeBag);

            signupButton.Rx().Tap()
                .Subscribe(ViewModel.Signup.Inputs)
                .DisposedBy(DisposeBag);

            passwordEditText.Rx().EditorActionSent()
                .Subscribe(ViewModel.Signup.Inputs)
                .DisposedBy(DisposeBag);

            googleSignupButton.Rx().Tap()
                .Subscribe(ViewModel.GoogleSignup.Inputs)
                .DisposedBy(DisposeBag);

            countrySelection.Rx().Tap()
                .Subscribe(ViewModel.PickCountry.Inputs)
                .DisposedBy(DisposeBag);

            string signupButtonTitle(bool isLoading)
                => isLoading ? "" : Shared.Resources.SignUpTitle;
        }
    }
}
