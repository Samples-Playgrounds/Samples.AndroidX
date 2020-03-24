using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using System.Collections.Immutable;
using Toggl.Core.Analytics;
using System.Linq;
using Toggl.Core.UI.Services;
using System.Reactive.Subjects;
using Toggl.Core.Extensions;
using Toggl.Core.Sync;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SuggestionsViewModel : ViewModel
    {
        private const int suggestionCount = 3;
        private static readonly SuggestionsComparer suggestionsComparer = new SuggestionsComparer();
        private static readonly TimeSpan recalculationThrottleDuration = TimeSpan.FromMilliseconds(500);

        private readonly IInteractorFactory interactorFactory;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IRxActionFactory rxActionFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ITimeService timeService;
        private readonly IPermissionsChecker permissionsChecker;
        private readonly IBackgroundService backgroundService;
        private readonly IUserPreferences userPreferences;
        private readonly ISyncManager syncManager;
        private readonly IWidgetsService widgetsService;

        public IObservable<IImmutableList<Suggestion>> Suggestions { get; private set; }
        public IObservable<bool> IsEmpty { get; private set; }
        public RxAction<Suggestion, IThreadSafeTimeEntry> StartTimeEntry { get; private set; }

        public SuggestionsViewModel(
            IInteractorFactory interactorFactory,
            IOnboardingStorage onboardingStorage,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            IAnalyticsService analyticsService,
            ITimeService timeService,
            IPermissionsChecker permissionsChecker,
            INavigationService navigationService,
            IBackgroundService backgroundService,
            IUserPreferences userPreferences,
            ISyncManager syncManager,
            IWidgetsService widgetsService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(widgetsService, nameof(widgetsService));

            this.interactorFactory = interactorFactory;
            this.onboardingStorage = onboardingStorage;
            this.schedulerProvider = schedulerProvider;
            this.rxActionFactory = rxActionFactory;
            this.analyticsService = analyticsService;
            this.timeService = timeService;
            this.permissionsChecker = permissionsChecker;
            this.backgroundService = backgroundService;
            this.userPreferences = userPreferences;
            this.syncManager = syncManager;
            this.widgetsService = widgetsService;
        }

        public override Task Initialize()
        {
            base.Initialize();

            StartTimeEntry = rxActionFactory.FromObservable<Suggestion, IThreadSafeTimeEntry>(startTimeEntry);

            var appResumedFromBackground = backgroundService
                .AppResumedFromBackground
                .SelectUnit()
                .Skip(1);

            var userCalendarPreferencesChanged = userPreferences
                .EnabledCalendars
                .DistinctUntilChanged()
                .SelectUnit()
                .Skip(1);

            var syncingFinishedWhenInForeground = syncManager.ProgressObservable
                .Where(progress => progress != SyncProgress.Syncing && progress != SyncProgress.Unknown && !backgroundService.AppIsInBackground)
                .SelectUnit();

            Suggestions = permissionsChecker.CalendarPermissionGranted
                .ReemitWhen(appResumedFromBackground)
                .ReemitWhen(userCalendarPreferencesChanged)
                .ReemitWhen(syncingFinishedWhenInForeground)
                .Throttle(recalculationThrottleDuration, schedulerProvider.BackgroundScheduler)
                .SelectMany(isCalendarAuthorized => getSuggestions()
                    .Do(suggestions => trackPresentedSuggestions(suggestions, isCalendarAuthorized)))
                .DistinctUntilChanged(suggestionsComparer)
                .Do(widgetsService.OnSuggestionsUpdated)
                .ObserveOn(schedulerProvider.BackgroundScheduler)
                .AsDriver(onErrorJustReturn: ImmutableList.Create<Suggestion>(), schedulerProvider: schedulerProvider);

            IsEmpty = Suggestions
                .Select(suggestions => suggestions.None())
                .StartWith(true)
                .AsDriver(onErrorJustReturn: true, schedulerProvider: schedulerProvider);

            return base.Initialize();
        }

        private IObservable<IImmutableList<Suggestion>> getSuggestions()
            => interactorFactory.GetSuggestions(suggestionCount).Execute()
                .Select(suggestions => suggestions.ToImmutableList());

        private IObservable<IThreadSafeTimeEntry> startTimeEntry(Suggestion suggestion)
        {
            onboardingStorage.SetTimeEntryContinued();

            var timeEntry = interactorFactory
                .StartSuggestion(suggestion)
                .Execute()
                .ToObservable();

            analyticsService.SuggestionStarted.Track(suggestion.ProviderType);

            if (suggestion.ProviderType == SuggestionProviderType.Calendar)
                trackCalendarOffset(suggestion);

            return timeEntry;
        }

        private void trackPresentedSuggestions(IImmutableList<Suggestion> suggestions, bool isAuthorized)
        {
            var suggestionsCount = suggestions
                .GroupBy(s => s.ProviderType)
                .Select(group => (group.Key, group.Count()));

            var workspaceCount = suggestions
                .Select(suggestion => suggestion.WorkspaceId)
                .Distinct()
                .Count();

            analyticsService.Track(new SuggestionsPresentedEvent(suggestionsCount, isAuthorized, workspaceCount));
        }

        private void trackCalendarOffset(Suggestion suggestion)
        {
            var currentTime = timeService.CurrentDateTime;
            var startTime = suggestion.StartTime;

            var offset = currentTime - startTime;

            analyticsService.Track(new CalendarSuggestionContinuedEvent(offset));
        }

        private class SuggestionsComparer : IEqualityComparer<IImmutableList<Suggestion>>
        {
            public bool Equals(IImmutableList<Suggestion> x, IImmutableList<Suggestion> y)
            {
                if (x.Count != y.Count)
                    return false;

                for (int i = 0; i < x.Count; i++)
                {
                    if (!x[i].Equals(y[i]))
                        return false;
                }

                return true;
            }

            public int GetHashCode(IImmutableList<Suggestion> obj)
                => obj.GetHashCode();
        }
    }
}
