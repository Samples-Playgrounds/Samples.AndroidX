using Android.App;
using Android.Content.PM;
using System;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;
using Toggl.Droid.Presentation;
using Android.Runtime;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class SelectClientActivity : ReactiveActivity<SelectClientViewModel>
    {
        public SelectClientActivity() : base(
            Resource.Layout.SelectClientActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        { }

        public SelectClientActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        protected override void InitializeBindings()
        {
            ViewModel.Clients
                .Subscribe(selectClientRecyclerAdapter.Rx().Items())
                .DisposedBy(DisposeBag);

            filterEditText.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            selectClientRecyclerAdapter.ItemTapObservable
                .Subscribe(ViewModel.SelectClient.Inputs)
                .DisposedBy(DisposeBag);
        }
    }
}
