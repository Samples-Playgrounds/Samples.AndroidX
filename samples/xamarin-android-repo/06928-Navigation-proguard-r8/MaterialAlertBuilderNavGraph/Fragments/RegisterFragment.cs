using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using Google.Android.Material.Navigation;

namespace com.companyname.MaterialAlertBuilderNavGraph.Fragments
{
    public class RegisterFragment : Fragment
    {
        private NavFragOnBackPressedCallback onBackPressedCallback;
        public RegisterFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            View view = inflater.Inflate(Resource.Layout.fragment_register, container, false);
            TextView textView = view.FindViewById<TextView>(Resource.Id.text_register);
            textView.Text = "This is Register fragment";
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            // Don't want a menu
            base.OnCreateOptionsMenu(menu, inflater);
            menu.Clear();
        }

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();

            onBackPressedCallback = new NavFragOnBackPressedCallback(this, true);
            //// Android docs:  Strongly recommended to use the ViewLifecycleOwner.This ensures that the OnBackPressedCallback is only added when the LifecycleOwner is Lifecycle.State.STARTED.
            //// The activity also removes registered callbacks when their associated LifecycleOwner is destroyed, which prevents memory leaks and makes it suitable for use in fragments or other lifecycle owners
            //// that have a shorter lifetime than the activity.
            //// Note: this rule out using OnAttach(Context context) as the view hasn't been created yet.
            RequireActivity().OnBackPressedDispatcher.AddCallback(ViewLifecycleOwner, onBackPressedCallback);
        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            onBackPressedCallback?.Remove();
            base.OnDestroy();
        }
        #endregion

        #region HandleBackPressed
        public void HandleBackPressed(NavOptions navOptions, bool animateFragments)
        {
            onBackPressedCallback.Enabled = false;

            NavController navController = Navigation.FindNavController(Activity, Resource.Id.nav_host_fragment_content_main);
            
            // Always Navigate back to the SlideShowFragment
            navController.PopBackStack(Resource.Id.nav_slideshow, false);
            navController.Navigate(Resource.Id.nav_slideshow, null, navOptions);
        }
        #endregion
    }
}