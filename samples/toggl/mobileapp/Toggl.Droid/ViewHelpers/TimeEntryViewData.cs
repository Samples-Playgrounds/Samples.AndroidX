using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.ViewHelpers
{
    public class TimeEntryViewData
    {
        public LogItemViewModel ViewModel { get; }
        public ISpannable ProjectTaskClientText { get; }
        public ViewStates ProjectArchivedIconVisibility { get; }
        public Color ProjectArchivedIconTintColor { get; }
        public ViewStates ProjectTaskClientVisibility { get; }
        public ViewStates HasTagsIconVisibility { get; }
        public ViewStates BillableIconVisibility { get; }
        public ViewStates ContinueButtonVisibility { get; }
        public ViewStates ErrorNeedsSyncVisibility { get; }
        public ViewStates ErrorImageViewVisibility { get; }
        public ViewStates ContinueImageVisibility { get; }
        public ViewStates AddDescriptionLabelVisibility { get; }
        public ViewStates DescriptionVisibility { get; }

        public TimeEntryViewData(Context context, LogItemViewModel viewModel)
        {
            ViewModel = viewModel;
            if (viewModel.HasProject)
            {
                ProjectArchivedIconTintColor = Color.ParseColor(viewModel.ProjectColor);
                ProjectArchivedIconVisibility = (!viewModel.IsActive).ToVisibility();
            }
            else
            {
                ProjectArchivedIconVisibility = ViewStates.Gone;
            }
            ProjectTaskClientText = TimeEntryExtensions.ToProjectTaskClient(context,
                ViewModel.HasProject,
                ViewModel.ProjectName,
                ViewModel.ProjectColor,
                ViewModel.TaskName,
                ViewModel.ClientName,
                ViewModel.ProjectIsPlaceholder,
                ViewModel.TaskIsPlaceholder,
                displayPlaceholders: true);
            ProjectTaskClientVisibility = viewModel.HasProject.ToVisibility();
            DescriptionVisibility = viewModel.HasDescription.ToVisibility();
            AddDescriptionLabelVisibility = (!viewModel.HasDescription).ToVisibility();
            ContinueImageVisibility = viewModel.CanContinue.ToVisibility();
            ErrorImageViewVisibility = (!viewModel.CanContinue).ToVisibility();
            ErrorNeedsSyncVisibility = viewModel.NeedsSync.ToVisibility();
            ContinueButtonVisibility = viewModel.CanContinue.ToVisibility();
            BillableIconVisibility = viewModel.IsBillable.ToVisibility();
            HasTagsIconVisibility = viewModel.HasTags.ToVisibility();
        }
    }
}
