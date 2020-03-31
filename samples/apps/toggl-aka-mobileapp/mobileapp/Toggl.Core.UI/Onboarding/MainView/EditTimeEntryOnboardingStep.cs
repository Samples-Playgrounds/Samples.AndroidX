using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.Onboarding.MainView
{
    public sealed class EditTimeEntryOnboardingStep : IOnboardingStep
    {
        public IObservable<bool> ShouldBeVisible { get; }

        public EditTimeEntryOnboardingStep(IOnboardingStorage onboardingStorage, IObservable<bool> isTimeEntriesLogEmpty)
        {
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(isTimeEntriesLogEmpty, nameof(isTimeEntriesLogEmpty));

            ShouldBeVisible = onboardingStorage.UserSignedUpUsingTheApp.CombineLatest(
                onboardingStorage.HasTappedTimeEntry,
                onboardingStorage.NavigatedAwayFromMainViewAfterTappingStopButton,
                onboardingStorage.HasTimeEntryBeenContinued,
                isTimeEntriesLogEmpty,
                shouldBeVisible);
        }

        private bool shouldBeVisible(
            bool signedUpUsingTheApp,
            bool hasTappedTimeEntry,
            bool navigatedAwayFromMainViewAfterTappingStopButton,
            bool hasTimeEntryBeenContinued,
            bool isEmpty)
        {
            return signedUpUsingTheApp
                && !navigatedAwayFromMainViewAfterTappingStopButton
                && !hasTimeEntryBeenContinued
                && !hasTappedTimeEntry
                && !isEmpty;
        }
    }
}
