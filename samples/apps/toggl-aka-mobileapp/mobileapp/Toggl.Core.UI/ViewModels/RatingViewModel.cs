using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class RatingViewModel : ViewModel
    {
        private readonly ITimeService timeService;
        private readonly IRatingService ratingService;
        private readonly IAnalyticsService analyticsService;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly ISchedulerProvider schedulerProvider;

        private readonly BehaviorSubject<bool?> impressionSubject = new BehaviorSubject<bool?>(null);
        private readonly ISubject<bool> isFeedbackSuccessViewShowing = new Subject<bool>();
        private readonly ISubject<Unit> hideRatingView = new Subject<Unit>();

        private bool impressionWasRegistered => impressionSubject.Value != null;

        // Warning: this property will throw if no impression has been registered yet.
        private bool impressionIsPositive => impressionSubject.Value.Value;

        public IObservable<bool?> Impression { get; }

        public IObservable<bool> IsFeedbackSuccessViewShowing { get; }

        public IObservable<Unit> HideRatingView { get; }

        public ViewAction PerformMainAction { get; }

        public RatingViewModel(
            ITimeService timeService,
            IRatingService ratingService,
            IAnalyticsService analyticsService,
            IOnboardingStorage onboardingStorage,
            INavigationService navigationService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(ratingService, nameof(ratingService));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.timeService = timeService;
            this.ratingService = ratingService;
            this.analyticsService = analyticsService;
            this.onboardingStorage = onboardingStorage;
            this.schedulerProvider = schedulerProvider;

            Impression = impressionSubject.AsDriver(this.schedulerProvider);

            IsFeedbackSuccessViewShowing = isFeedbackSuccessViewShowing.AsDriver(this.schedulerProvider);

            HideRatingView = hideRatingView.AsDriver(this.schedulerProvider);

            PerformMainAction = rxActionFactory.FromAsync(performMainAction);
        }

        public void CloseFeedbackSuccessView()
        {
            isFeedbackSuccessViewShowing.OnNext(false);
        }

        public void RegisterImpression(bool isPositive)
        {
            impressionSubject.OnNext(isPositive);
            analyticsService.UserFinishedRatingViewFirstStep.Track(isPositive);

            if (isPositive)
            {
                trackStepOutcome(
                    RatingViewOutcome.PositiveImpression,
                    analyticsService.RatingViewFirstStepLike);
            }
            else
            {
                trackStepOutcome(
                    RatingViewOutcome.NegativeImpression,
                    analyticsService.RatingViewFirstStepDislike);
            }
        }

        private async Task performMainAction()
        {
            hide();

            if (!impressionWasRegistered)
                return;

            if (impressionIsPositive)
            {
                ratingService.AskForRating();
                /*
                 * We can't really know whether the user has actually rated.
                 * We only know that we presented the rating view (iOS)
                 * or navigated to the market (Android).
                */
                trackSecondStepOutcome(
                    RatingViewOutcome.AppWasRated,
                    RatingViewSecondStepOutcome.AppWasRated,
                    analyticsService.RatingViewSecondStepRate);
            }
            else
            {
                var sendFeedbackSucceed = await Navigate<SendFeedbackViewModel, bool>();
                isFeedbackSuccessViewShowing.OnNext(sendFeedbackSucceed);

                trackSecondStepOutcome(
                    RatingViewOutcome.FeedbackWasLeft,
                    RatingViewSecondStepOutcome.FeedbackWasLeft,
                    analyticsService.RatingViewSecondStepSendFeedback);
            }
        }

        public void Dismiss()
        {
            hide();

            if (!impressionWasRegistered)
                return;

            if (impressionIsPositive)
            {
                trackSecondStepOutcome(
                    RatingViewOutcome.AppWasNotRated,
                    RatingViewSecondStepOutcome.AppWasNotRated,
                    analyticsService.RatingViewSecondStepDontRate);
            }
            else
            {
                trackSecondStepOutcome(
                    RatingViewOutcome.FeedbackWasNotLeft,
                    RatingViewSecondStepOutcome.FeedbackWasNotLeft,
                    analyticsService.RatingViewSecondStepDontSendFeedback);
            }
        }

        private void trackSecondStepOutcome(RatingViewOutcome outcome, RatingViewSecondStepOutcome genericEventParameter, IAnalyticsEvent specificEvent)
        {
            trackStepOutcome(outcome, specificEvent);
            analyticsService.UserFinishedRatingViewSecondStep.Track(genericEventParameter);
        }

        private void trackStepOutcome(RatingViewOutcome outcome, IAnalyticsEvent specificEvent)
        {
            onboardingStorage.SetRatingViewOutcome(outcome, timeService.CurrentDateTime);
            specificEvent.Track();
        }

        private void hide()
        {
            hideRatingView.OnNext(Unit.Default);
        }
    }
}
