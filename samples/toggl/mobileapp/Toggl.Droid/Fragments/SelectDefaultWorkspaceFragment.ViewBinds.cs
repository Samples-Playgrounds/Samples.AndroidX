using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace Toggl.Droid.Fragments
{
    public partial class SelectDefaultWorkspaceFragment
    {
        private RecyclerView recyclerView;
        private TextView txtSetDefaultWorkspace;
        private TextView txtTimeWillBeTrackedAuto;
        
        protected override void InitializeViews(View rootView)
        {
            recyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.SelectDefaultWorkspaceRecyclerView);
            txtSetDefaultWorkspace = rootView.FindViewById<TextView>(Resource.Id.TxtSetDefaultWorkspace);
            txtTimeWillBeTrackedAuto = rootView.FindViewById<TextView>(Resource.Id.TxtTimeWillBeTrackedAuto);

            txtSetDefaultWorkspace.Text = Shared.Resources.SetDefaultWorkspace;
            txtTimeWillBeTrackedAuto.Text = Shared.Resources.SelectDefaultWorkspaceDescription;
        }
    }
}
