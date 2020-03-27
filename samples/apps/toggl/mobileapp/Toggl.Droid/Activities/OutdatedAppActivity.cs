using Android.App;
using Android.Content.PM;
using Android.Runtime;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class OutdatedAppActivity : ReactiveActivity<OutdatedAppViewModel>
    {
        public OutdatedAppActivity() : base(
            Resource.Layout.OutdatedAppActivity,
            Resource.Style.AppTheme_OutdatedAppStatusBarColor,
            Transitions.Fade)
        { }

        public OutdatedAppActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            updateAppButton.Rx()
                .BindAction(ViewModel.UpdateApp)
                .DisposedBy(DisposeBag);

            openWebsiteButton.Rx()
                .BindAction(ViewModel.OpenWebsite)
                .DisposedBy(DisposeBag);
        }
    }
}
