using Android.App;
using Android.Content.PM;
using Android.Runtime;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class TokenResetActivity : ReactiveActivity<TokenResetViewModel>
    {
        public TokenResetActivity() : base(
            Resource.Layout.TokenResetActivity,
            Resource.Style.AppTheme,
            Transitions.Fade)
        { }

        public TokenResetActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            this.CancelAllNotifications();

            emailLabel.Text = ViewModel.Email.ToString();

            passwordEditText
                .Rx().Text()
                .Subscribe(ViewModel.Password)
                .DisposedBy(DisposeBag);

            ViewModel.Done.Executing
                .Subscribe(progressBar.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            signoutLabel.Rx()
                .BindAction(ViewModel.SignOut)
                .DisposedBy(DisposeBag);

            doneButton.Rx()
                .BindAction(ViewModel.Done)
                .DisposedBy(DisposeBag);
        }
    }
}
