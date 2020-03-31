using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Storage.Tests.Onboarding
{
    public sealed class DismissableOnboardingStepTests
    {
        public abstract class DismissableOnboardingStepTest : ReactiveTest
        {
            protected IOnboardingStorage OnboardingStorage { get; } = Substitute.For<IOnboardingStorage>();
            protected IOnboardingStep OnboardingStep { get; } = Substitute.For<IOnboardingStep>();
            protected TestScheduler Scheduler { get; } = new TestScheduler();
            protected string Key { get; } = nameof(OnboardingStep);

            protected DismissableOnboardingStep DismissableStep { get; set; }

            public DismissableOnboardingStepTest()
            {
                DismissableStep = new DismissableOnboardingStep(
                    OnboardingStep, Key, OnboardingStorage
                );
            }

            protected void PrepareDismissableStep()
                => DismissableStep = new DismissableOnboardingStep(
                    OnboardingStep, Key, OnboardingStorage);
        }

        public sealed class TheShouldBeVisibleProperty : DismissableOnboardingStepTest
        {
            private ITestableObserver<bool> observer;

            private void prepareTest(bool stepShouldBeVisible, bool wasDismissed)
            {
                var shouldBeVisibleObservable = Scheduler.CreateColdObservable(
                    OnNext(1, stepShouldBeVisible)
                );
                observer = Scheduler.CreateObserver<bool>();
                OnboardingStep.ShouldBeVisible.Returns(shouldBeVisibleObservable);
                OnboardingStorage.WasDismissed(Arg.Any<IDismissable>()).Returns(wasDismissed);
                PrepareDismissableStep();
                DismissableStep.ShouldBeVisible.Subscribe(observer);
                Scheduler.AdvanceTo(1);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTrueWhenOnboardingStepShouldBeVisibleAndWasNotDismissedBefore()
            {
                prepareTest(stepShouldBeVisible: true, wasDismissed: false);
                var expectedMessages = new[]
                {
                    OnNext(1, true)
                };

                observer.Messages.Should().BeEquivalentTo(expectedMessages);
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ReturnsFalseWhenOnboardingStepShouldNotBeVisible(bool wasDismissedBefore)
            {
                prepareTest(stepShouldBeVisible: false, wasDismissed: wasDismissedBefore);
                var expectedMessages = new[]
                {
                    OnNext(1, false)
                };

                observer.Messages.Should().BeEquivalentTo(expectedMessages);
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ReturnsFalseWhenWasDismissedBefore(bool stepShouldBeVisible)
            {
                prepareTest(stepShouldBeVisible: stepShouldBeVisible, wasDismissed: true);
                var expectedMessages = new[]
                {
                    OnNext(1, false)
                };

                observer.Messages.Should().BeEquivalentTo(expectedMessages);
            }

            [Fact, LogIfTooSlow]
            public void BecomesFalseAfterDismissingTheStep()
            {
                prepareTest(stepShouldBeVisible: true, wasDismissed: false);
                Scheduler.AdvanceTo(2);
                DismissableStep.Dismiss();
                var expectedMessages = new[]
                {
                    OnNext(1, true),
                    OnNext(2, false)
                };

                observer.Messages.Should().BeEquivalentTo(expectedMessages);
            }
        }

        public sealed class TheDismissMethod : DismissableOnboardingStepTest
        {
            [Fact, LogIfTooSlow]
            public void DismissesTheStepInOnboardingStorage()
            {
                DismissableStep.Dismiss();

                OnboardingStorage.Received().Dismiss(DismissableStep);
            }
        }
    }
}
