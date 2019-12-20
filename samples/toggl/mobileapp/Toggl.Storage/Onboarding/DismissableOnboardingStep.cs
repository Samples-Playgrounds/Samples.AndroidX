using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Storage.Onboarding
{
    public sealed class DismissableOnboardingStep : IDismissable, IOnboardingStep
    {
        private readonly ISubject<bool> wasDismissedSubject;
        private readonly IOnboardingStorage onboardingStorage;

        public IObservable<bool> ShouldBeVisible { get; }

        public string Key { get; }

        public DismissableOnboardingStep(IOnboardingStep onboardingStep, string key, IOnboardingStorage onboardingStorage)
        {
            Ensure.Argument.IsNotNull(onboardingStep, nameof(onboardingStep));
            Ensure.Argument.IsNotNullOrWhiteSpaceString(key, nameof(key));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));

            this.onboardingStorage = onboardingStorage;

            Key = key;

            wasDismissedSubject = new BehaviorSubject<bool>(
                onboardingStorage.WasDismissed(this)
            );

            ShouldBeVisible = onboardingStep
                .ShouldBeVisible
                .CombineLatest(
                    wasDismissedSubject.AsObservable(),
                    (shouldBeVisible, wasDismissed) => shouldBeVisible && !wasDismissed);
        }

        public void Dismiss()
        {
            onboardingStorage.Dismiss(this);
            wasDismissedSubject.OnNext(true);
        }
    }
}
