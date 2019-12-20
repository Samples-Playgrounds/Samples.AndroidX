using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Shared;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.Onboarding.StartTimeEntryView
{
    public sealed class AddProjectOrTagOnboardingStep : IOnboardingStep
    {
        public IObservable<bool> ShouldBeVisible { get; }

        public AddProjectOrTagOnboardingStep(
            IOnboardingStorage onboardingStorage, ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));

            var hasTagObservable = dataSource.Tags.GetAll().Select(tags => tags.Any());
            var hasProjectObservable = dataSource.Projects.GetAll().Select(projects => projects.Any());
            var hasAddedProjectOrTagObservable = onboardingStorage.ProjectOrTagWasAddedBefore;

            ShouldBeVisible = hasAddedProjectOrTagObservable.CombineLatest(
                hasProjectObservable,
                hasTagObservable,
                (hasAddedProjectOrTag, hasProject, hasTag) => (hasProject || hasTag) && !hasAddedProjectOrTag
            );
        }
    }
}
