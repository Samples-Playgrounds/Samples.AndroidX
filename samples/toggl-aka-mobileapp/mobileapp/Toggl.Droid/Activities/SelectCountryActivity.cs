using Android.App;
using Android.Content.PM;
using Android.Runtime;
using System;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class SelectCountryActivity : ReactiveActivity<SelectCountryViewModel>
    {
        public SelectCountryActivity() : base(
            Resource.Layout.SelectCountryActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromRight)
        { }

        public SelectCountryActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            ViewModel.Countries
                .Subscribe(recyclerAdapter.Rx().Items())
                .DisposedBy(DisposeBag);

            filterEditText.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            recyclerAdapter.ItemTapObservable
                .Subscribe(ViewModel.SelectCountry.Inputs)
                .DisposedBy(DisposeBag);
        }
    }
}
