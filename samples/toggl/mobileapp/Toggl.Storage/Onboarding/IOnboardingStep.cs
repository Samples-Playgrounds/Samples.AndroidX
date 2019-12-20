using System;

namespace Toggl.Storage.Onboarding
{
    public interface IOnboardingStep
    {
        IObservable<bool> ShouldBeVisible { get; }
    }
}
