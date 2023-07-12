using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.Navigation;

namespace com.companyname.NavigationAdvancedSample.Fragments.Home
{
    public class Title : Fragment
    {
        public Title() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.fragment_title, container, false);
            
            view.FindViewById<Button>(Resource.Id.about_btn).Click += (sender, e) =>
            {
                Navigation.FindNavController((View)sender).Navigate(Resource.Id.action_title_to_about);
            };
            return view;
        }
    }
    
}