using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace com.companyname.MaterialAlertBuilderNavGraph.Fragments
{
    public class GalleryFragment : Fragment
    {
        public GalleryFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            View view = inflater.Inflate(Resource.Layout.fragment_gallery, container, false);
            TextView textView = view.FindViewById<TextView>(Resource.Id.text_gallery);
            textView.Text = "This is gallery fragment";
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            // Don't want a menu
            base.OnCreateOptionsMenu(menu, inflater);
            menu.Clear();
        }
    }
}