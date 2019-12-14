using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.AppBar;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using static Toggl.Droid.Resource.Id;
using TextResources = Toggl.Shared.Resources;
using TagsAdapter = Toggl.Droid.Adapters.SimpleAdapter<string>;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.Activities
{
    public sealed partial class EditTimeEntryActivity : ReactiveActivity<EditTimeEntryViewModel>
    {
        private readonly TagsAdapter tagsAdapter = new TagsAdapter(
            Resource.Layout.EditTimeEntryTagCell,
            StringViewHolder.Create);

        private EditText descriptionEditText;

        private Group singleTimeEntryModeViews;
        private Group timeEntriesGroupModeViews;
        private Group stoppedTimeEntryStopTimeElements;
        private Group billableRelatedViews;

        private TextView errorTitle;
        private CardView errorContainer;
        private TextView errorText;

        private TextView groupCountTextView;
        private TextView groupDurationTextView;

        private View projectButton;
        private TextView projectPlaceholderLabel;
        private TextView projectTaskClientTextView;

        private View tagsButton;
        private RecyclerView tagsRecycler;

        private View billableButton;
        private TextView billableLabel;
        private Switch billableSwitch;

        private TextView startTimeTextView;
        private TextView startTimeLabel;
        private TextView startDateTextView;
        private View changeStartTimeButton;

        private TextView stopTimeTextView;
        private TextView stopTimeLabel;
        private TextView stopDateTextView;
        private View changeStopTimeButton;

        private View stopTimeEntryButton;

        private TextView durationTextView;
        private TextView durationLabel;
        private View changeDurationButton;

        private TextView deleteLabel;
        private View deleteButton;
        
        private AppBarLayout appBarLayout;
        private NestedScrollView scrollView;

        protected override void InitializeViews()
        {
            descriptionEditText = FindViewById<EditText>(DescriptionEditText);

            singleTimeEntryModeViews = FindViewById<Group>(SingleTimeEntryModeViews);
            timeEntriesGroupModeViews = FindViewById<Group>(TimeEntriesGroupModeViews);
            stoppedTimeEntryStopTimeElements = FindViewById<Group>(StoppedTimeEntryStopTimeElements);
            billableRelatedViews = FindViewById<Group>(BillableRelatedViews);

            errorContainer = FindViewById<CardView>(ErrorContainer);
            errorTitle = FindViewById<TextView>(ErrorTitle);
            errorText = FindViewById<TextView>(ErrorText);

            groupCountTextView = FindViewById<TextView>(GroupCount);
            groupDurationTextView = FindViewById<TextView>(GroupDuration);

            projectButton = FindViewById(SelectProjectButton);
            projectPlaceholderLabel = FindViewById<TextView>(ProjectPlaceholderLabel);
            projectTaskClientTextView = FindViewById<TextView>(ProjectTaskClient);

            tagsButton = FindViewById(SelectTagsButton);
            tagsRecycler = FindViewById<RecyclerView>(TagsRecyclerView);

            billableButton = FindViewById(ToggleBillableButton);
            billableLabel = FindViewById<TextView>(Resource.Id.BillableLabel);
            billableSwitch = FindViewById<Switch>(BillableSwitch);

            startTimeTextView = FindViewById<TextView>(StartTime);
            startTimeLabel = FindViewById<TextView>(StartTimeLabel);
            startDateTextView = FindViewById<TextView>(StartDate);
            changeStartTimeButton = FindViewById(StartTimeButton);

            stopTimeTextView = FindViewById<TextView>(StopTime);
            stopTimeLabel = FindViewById<TextView>(EditStopTimeLabel);
            stopDateTextView = FindViewById<TextView>(StopDate);
            changeStopTimeButton = FindViewById(StopTimeButton);

            stopTimeEntryButton = FindViewById(StopTimeEntryButtonLabel);

            durationTextView = FindViewById<TextView>(Duration);
            durationLabel = FindViewById<TextView>(DurationLabel);
            changeDurationButton = FindViewById(DurationButton);

            deleteLabel = FindViewById<TextView>(DeleteLabel);
            deleteButton = FindViewById(DeleteButton);

            scrollView = FindViewById<NestedScrollView>(Resource.Id.ScrollView);
            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.AppBarLayout);

            singleTimeEntryModeViews.Visibility = (!ViewModel.IsEditingGroup).ToVisibility();
            timeEntriesGroupModeViews.Visibility = ViewModel.IsEditingGroup.ToVisibility();

            descriptionEditText.Text = ViewModel.Description.Value;
            descriptionEditText.Hint = Shared.Resources.StartTimeEntryPlaceholder;
            errorTitle.Text = Shared.Resources.Oops;
            billableLabel.Text = Shared.Resources.Billable;
            startTimeLabel.Text = Shared.Resources.StartTime;
            stopTimeLabel.Text = Shared.Resources.EndTime;
            durationLabel.Text = Shared.Resources.Duration;
            deleteLabel.Text = Shared.Resources.DeleteThisEntry;
            projectPlaceholderLabel.Text = Shared.Resources.AddProjectTask;

            groupCountTextView.Text = string.Format(
                TextResources.EditingTimeEntryGroup,
                ViewModel.GroupCount);

            deleteLabel.Text = ViewModel.IsEditingGroup
                ? string.Format(TextResources.DeleteNTimeEntries, ViewModel.GroupCount)
                : TextResources.DeleteThisEntry;

            var layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            layoutManager.ItemPrefetchEnabled = true;
            layoutManager.InitialPrefetchItemCount = 5;
            tagsRecycler.SetLayoutManager(layoutManager);
            tagsRecycler.SetAdapter(tagsAdapter);

            SetupToolbar();
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.toolbar_close);

            scrollView.AttachMaterialScrollBehaviour(appBarLayout);
        }
    }
}
