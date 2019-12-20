using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Adapters;
using Toggl.Droid.LayoutManagers;

namespace Toggl.Droid.Activities
{
    public partial class SelectClientActivity
    {
        private readonly SelectClientRecyclerAdapter selectClientRecyclerAdapter = new SelectClientRecyclerAdapter();

        private EditText filterEditText;
        private RecyclerView selectClientRecyclerView;

        protected override void InitializeViews()
        {
            filterEditText = FindViewById<EditText>(Resource.Id.FilterEditText);
            selectClientRecyclerView = FindViewById<RecyclerView>(Resource.Id.SelectClientRecyclerView);

            filterEditText.Hint = Shared.Resources.AddClient;
            selectClientRecyclerView.SetLayoutManager(new UnpredictiveLinearLayoutManager(this)
            {
                ItemPrefetchEnabled = true,
                InitialPrefetchItemCount = 4
            });
            selectClientRecyclerView.SetAdapter(selectClientRecyclerAdapter);
            
            SetupToolbar();
        }
    }
}
