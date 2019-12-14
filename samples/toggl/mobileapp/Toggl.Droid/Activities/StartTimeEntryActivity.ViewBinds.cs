using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;
using Toggl.Droid.LayoutManagers;
using Toggl.Droid.Views;
using static Toggl.Droid.Resource.Id;

namespace Toggl.Droid.Activities
{
    public partial class StartTimeEntryActivity
    {
        private ImageView selectTagToolbarButton;
        private ImageView selectProjectToolbarButton;
        private ImageView selectBillableToolbarButton;

        private ViewGroup durationCard;

        private TextView durationLabel;

        private RecyclerView recyclerView;

        private AutocompleteEditText descriptionField;
        private StartTimeEntryRecyclerAdapter adapter;

        protected override void InitializeViews()
        {
            selectTagToolbarButton = FindViewById<ImageView>(ToolbarTagButton);
            selectProjectToolbarButton = FindViewById<ImageView>(ToolbarProjectButton);
            selectBillableToolbarButton = FindViewById<ImageView>(ToolbarBillableButton);

            durationCard = FindViewById<ViewGroup>(DurationCard);
            durationCard.FitBottomInset();

            durationLabel = FindViewById<TextView>(DurationText);

            recyclerView = FindViewById<RecyclerView>(SuggestionsRecyclerView);

            descriptionField = FindViewById<AutocompleteEditText>(DescriptionTextField);

            adapter = new StartTimeEntryRecyclerAdapter();
            recyclerView.SetLayoutManager(new UnpredictiveLinearLayoutManager(this));
            recyclerView.SetAdapter(adapter);

            SetupToolbar();
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.toolbar_close);
        }
    }
}
