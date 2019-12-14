using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;

namespace Toggl.Droid.Fragments
{
    public sealed partial class ReportsFragment
    {
        private FloatingActionButton selectWorkspaceFab;
        private TextView workspaceName;
        private TextView toolbarCurrentDateRangeText;
        private RecyclerView reportsRecyclerView;
        private AppBarLayout appBarLayout;

        protected override void InitializeViews(View fragmentView)
        {
            selectWorkspaceFab = fragmentView.FindViewById<FloatingActionButton>(Resource.Id.SelectWorkspaceFAB);
            workspaceName = fragmentView.FindViewById<TextView>(Resource.Id.ReportsFragmentWorkspaceName);
            toolbarCurrentDateRangeText = fragmentView.FindViewById<TextView>(Resource.Id.ToolbarCurrentDateRangeText);
            reportsRecyclerView = fragmentView.FindViewById<RecyclerView>(Resource.Id.ReportsFragmentRecyclerView);
            appBarLayout = fragmentView.FindViewById<AppBarLayout>(Resource.Id.AppBarLayout);
        }
    }
}
