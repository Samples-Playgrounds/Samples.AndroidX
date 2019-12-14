using Android.Graphics;
using Android.Views;
using Android.Widget;
using Toggl.Core.Autocomplete.Suggestions;
using static Toggl.Droid.Resource.Id;

namespace Toggl.Droid.ViewHolders
{
    public class TimeEntrySuggestionViewHolder : SuggestionRecyclerViewHolder<TimeEntrySuggestion>
    {
        private TextView taskLabel;
        private TextView projectLabel;
        private TextView clientNameLabel;
        private TextView descriptionLabel;

        public TimeEntrySuggestionViewHolder(View itemView)
            : base(itemView)
        {
        }

        protected override void InitializeViews()
        {
            taskLabel = ItemView.FindViewById<TextView>(TaskLabel);
            projectLabel = ItemView.FindViewById<TextView>(ProjectLabel);
            clientNameLabel = ItemView.FindViewById<TextView>(ClientNameLabel);
            descriptionLabel = ItemView.FindViewById<TextView>(DescriptionLabel);
        }

        protected override void UpdateView()
        {
            if (string.IsNullOrEmpty(Suggestion.Description))
            {
                descriptionLabel.Visibility = ViewStates.Gone;
            }
            else
            {
                descriptionLabel.Text = Suggestion.Description;
                descriptionLabel.Visibility = ViewStates.Visible;
            }

            if (string.IsNullOrEmpty(Suggestion.ClientName))
            {
                clientNameLabel.Visibility = ViewStates.Gone;
            }
            else
            {
                clientNameLabel.Text = Suggestion.ClientName;
                clientNameLabel.Visibility = ViewStates.Visible;
            }

            if (string.IsNullOrEmpty(Suggestion.ProjectName))
            {
                projectLabel.Visibility = ViewStates.Gone;
                taskLabel.Visibility = ViewStates.Invisible;
                return;
            }

            var projectColor = Color.ParseColor(Suggestion.ProjectColor);
            projectLabel.Text = Suggestion.ProjectName;
            projectLabel.SetTextColor(projectColor);
            projectLabel.Visibility = ViewStates.Visible;

            if (string.IsNullOrEmpty(Suggestion.TaskName))
            {
                taskLabel.Visibility = ViewStates.Invisible;
                return;
            }

            taskLabel.SetTextColor(projectColor);
            taskLabel.Visibility = ViewStates.Visible;
            taskLabel.Text = $": {Suggestion.TaskName}";
        }
    }
}
