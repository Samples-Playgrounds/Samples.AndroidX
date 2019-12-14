using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.Onboarding.EditView
{
    public class CategorizeTimeUsingProjectsOnboardingStep : IOnboardingStep
    {
        public IObservable<bool> ShouldBeVisible { get; }

        public CategorizeTimeUsingProjectsOnboardingStep(IOnboardingStorage storage, IObservable<bool> hasProjectObservable)
        {
            Ensure.Argument.IsNotNull(storage, nameof(storage));
            Ensure.Argument.IsNotNull(hasProjectObservable, nameof(hasProjectObservable));

            ShouldBeVisible = storage.HasEditedTimeEntry.CombineLatest(
                storage.HasSelectedProject,
                hasProjectObservable,
                (hasEditedTimeEntry, hasSelectedProject, hasProject) => !hasEditedTimeEntry && !hasSelectedProject && !hasProject)
                .DistinctUntilChanged();
        }
    }
}
