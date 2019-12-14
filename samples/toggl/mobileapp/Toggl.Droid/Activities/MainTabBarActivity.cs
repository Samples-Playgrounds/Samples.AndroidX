using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Util;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Fragments;
using Toggl.Droid.Helper;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class MainTabBarActivity : ReactiveActivity<MainTabBarViewModel>
    {
        public static readonly string StartingTabExtra = "StartingTabExtra";
        public static readonly string WorkspaceIdExtra = "WorkspaceIdExtra";
        public static readonly string StartDateExtra = "StartDateExtra";
        public static readonly string EndDateExtra = "EndDateExtra";

        private readonly Dictionary<int, Fragment> fragments = new Dictionary<int, Fragment>();
        private Fragment activeFragment;
        private bool activityResumedBefore = false;
        private int? requestedInitialTab;
        private long? reportsRequestedWorkspaceId;
        private DateTimeOffset? reportsRequestedStartDate;
        private DateTimeOffset? reportsRequestedEndDate;

        public MainTabBarActivity() : base(
            Resource.Layout.MainTabBarActivity,
            Resource.Style.AppTheme,
            Transitions.Fade)
        { }

        public MainTabBarActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void RestoreViewModelStateFromBundle(Bundle bundle)
        {
            base.RestoreViewModelStateFromBundle(bundle);

            restoreFragmentsViewModels();
            showInitialFragment(getInitialTab(Intent, bundle));
            loadReportsIntentExtras(Intent);
        }

        protected override void InitializeBindings()
        {
            navigationView
                .Rx()
                .ItemSelected()
                .Subscribe(onTabSelected)
                .DisposedBy(DisposeBag);
        }

        private int getInitialTab(Intent intent, Bundle bundle = null)
        {
            var intentTab = intent.GetIntExtra(StartingTabExtra, Resource.Id.MainTabTimerItem);
            if (intentTab != Resource.Id.MainTabTimerItem || bundle == null)
                return intentTab;

            var bundleTab = bundle.GetInt(StartingTabExtra, Resource.Id.MainTabTimerItem);
            return bundleTab;
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            requestedInitialTab = getInitialTab(intent);
            loadReportsIntentExtras(intent);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(StartingTabExtra, navigationView.SelectedItemId);
            base.OnSaveInstanceState(outState);
        }

        private void loadReportsIntentExtras(Intent intent)
        {
            var workspaceId = intent.GetLongExtra(WorkspaceIdExtra, 0L);
            var startDate = intent.GetLongExtra(StartDateExtra, 0L);
            var endDate = intent.GetLongExtra(EndDateExtra, 0L);

            if (workspaceId == 0)
                reportsRequestedWorkspaceId = null;

            if (startDate == 0 || endDate == 0)
            {
                reportsRequestedStartDate = default(DateTimeOffset);
                reportsRequestedEndDate = default(DateTimeOffset);
            }
            else
            {
                reportsRequestedStartDate = DateTimeOffset.FromUnixTimeSeconds(startDate);
                reportsRequestedEndDate = DateTimeOffset.FromUnixTimeSeconds(endDate);    
            }
        }

        private void restoreFragmentsViewModels()
        {
            foreach (var frag in SupportFragmentManager.Fragments)
            {
                switch (frag)
                {
                    case MainFragment mainFragment:
                        mainFragment.ViewModel = getTabViewModel<MainViewModel>();
                        break;
                    case ReportsFragment reportsFragment:
                        reportsFragment.ViewModel = getTabViewModel<ReportsViewModel>();
                        break;
                    case CalendarFragment calendarFragment:
                        calendarFragment.ViewModel = getTabViewModel<CalendarViewModel>();
                        break;
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (!activityResumedBefore)
            {
                navigationView.SelectedItemId = requestedInitialTab ?? Resource.Id.MainTabTimerItem;
                activityResumedBefore = true;
                requestedInitialTab = null;
                loadReportsIfNeeded();
                return;
            }

            if (requestedInitialTab == null) return;
            navigationView.SelectedItemId = requestedInitialTab.Value;
            requestedInitialTab = null;
            loadReportsIfNeeded();
        }

        private void loadReportsIfNeeded()
        {
            if (reportsRequestedStartDate == null || reportsRequestedEndDate == null)
                return;

            var reportsViewModel = getTabViewModel<ReportsViewModel>();
            if (reportsViewModel != null && navigationView.SelectedItemId == Resource.Id.MainTabReportsItem)
            {
                reportsViewModel.LoadReport(reportsRequestedWorkspaceId, reportsRequestedStartDate.Value, reportsRequestedEndDate.Value, ReportsSource.Other);
            }

            reportsRequestedWorkspaceId = null;
            reportsRequestedStartDate = null;
            reportsRequestedEndDate = null;
        }

        private Fragment getCachedFragment(int itemId)
        {
            if (fragments.TryGetValue(itemId, out var fragment))
                return fragment;

            switch (itemId)
            {
                case Resource.Id.MainTabTimerItem:
                    fragment = new MainFragment { ViewModel = getTabViewModel<MainViewModel>() };
                    break;
                case Resource.Id.MainTabReportsItem:
                    fragment = new ReportsFragment { ViewModel = getTabViewModel<ReportsViewModel>() };
                    break;
                case Resource.Id.MainTabCalendarItem:
                    fragment = new CalendarFragment { ViewModel = getTabViewModel<CalendarViewModel>() };
                    break;
                default:
                    throw new ArgumentException($"Unexpected item id {itemId}");
            }
            fragments[itemId] = fragment;
            return fragment;
        }

        private TTabViewModel getTabViewModel<TTabViewModel>()
            where TTabViewModel : class, IViewModel
            => ViewModel.Tabs.OfType<TTabViewModel>().Single();

        public override void OnBackPressed()
        {
            if (navigationView.SelectedItemId == Resource.Id.MainTabTimerItem)
            {
                FinishAfterTransition();
                return;
            }

            if (navigationView.SelectedItemId == Resource.Id.MainTabCalendarItem)
            {
                var calendarFragment = getCachedFragment(Resource.Id.MainTabCalendarItem) as IBackPressHandler;
                if (calendarFragment?.HandledBackPress() == true)
                    return;
            }

            var fragment = getCachedFragment(Resource.Id.MainTabTimerItem);

            showFragment(fragment);

            navigationView.SelectedItemId = Resource.Id.MainTabTimerItem;
        }

        private void onTabSelected(IMenuItem item)
        {
            var fragment = getCachedFragment(item.ItemId);
            if (item.ItemId != navigationView.SelectedItemId)
            {
                showFragment(fragment);
                return;
            }

            if (fragment is IScrollableToStart scrollableToTop)
            {
                scrollableToTop.ScrollToStart();
            }
        }

        private void showFragment(Fragment fragment)
        {
            SupportFragmentManager.ExecutePendingTransactions();
            var transaction = SupportFragmentManager.BeginTransaction();

            if (activeFragment is MainFragment mainFragmentToHide)
                mainFragmentToHide.SetFragmentIsVisible(false);

            if (fragment.IsAdded)
                transaction.Hide(activeFragment).Show(fragment);
            else
                transaction.Add(Resource.Id.CurrentTabFragmmentContainer, fragment).Hide(activeFragment);

            transaction.Commit();

            if (fragment is MainFragment mainFragmentToShow)
                mainFragmentToShow.SetFragmentIsVisible(true);

            activeFragment = fragment;
        }

        private void showInitialFragment(int initialTabItemId)
        {
            SupportFragmentManager.RemoveAllFragments();
            SupportFragmentManager.ExecutePendingTransactions();

            var initialFragment = getCachedFragment(initialTabItemId);
            if (!initialFragment.IsAdded)
            {
                SupportFragmentManager
                    .BeginTransaction()
                    .Add(Resource.Id.CurrentTabFragmmentContainer, initialFragment)
                    .Commit();
            }

            if (initialFragment is MainFragment mainFragment)
                mainFragment.SetFragmentIsVisible(true);

            if (!(initialFragment is CalendarFragment))
            {
                ChangeBottomBarVisibility(true);
            }

            requestedInitialTab = initialTabItemId;
            navigationView.SelectedItemId = initialTabItemId;
            activeFragment = initialFragment;
        }

        public void ChangeBottomBarVisibility(bool isVisible)
        {
            navigationView.Visibility = isVisible.ToVisibility();
        }
    }
}
