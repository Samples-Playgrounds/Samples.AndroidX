using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Adapters;
using static Toggl.Droid.Resource.Id;

namespace Toggl.Droid.Activities
{
    public sealed partial class SelectProjectActivity
    {
        private TextView searchField;
        private RecyclerView recyclerView;
        private SelectProjectRecyclerAdapter adapter;

        protected override void InitializeViews()
        {
            searchField = FindViewById<TextView>(SearchField);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView);

            adapter = new SelectProjectRecyclerAdapter();
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.SetAdapter(adapter);
            
            SetupToolbar();
        }
    }
}
