using Android.App;
using Android.Content.PM;
using Android.Runtime;
using System;
using AndroidX.RecyclerView.Widget;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.Presentation;
using Toggl.Droid.ViewHolders;
using Toggl.Shared;
using CoreResource = Toggl.Shared.Resources;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class LicensesActivity : ReactiveActivity<LicensesViewModel>
    {
        private SimpleAdapter<License> adapter;

        public LicensesActivity() : base(
            Resource.Layout.LicensesActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromRight)
        {
        }

        public LicensesActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeViews()
        {
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView);

            adapter = new SimpleAdapter<License>(Resource.Layout.LicensesActivityCell, LicenseViewHolder.Create);
            var layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);

            SetupToolbar(CoreResource.Licenses);
        }

        protected override void InitializeBindings()
        {
            adapter.Items = ViewModel.Licenses;
        }
    }
}
