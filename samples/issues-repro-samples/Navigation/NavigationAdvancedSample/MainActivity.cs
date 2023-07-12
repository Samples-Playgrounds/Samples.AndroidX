using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using Google.Android.Material.BottomNavigation;

namespace com.companyname.NavigationAdvancedSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private AppBarConfiguration appBarConfiguration;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host_container) as NavHostFragment;
            var navController = navHostFragment.NavController;

            // Setup the bottom navigation view with navController
            var bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav);
            NavigationUI.SetupWithNavController(bottomNavigationView, navController);


            // Setup the ActionBar with navController and 3 top level destinations
            int[] topLevelDestinationIds = new int[] { Resource.Id.titleScreen, Resource.Id.leaderboard, Resource.Id.register};
            appBarConfiguration = new AppBarConfiguration.Builder(topLevelDestinationIds).Build();
            NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);
            
        }

        public override bool OnSupportNavigateUp()
        {
            return NavigationUI.NavigateUp(Navigation.FindNavController(this, Resource.Id.nav_host_container), appBarConfiguration);
        }

    }
}