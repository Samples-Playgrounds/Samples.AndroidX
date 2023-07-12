using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace com.companyname.NavigationAdvancedSample.Fragments.List
{
    public class Leaderboard : Fragment
    {
        public Leaderboard() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.fragment_leaderboard, container, false);
            return view;
        }
    }
}