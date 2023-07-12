using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace com.companyname.NavigationAdvancedSample.Fragments.List
{
    public class UserProfile : Fragment
    {
        public UserProfile() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.fragment_user_profile, container, false);
            string name = string.IsNullOrEmpty(Arguments?.GetString("USERNAME_KEY")) ? "Ali Connors" : string.Empty;
            view.FindViewById<TextView>(Resource.Id.profile_user_name).Text = name;

            return view;
        }
    }
}