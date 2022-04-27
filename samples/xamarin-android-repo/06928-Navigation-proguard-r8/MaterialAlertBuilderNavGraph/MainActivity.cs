using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using AndroidX.Preference;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;

namespace com.companyname.MaterialAlertBuilderNavGraph
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.OBDNowPros.RedBmw", MainLauncher = true)]
    public class MainActivity : BaseActivity, NavController.IOnDestinationChangedListener, IOnApplyWindowInsetsListener
    {
        internal readonly string logTag = "GLM - MainActivity";

        private AppBarConfiguration appBarConfiguration;
        private NavigationView navigationView;
        private DrawerLayout drawerLayout;
        private BottomNavigationView bottomNavigationView;
        private NavController navController;

        // Normally this is set via a user preference setting. Just change to either true/false - see OnDestinationChanged where it is checked
        private bool devicesWithNotchesAllowFullScreen = true;
        
        // Check whether the dark theme is active or not.
        private bool nightModeActive;

        //public NavigationView NavView => navigationView;

        // A couple of pertinent articles from two of Google's Android Developers
        // Chris Banes
        //http//s://medium.com/androiddevelopers/windowinsets-listeners-to-layouts-8f9ccc8fa4d1
        // Ian Lake
        //https://medium.com/androiddevelopers/why-would-i-want-to-fitssystemwindows-4e26d9ce1eec

        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);

            Log.Debug(logTag, "About to execute SetContentView");
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // This rather than android:windowTranslucentStatus in styles seems to have fixed the problem with the OK button on the BasicDialogFragment
            // It also fixes the AppBarlayout so it extends full screen, when devicesWithNotchesAllowFullScreen = true; 
            Window.AddFlags(WindowManagerFlags.TranslucentStatus);
            
            // Require a toolbar
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            ViewCompat.SetOnApplyWindowInsetsListener(toolbar, this);
            

            // navigationView, bottomNavigationView for NavigationUI and drawerLayout for the AppBarConfiguration and NavigationUI
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav);

            // NavHostFragment so we can get a NavController 
            Log.Debug(logTag, "About to get NavHostFragment"); // We don't get to here because it fails in SetContentView() above.
            NavHostFragment navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host_fragment_content_main) as NavHostFragment;
            Log.Debug(logTag, "NavHostFragment = "+navHostFragment.ToString());
            navController = navHostFragment.NavController;
            Log.Debug(logTag, "NavController = "+navController.ToString());
            Log.Debug(logTag, "NavController.NavigatorProvider.Class.Name = "+navController.NavigatorProvider.Class.Name.ToString());
            
            // These are the fragments that you don't wont the back button of the toolbar to display on e.g. topLevel fragments. They correspond to the items of the NavigationView.
            int[] topLevelDestinationIds = new int[] { Resource.Id.nav_home, Resource.Id.nav_gallery, Resource.Id.nav_slideshow };
            appBarConfiguration = new AppBarConfiguration.Builder(topLevelDestinationIds).SetOpenableLayout(drawerLayout).Build();  // SetDrawerLayout replaced with SetOpenableLayout
            
            NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);
            NavigationUI.SetupWithNavController(navigationView, navController);
            NavigationUI.SetupWithNavController(bottomNavigationView, navController);
            
            // Add the DestinationChanged listener
            navController.AddOnDestinationChangedListener(this);
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            if (v is Toolbar)
            {
                AndroidX.Core.Graphics.Insets statusBarInsets = insets.GetInsets(WindowInsetsCompat.Type.StatusBars());

                SetMargins(v, statusBarInsets);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                {
                    if (insets.DisplayCutout != null)
                    {
                        if (devicesWithNotchesAllowFullScreen)
                            Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
                        else
                            Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
                    }
                }
            }
            return insets;
        }
        #endregion

        #region SetMargins
        private void SetMargins(View v, AndroidX.Core.Graphics.Insets insets)
        {
            ViewGroup.MarginLayoutParams marginLayoutParams = (ViewGroup.MarginLayoutParams)v.LayoutParameters;
            marginLayoutParams.LeftMargin = insets.Left;    
            marginLayoutParams.TopMargin = insets.Top;          // top is all we are concerned with
            marginLayoutParams.RightMargin = insets.Right;
            marginLayoutParams.BottomMargin = insets.Bottom;
            v.LayoutParameters = marginLayoutParams;
            v.RequestLayout();
        }
        #endregion

        #region OnCreateOptionsMenu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return true;
        }
        #endregion

        #region OnSupportNavigationUp
        public override bool OnSupportNavigateUp()
        {
            //return NavigationUI.NavigateUp(Navigation.FindNavController(this, Resource.Id.nav_host_fragment_content_main), appBarConfiguration);
            return NavigationUI.NavigateUp(navController, appBarConfiguration) || base.OnSupportNavigateUp();
        }
        #endregion

        #region OnOptionsItemSelected
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_settings:
                    navController.Navigate(Resource.Id.settingsFragment);
                    break;

                case Resource.Id.action_subscription_info:
                    ShowSubscriptionInfoDialog(GetString(Resource.String.subscription_explanation_title), GetString(Resource.String.subscription_explanation_text));
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        #region OnDestinationChanged
        public void OnDestinationChanged(NavController navController, NavDestination navDestination, Bundle bundle)
        {
            
            CheckForPreferenceChanges();

            if (navDestination.Id == Resource.Id.nav_home)
                AppCompatDelegate.DefaultNightMode = nightModeActive ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

            if (navDestination.Id == Resource.Id.nav_slideshow)
            {
                bottomNavigationView.Visibility = ViewStates.Visible;
                navigationView.Visibility = ViewStates.Gone;
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            else
            {
                bottomNavigationView.Visibility = ViewStates.Gone;
                navigationView.Visibility = ViewStates.Visible;
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            }

            #region Notes about Window.Attributes.LayoutInDisplayCutoutMode
            // Here is a bit of a trick. If we haven't set <item name="android:windowLayoutInDisplayCutoutMode">shortEdges</item> in our theme (for whatever reason). Then OnApplyWindowInsets
            // will never be called if our DrawerLayout and NavigationView have android:fitsSystemWindows="true". 

            // Therefore to guarantee that it does get called we set it here, because we don't want to letterbox our normal layouts, especially all our landscape views with the gauge views
            // Note if you do set it in styles then it should be in values-v28 or even values-v27. Android Studio gives you a warning if you try and set it in values. The problem setting in values-28 is that values-v28
            // requires the theme of the activity. Normally our theme is the splash theme and we swap it by calling SetTheme(Resource.Style.OBDTheme) in the first line of OnCreate in the MainActivity.

            // Note: Only when devicesWithNotchesAllowFullScreen is true and therefore LayoutInDisplayCutoutMode is ShortEdges will insets.DisplayCutout not be null.
            // Whenever LayoutInDisplayCutoutMode it is default or never insets.DisplayCutout will always be null.
            // So even if a device has a notch, if devicesWithNotchesAllOwFullScreen is false then will always get Default because DisplayCutout will be null.

            // Do we need this? We only need shortEdges if we have a notch, therefore why not wait until the test in OnApplyWindowInsets?
            // Answer: We do need it here, because if ShortEdges is not set here, then later in the test in OnApplyWindowInsets, insets.DisplayCutout will be null which will always result in Default being set,
            // so we can't avoid this.
            // It is really the same as if we had set ShortEdges in styles.xml of values-v28, (which we don't want to do because we are using OBDTheme.Splash). By presetting Window.Attributes.LayoutInDisplayCutoutMode
            // here, when the user tells us they want it allows insets.DisplayCutout to be not null by the time we do the test in OnApplyWindowInsets.
            // Note the Setting in Preferences has no effect if the device does not have a notch, so no harm is done if a user accidently sets devicesWithNotchesAllowFullscreen to true.
            // TODO: Make a note in our user Guide.
            #endregion


            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                Window.Attributes.LayoutInDisplayCutoutMode = devicesWithNotchesAllowFullScreen ? LayoutInDisplayCutoutMode.ShortEdges : LayoutInDisplayCutoutMode.Default;
        }
        #endregion

        #region CheckForPreferenceChanges
        private void CheckForPreferenceChanges()
        {
            // Check if anything has been changed in the Settings Fragment before re-reading and updating all the preference variables
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            nightModeActive = sharedPreferences.GetBoolean("darkTheme", false);
            currentTheme = sharedPreferences.GetString("colorThemeValue", "1");
            
        }
        #endregion

        #region ShowSubscriptionInfoDialog
        private void ShowSubscriptionInfoDialog(string title, string explanation)
        {
            string tag = "SubscriptionInfoDialogFragment";
            AndroidX.Fragment.App.FragmentManager fm = SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                AndroidX.Fragment.App.Fragment fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    BasicDialogFragment.NewInstance(title, explanation).Show(fm, tag);
            }
        }
        #endregion

    }
}

