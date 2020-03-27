using Android.Widget;
using System.Reactive.Linq;
using Toggl.Core.UI.Onboarding.EditView;
using Toggl.Droid.Extensions;
using Toggl.Droid.Helper;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    public sealed partial class EditTimeEntryActivity
    {
        private PopupWindow projectTooltip;

        private void resetOnboardingOnResume()
        {
            projectTooltip = projectTooltip
                ?? PopupWindowFactory.PopupWindowWithText(
                    this,
                    Resource.Layout.TooltipWithLeftTopArrow,
                    Resource.Id.TooltipText,
                    Shared.Resources.CategorizeWithProjects);

            prepareOnboarding();
        }

        private void clearOnboardingOnStop()
        {
            projectTooltip?.Dismiss();
            projectTooltip = null;
        }

        private void prepareOnboarding()
        {
            var storage = ViewModel.OnboardingStorage;

            var hasProject = ViewModel.ProjectClientTask.Select(projectClientTask => projectClientTask.HasProject);

            new CategorizeTimeUsingProjectsOnboardingStep(storage, hasProject)
                .ManageDismissableTooltip(
                    Observable.Return(true),
                    projectTooltip,
                    projectButton,
                    (window, view) => PopupOffsets.FromDp(16, 8, this),
                    storage)
                .DisposedBy(DisposeBag);
        }
    }
}
