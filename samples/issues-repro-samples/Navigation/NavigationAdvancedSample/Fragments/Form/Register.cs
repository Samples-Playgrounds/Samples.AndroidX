using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.Navigation;

namespace com.companyname.NavigationAdvancedSample.Fragments.Form
{
    public class Register : Fragment
    {
        public Register() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.fragment_register, container, false);

            view.FindViewById<Button>(Resource.Id.signup_btn).Click += (sender, e) =>
            {
                Navigation.FindNavController((View)sender).Navigate(Resource.Id.action_register_to_registered);
            };
            return view;
        }
    }
}