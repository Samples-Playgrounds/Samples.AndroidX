using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.Onboarding.MainView
{
    public sealed class StartTimeEntryOnboardingStep : IOnboardingStep
    {
        public IObservable<bool> ShouldBeVisible { get; }

        public StartTimeEntryOnboardingStep(IOnboardingStorage onboardingStorage)
        {
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));

            ShouldBeVisible = onboardingStorage.IsNewUser.CombineLatest(onboardingStorage.StartButtonWasTappedBefore,
                (isNewUser, startButtonWasTapped) => isNewUser && !startButtonWasTapped);
        }
    }
}
