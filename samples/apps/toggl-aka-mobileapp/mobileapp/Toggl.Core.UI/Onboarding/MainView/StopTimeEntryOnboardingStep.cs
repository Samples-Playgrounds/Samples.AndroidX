using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.Onboarding.MainView
{
    public sealed class StopTimeEntryOnboardingStep : IOnboardingStep
    {
        public IObservable<bool> ShouldBeVisible { get; }

        public StopTimeEntryOnboardingStep(IOnboardingStorage onboardingStorage, IObservable<bool> isTimeEntryRunning)
        {
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(isTimeEntryRunning, nameof(isTimeEntryRunning));

            ShouldBeVisible = onboardingStorage.IsNewUser
                .CombineLatest(
                    onboardingStorage.StopButtonWasTappedBefore,
                    isTimeEntryRunning,
                    (isNewUser, stopButtonWasTapped, isRunning) => isNewUser && !stopButtonWasTapped && isRunning)
                .DistinctUntilChanged();
        }
    }
}
