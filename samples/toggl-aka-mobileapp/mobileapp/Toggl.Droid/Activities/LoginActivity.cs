using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
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
    public sealed partial class LoginActivity : ReactiveActivity<LoginViewModel>
    {
        public LoginActivity() : base(
            Resource.Layout.LoginActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        { }

        public LoginActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }
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
                .Select(loginButtonTitle)
                .Subscribe(loginButton.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            //Visibility
            ViewModel.HasError
                .Subscribe(errorTextView.Rx().IsVisible(useGone: false))
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Subscribe(progressBar.Rx().IsVisible(useGone: false))
                .DisposedBy(DisposeBag);

            ViewModel.LoginEnabled
                .Subscribe(loginButton.Rx().Enabled())
                .DisposedBy(DisposeBag);

            //Commands
            signupCard.Rx()
                .BindAction(ViewModel.Signup)
                .DisposedBy(DisposeBag);

            loginButton.Rx().Tap()
                .Subscribe(ViewModel.Login)
                .DisposedBy(DisposeBag);

            passwordEditText.Rx().EditorActionSent()
                .Subscribe(ViewModel.Login)
                .DisposedBy(DisposeBag);

            googleLoginButton.Rx().Tap()
                .Subscribe(ViewModel.GoogleLogin)
                .DisposedBy(DisposeBag);

            forgotPasswordView.Rx()
                .BindAction(ViewModel.ForgotPassword)
                .DisposedBy(DisposeBag);

            string loginButtonTitle(bool isLoading)
                => isLoading ? "" : Shared.Resources.LoginTitle;

            this.CancelAllNotifications();
        }
    }
}
