using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace com.companyname.NavigationAdvancedSample.Fragments.Home
{
    public class About : Fragment
    {
        public About() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_about, container, false);
        }
    }
}