using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace com.companyname.NavigationAdvancedSample.Fragments.Form
{
    public class Registered : Fragment
    {
        public Registered() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_registered, container, false);
        }
    }
}