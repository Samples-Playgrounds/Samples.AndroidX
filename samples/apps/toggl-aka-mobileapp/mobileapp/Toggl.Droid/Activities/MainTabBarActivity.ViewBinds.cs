using Android.Views;
using Toggl.Droid.Extensions;
using Google.Android.Material.BottomNavigation;

namespace Toggl.Droid.Activities
{
    public sealed partial class MainTabBarActivity
    {
        private BottomNavigationView navigationView;

        protected override void InitializeViews()
        {
            navigationView = FindViewById<BottomNavigationView>(Resource.Id.MainTabBarBottomNavigationView);
            navigationView.FitBottomInset();

            var menu = navigationView.Menu;
            var timerTab = menu.FindItem(Resource.Id.MainTabTimerItem);
            timerTab.SetTitle(Shared.Resources.Timer);

            var reportsTab = menu.FindItem(Resource.Id.MainTabReportsItem);
            reportsTab.SetTitle(Shared.Resources.Reports);

            var calendarTab = menu.FindItem(Resource.Id.MainTabCalendarItem);
            calendarTab.SetTitle(Shared.Resources.Calendar);
        }
    }
}
