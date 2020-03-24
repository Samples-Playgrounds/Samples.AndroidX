using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class RatingViewModelTests
    {
        public abstract class RatingViewModelTest : BaseViewModelTests<RatingViewModel>
        {
            protected DateTimeOffset CurrentDateTime { get; } = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero);
            protected TestScheduler Scheduler { get; } = new TestScheduler();

            protected override void AdditionalSetup()
            {
                base.AdditionalSetup();

                TimeService.CurrentDateTime.Returns(CurrentDateTime);
            }

            protected override RatingViewModel CreateViewModel()
                => new RatingViewModel(
                    TimeService,
                    RatingService,
                    AnalyticsService,
                    OnboardingStorage,
                    NavigationService,
                    SchedulerProvider,
                    RxActionFactory);
        }

        public sealed class TheConstructor : RatingViewModelTest
        {
            [Xunit.Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useTimeService,
                bool useRatingService,
                bool useAnalyticsService,
                bool useOnboardingStorage,
                bool useNavigationService,
                bool useSchedulerProvider,
                bool useRxActionFactory)
            {
                var timeService = useTimeService ? TimeService : null;
                var ratingService = useRatingService ? RatingService : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new RatingViewModel(
                        timeService,
                        ratingService,
                        analyticsService,
                        onboardingStorage,
                        navigationService,
                        schedulerProvider,
                        rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheRegisterImpressionMethod : RatingViewModelTest
        {
            [FsCheck.Xunit.Property]
            public void EmitsNewImpression(bool impressionIsPositive)
            {
                var expectedValues = new[] { (bool?)null, impressionIsPositive };
                var actualValues = new List<bool?>();
                var viewModel = CreateViewModel();
                viewModel.Impression.Subscribe(actualValues.Add);

                viewModel.RegisterImpression(impressionIsPositive);

                TestScheduler.Start();
                CollectionAssert.AreEqual(expectedValues, actualValues);
            }

            [FsCheck.Xunit.Property]
            public void TracksTheUserFinishedRatingViewFirstStepEvent(bool impressionIsPositive)
            {
                ViewModel.RegisterImpression(impressionIsPositive);

                AnalyticsService.UserFinishedRatingViewFirstStep.Received().Track(impressionIsPositive);
            }

            public abstract class RegisterImpressionMethodTest : RatingViewModelTest
            {
                protected abstract bool ImpressionIsPositive { get; }
                protected abstract RatingViewOutcome ExpectedStorageOucome { get; }
                protected abstract IAnalyticsEvent ExpectedEvent { get; }

                [Fact, LogIfTooSlow]
                public void StoresTheAppropriateRatingViewOutcomeAndTime()
                {
                    ViewModel.RegisterImpression(ImpressionIsPositive);

                    OnboardingStorage.Received().SetRatingViewOutcome(ExpectedStorageOucome, CurrentDateTime);
                }

                [Fact, LogIfTooSlow]
                public void TracksTheCorrectEvent()
                {
                    ViewModel.RegisterImpression(ImpressionIsPositive);

                    ExpectedEvent.Received().Track();
                }
            }

            public sealed class WhenImpressionIsPositive : RegisterImpressionMethodTest
            {
                protected override bool ImpressionIsPositive => true;
                protected override RatingViewOutcome ExpectedStorageOucome => RatingViewOutcome.PositiveImpression;
                protected override IAnalyticsEvent ExpectedEvent => AnalyticsService.RatingViewFirstStepLike;
            }

            public sealed class WhenImpressionIsNegative : RegisterImpressionMethodTest
            {
                protected override bool ImpressionIsPositive => false;
                protected override RatingViewOutcome ExpectedStorageOucome => RatingViewOutcome.NegativeImpression;
                protected override IAnalyticsEvent ExpectedEvent => AnalyticsService.RatingViewFirstStepDislike;
            }
        }

        public sealed class ThePerformMainActionMethod
        {
            public abstract class PerformMainActionMethodTest : RatingViewModelTest
            {
                protected abstract bool ImpressionIsPositive { get; }
                protected abstract void EnsureCorrectActionWasPerformed();
                protected abstract RatingViewSecondStepOutcome ExpectedEventParameterToTrack { get; }
                protected abstract RatingViewOutcome ExpectedStoragetOutcome { get; }
                protected abstract IAnalyticsEvent ExpectedEvent { get; }

                [Fact, LogIfTooSlow]
                public async Task HidesTheView()
                {
                    var observer = TestScheduler.CreateObserver<Unit>();
                    ViewModel.HideRatingView.Subscribe(observer);

                    ViewModel.PerformMainAction.Execute();

                    TestScheduler.Start();
                    observer.Messages.Count.Should().Be(1);
                }

                protected override void AdditionalViewModelSetup()
                {
                    ViewModel.RegisterImpression(ImpressionIsPositive);
                }

                [Fact, LogIfTooSlow]
                public async Task PerformsTheCorrectAction()
                {
                    ViewModel.PerformMainAction.Execute();
                    TestScheduler.Start();

                    EnsureCorrectActionWasPerformed();
                }

                [Fact, LogIfTooSlow]
                public async Task TracksTheAppropriateEventWithTheExpectedParameter()
                {
                    ViewModel.PerformMainAction.Execute();
                    TestScheduler.Start();

                    AnalyticsService
                        .UserFinishedRatingViewSecondStep
                        .Received()
                        .Track(ExpectedEventParameterToTrack);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksTheCorrectEvent()
                {
                    ViewModel.PerformMainAction.Execute();
                    TestScheduler.Start();

                    ExpectedEvent.Received().Track();
                }

                [Fact, LogIfTooSlow]
                public async Task StoresTheAppropriateRatingViewOutcomeAndTime()
                {
                    ViewModel.PerformMainAction.Execute();
                    TestScheduler.Start();

                    OnboardingStorage
                        .Received()
                        .SetRatingViewOutcome(ExpectedStoragetOutcome, CurrentDateTime);
                }
            }

            public sealed class WhenImpressionIsPositive : PerformMainActionMethodTest
            {
                protected override bool ImpressionIsPositive => true;
                protected override RatingViewOutcome ExpectedStoragetOutcome => RatingViewOutcome.AppWasRated;
                protected override RatingViewSecondStepOutcome ExpectedEventParameterToTrack => RatingViewSecondStepOutcome.AppWasRated;
                protected override IAnalyticsEvent ExpectedEvent => AnalyticsService.RatingViewSecondStepRate;

                protected override void EnsureCorrectActionWasPerformed()
                {
                    RatingService.Received().AskForRating();
                }
            }

            public sealed class WhenImpressionIsNegative : PerformMainActionMethodTest
            {
                protected override bool ImpressionIsPositive => false;
                protected override RatingViewOutcome ExpectedStoragetOutcome => RatingViewOutcome.FeedbackWasLeft;
                protected override RatingViewSecondStepOutcome ExpectedEventParameterToTrack => RatingViewSecondStepOutcome.FeedbackWasLeft;
                protected override IAnalyticsEvent ExpectedEvent => AnalyticsService.RatingViewSecondStepSendFeedback;

                protected override void EnsureCorrectActionWasPerformed()
                {
                    NavigationService.Received().Navigate<SendFeedbackViewModel, bool>(ViewModel.View)
                        .Wait();
                }
            }

            public sealed class WhenImpressionWasntLeft : RatingViewModelTest
            {
                [Fact, LogIfTooSlow]
                public async Task DoesNothing()
                {
                    ViewModel.PerformMainAction.Execute();
                    TestScheduler.Start();

                    RatingService.DidNotReceive().AskForRating();
                }
            }
        }

        public sealed class TheDismissMethod : RatingViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void HidesTheView()
            {
                var observer = TestScheduler.CreateObserver<Unit>();
                ViewModel.HideRatingView.Subscribe(observer);

                ViewModel.Dismiss();

                TestScheduler.Start();
                observer.Messages.Count.Should().Be(1);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotTrackAnythingIfImpressionWasNotLeft()
            {
                ViewModel.Dismiss();

                AnalyticsService.UserFinishedRatingViewFirstStep.DidNotReceive().Track(Arg.Any<bool>());
                AnalyticsService.UserFinishedRatingViewSecondStep.DidNotReceive().Track(Arg.Any<RatingViewSecondStepOutcome>());
            }

            [Fact, LogIfTooSlow]
            public void DoesNotSotreAnythingIfImpressionWasNotLeft()
            {
                ViewModel.Dismiss();

                OnboardingStorage.DidNotReceive().SetRatingViewOutcome(Arg.Any<RatingViewOutcome>(), Arg.Any<DateTimeOffset>());
            }

            public abstract class DismissMethodTest : RatingViewModelTest
            {
                protected abstract bool ImpressionIsPositive { get; }
                protected abstract RatingViewOutcome ExpectedStorageOutcome { get; }
                protected abstract RatingViewSecondStepOutcome ExpectedEventParameterToTrack { get; }
                protected abstract IAnalyticsEvent ExpectedEvent { get; }

                protected override void AdditionalViewModelSetup()
                {
                    ViewModel.RegisterImpression(ImpressionIsPositive);
                }

                [Fact, LogIfTooSlow]
                public void StoresTheExpectedRatingViewOutcomeAndTime()
                {
                    ViewModel.Dismiss();

                    OnboardingStorage.Received().SetRatingViewOutcome(ExpectedStorageOutcome, CurrentDateTime);
                }

                [Fact, LogIfTooSlow]
                public void TracksTheAppropriateEventWithTheExpectedParameter()
                {
                    ViewModel.Dismiss();

                    AnalyticsService.UserFinishedRatingViewSecondStep.Received().Track(ExpectedEventParameterToTrack);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksTheCorrectEvent()
                {
                    ViewModel.Dismiss();

                    ExpectedEvent.Received().Track();
                }
            }

            public sealed class WhenImpressionIsPositive : DismissMethodTest
            {
                protected override bool ImpressionIsPositive => true;
                protected override RatingViewOutcome ExpectedStorageOutcome => RatingViewOutcome.AppWasNotRated;
                protected override RatingViewSecondStepOutcome ExpectedEventParameterToTrack => RatingViewSecondStepOutcome.AppWasNotRated;
                protected override IAnalyticsEvent ExpectedEvent => AnalyticsService.RatingViewSecondStepDontRate;
            }

            public sealed class WhenImpressionIsNegative : DismissMethodTest
            {
                protected override bool ImpressionIsPositive => false;
                protected override RatingViewOutcome ExpectedStorageOutcome => RatingViewOutcome.FeedbackWasNotLeft;
                protected override RatingViewSecondStepOutcome ExpectedEventParameterToTrack => RatingViewSecondStepOutcome.FeedbackWasNotLeft;
                protected override IAnalyticsEvent ExpectedEvent => AnalyticsService.RatingViewSecondStepDontSendFeedback;
            }

            public sealed class TheIsFeedBackSuccessViewShowingProperty : RatingViewModelTest
            {
                [Fact, LogIfTooSlow]
                public void EmitsTrueWhenTapOnTheView()
                {
                    var observer = TestScheduler.CreateObserver<bool>();
                    var viewModel = CreateViewModel();

                    viewModel.IsFeedbackSuccessViewShowing.Subscribe(observer);
                    viewModel.CloseFeedbackSuccessView();
                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, false)
                    );
                }
            }
        }
    }
}
