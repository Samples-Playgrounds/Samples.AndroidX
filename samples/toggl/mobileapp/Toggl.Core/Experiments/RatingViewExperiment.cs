using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.DataSources;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.Experiments
{
    [Preserve(AllMembers = true)]
    public sealed class RatingViewExperiment
    {
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IRemoteConfigService remoteConfigService;
        private readonly IUpdateRemoteConfigCacheService updateRemoteConfigCacheService;

        public IObservable<bool> RatingViewShouldBeVisible
            => updateRemoteConfigCacheService.RemoteConfigChanged
                .Select(_ => remoteConfigService.GetRatingViewConfiguration())
                .SelectMany(criterionMatched)
                .Select(tuple => tuple.criterionMatched && dayCountPassed(tuple.configuration));

        public RatingViewExperiment(
            ITimeService timeService,
            ITogglDataSource dataSource,
            IOnboardingStorage onboardingStorage,
            IRemoteConfigService remoteConfigService,
            IUpdateRemoteConfigCacheService updateRemoteConfigCacheService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(remoteConfigService, nameof(remoteConfigService));
            Ensure.Argument.IsNotNull(updateRemoteConfigCacheService, nameof(updateRemoteConfigCacheService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.onboardingStorage = onboardingStorage;
            this.remoteConfigService = remoteConfigService;
            this.updateRemoteConfigCacheService = updateRemoteConfigCacheService;
        }

        private bool dayCountPassed(RatingViewConfiguration ratingViewConfiguration)
        {
            var firstOpened = onboardingStorage.GetFirstOpened();
            if (firstOpened == null)
                return false;

            var targetDayCount = ratingViewConfiguration.DayCount;
            var actualDayCount = (timeService.CurrentDateTime - firstOpened.Value).TotalDays;
            return actualDayCount >= targetDayCount;
        }

        private IObservable<(bool criterionMatched, RatingViewConfiguration configuration)> criterionMatched(RatingViewConfiguration configuration)
        {
            switch (configuration.Criterion)
            {
                case RatingViewCriterion.Stop:
                    return dataSource
                        .TimeEntries
                        .TimeEntryStopped
                        .Select(_ => (true, configuration));

                case RatingViewCriterion.Start:
                    return dataSource
                        .TimeEntries
                        .TimeEntryStarted
                        .Select(_ => (true, configuration));

                case RatingViewCriterion.Continue:
                    return dataSource
                        .TimeEntries
                        .TimeEntryContinued
                        .Merge(dataSource.TimeEntries.SuggestionStarted)
                        .Select(_ => (true, configuration));

                default:
                    return Observable.Return((false, configuration));
            }
        }
    }
}
