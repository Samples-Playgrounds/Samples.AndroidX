using Android.Views;
using Android.Widget;
using Toggl.Droid.Views;

namespace Toggl.Droid.Activities
{
    public sealed partial class EditProjectActivity
    {
        private TextView errorText;
        private View colorArrow;
        private View changeClientView;
        private View changeWorkspaceView;
        private View toggleIsPrivateView;

        private TextView workspaceNameLabel;
        private TextView clientNameTextView;
        private TextView projectNameTextView;
        private TextView privateProjectLabel;
        private TextView privateProjectHintLabel;

        private Switch isPrivateSwitch;

        private CircleView colorCircle;

        protected override void InitializeViews()
        {
            errorText = FindViewById<TextView>(Resource.Id.ErrorText);
            colorArrow = FindViewById(Resource.Id.ColorArrow);
            changeClientView = FindViewById(Resource.Id.ChangeClientView);
            colorCircle = FindViewById<CircleView>(Resource.Id.ColorCircle);
            changeWorkspaceView = FindViewById(Resource.Id.ChangeWorkspaceView);
            toggleIsPrivateView = FindViewById(Resource.Id.ToggleIsPrivateView);
            isPrivateSwitch = FindViewById<Switch>(Resource.Id.IsPrivateSwitch);
            clientNameTextView = FindViewById<TextView>(Resource.Id.ClientNameTextView);
            workspaceNameLabel = FindViewById<TextView>(Resource.Id.WorkspaceNameLabel);
            projectNameTextView = FindViewById<TextView>(Resource.Id.ProjectNameTextView);
            privateProjectLabel = FindViewById<TextView>(Resource.Id.ChangeWorkspaceViewPrivateProjectLabel);
            privateProjectHintLabel = FindViewById<TextView>(Resource.Id.PrivateProjectHint);
            
            errorText.Text = Shared.Resources.ProjectNameTakenError;
            privateProjectLabel.Text = Shared.Resources.PrivateProject;
            privateProjectHintLabel.Text = Shared.Resources.PrivateProjectHint;
            
            SetupToolbar(ViewModel.Title);
        }
    }
}
