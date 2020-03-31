using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;

namespace Toggl.Storage.Extensions
{
    public static class OnboardingStepExtensions
    {
        public static DismissableOnboardingStep ToDismissable(this IOnboardingStep step, string key, IOnboardingStorage onboardingStorage)
            => new DismissableOnboardingStep(step, key, onboardingStorage);
    }
}
