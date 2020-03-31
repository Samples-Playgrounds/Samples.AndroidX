using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.Core.Graphics;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using static Toggl.Droid.Resource.Id;

namespace Toggl.Droid.ViewHolders
{
    public class ProjectSuggestionViewHolder : SuggestionRecyclerViewHolder<ProjectSuggestion>
    {
        private View caret;
        private View toggleTasksButton;
        private View selectedProjectToken;

        private TextView taskLabel;
        private TextView projectLabel;
        private TextView taskCountLabel;
        private TextView clientNameLabel;
        private IDisposable toggleTasksDisposable;
        private readonly ISubject<ProjectSuggestion> toggleTasksSubject;

        public ProjectSuggestionViewHolder(View itemView, ISubject<ProjectSuggestion> toggleTasksSubject)
            : base(itemView)
        {
            this.toggleTasksSubject = toggleTasksSubject;
        }

        protected override void InitializeViews()
        {
            caret = ItemView.FindViewById(Caret);
            taskLabel = ItemView.FindViewById<TextView>(TaskLabel);
            toggleTasksButton = ItemView.FindViewById(ToggleTasksButton);
            projectLabel = ItemView.FindViewById<TextView>(ProjectLabel);
            taskCountLabel = ItemView.FindViewById<TextView>(TaskCountLabel);
            clientNameLabel = ItemView.FindViewById<TextView>(ClientNameLabel);
            selectedProjectToken = ItemView.FindViewById(ProjectSelectionToken);

            toggleTasksDisposable = toggleTasksButton.Rx().Tap()
                .Select(_ => Suggestion)
                .Subscribe(toggleTasksSubject);
        }

        protected override void UpdateView()
        {
            var projectColor = Color.ParseColor(Suggestion.ProjectColor);
            projectLabel.Text = Suggestion.ProjectName;
            projectLabel.SetTextColor(projectColor);

            clientNameLabel.Text = Suggestion.ClientName ?? "";
            clientNameLabel.Visibility = string.IsNullOrEmpty(Suggestion.ClientName) ? ViewStates.Gone : ViewStates.Visible;

            taskCountLabel.Text = Suggestion.FormattedNumberOfTasks();

            var caretAngle = Suggestion.TasksVisible ? 180.0f : 0.0f;
            caret.Visibility = Suggestion.HasTasks.ToVisibility();
            caret.Animate().SetDuration(1).Rotation(caretAngle);

            toggleTasksButton.Visibility = Suggestion.HasTasks.ToVisibility();

            if (selectedProjectToken == null)
                return;

            selectedProjectToken.Visibility = Suggestion.Selected.ToVisibility(useGone: false);
            if (Suggestion.Selected && selectedProjectToken.Background is GradientDrawable drawable)
            {
                var opacity = (int)Math.Round(2.55 * 12);
                var argb = ColorUtils.SetAlphaComponent(projectColor.ToArgb(), opacity);
                drawable.SetColor(argb);
                drawable.InvalidateSelf();
            }
        }
    }
}
