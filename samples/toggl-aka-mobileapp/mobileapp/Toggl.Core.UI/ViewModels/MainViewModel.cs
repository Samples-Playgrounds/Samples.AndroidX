using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Experiments;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;
using Toggl.Storage.Settings;
using Toggl.Core.UI.Services;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using static Toggl.Core.Analytics.ContinueTimeEntryMode;
using static Toggl.Core.Analytics.ContinueTimeEntryOrigin;


namespace Toggl.Core.UI.ViewModels
{
    using MainLogSection = AnimatableSectionModel<DaySummaryViewModel, LogItemViewModel, IMainLogKey>;

    [Preserve(AllMembers = true)]
    public sealed class MainViewModel : ViewModel
    {
        private const int ratingViewTimeout = 5;
        private const double throttlePeriodInSeconds = 0.1;

        private bool noWorkspaceViewPresented;
        private bool hasStopButtonEverBeenUsed;
        private bool noDefaultWorkspaceViewPresented;
        private bool shouldHideRatingViewIfStillVisible = false;

        private readonly ISyncManager syncManager;
        private readonly IPlatformInfo platformInfo;
        private readonly ITogglDataSource dataSource;
        private readonly IUserPreferences userPreferences;
        private readonly IRxActionFactory rxActionFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IInteractorFactory interactorFactory;
        private readonly IAccessibilityService accessibilityService;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;
        private readonly IWidgetsService widgetsService;
        private readonly IRemoteConfigService remoteConfigService;
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;

        private readonly RatingViewExperiment ratingViewExperiment;
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly ISubject<Unit> hideRatingView = new Subject<Unit>();

        public IObservable<bool> LogEmpty { get; }
        public IObservable<int> TimeEntriesCount { get; }
        public IObservable<bool> IsInManualMode { get; private set; }
        public IObservable<string> ElapsedTime { get; private set; }
        public IObservable<bool> IsTimeEntryRunning { get; private set; }
        public IObservable<int> NumberOfSyncFailures { get; private set; }
        public IObservable<bool> ShouldShowEmptyState { get; private set; }
        public IObservable<bool> ShouldShowWelcomeBack { get; private set; }
        public IObservable<Unit> ShouldReloadTimeEntryLog { get; private set; }
        public IObservable<SyncProgress> SyncProgressState { get; private set; }
        public IObservable<bool> ShouldShowRunningTimeEntryNotification { get; private set; }
        public IObservable<bool> ShouldShowStoppedTimeEntryNotification { get; private set; }
        public IObservable<IThreadSafeTimeEntry> CurrentRunningTimeEntry { get; private set; }
        public IObservable<bool> ShouldShowRatingView { get; private set; }
        public IObservable<bool> SwipeActionsEnabled { get; }
        public IObservable<IImmutableList<MainLogSection>> TimeEntries { get; }

        public RatingViewModel RatingViewModel { get; }
        public SuggestionsViewModel SuggestionsViewModel { get; }
        public IOnboardingStorage OnboardingStorage { get; }

        public ViewAction Refresh { get; private set; }
        public ViewAction OpenSettings { get; private set; }
        public ViewAction OpenSyncFailures { get; private set; }
        public InputAction<bool> StartTimeEntry { get; private set; }
        public InputAction<EditTimeEntryInfo> SelectTimeEntry { get; private set; }
        public InputAction<TimeEntryStopOrigin> StopTimeEntry { get; private set; }
        public RxAction<ContinueTimeEntryInfo, IThreadSafeTimeEntry> ContinueTimeEntry { get; private set; }

        public ITimeService TimeService { get; }

        public TimeEntriesViewModel TimeEntriesViewModel { get; }

        public MainViewModel(
            ITogglDataSource dataSource,
            ISyncManager syncManager,
            ITimeService timeService,
            IRatingService ratingService,
            IUserPreferences userPreferences,
            IAnalyticsService analyticsService,
            IOnboardingStorage onboardingStorage,
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IRemoteConfigService remoteConfigService,
            IAccessibilityService accessibilityService,
            IUpdateRemoteConfigCacheService updateRemoteConfigCacheService,
            IAccessRestrictionStorage accessRestrictionStorage,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            IPermissionsChecker permissionsChecker,
            IBackgroundService backgroundService,
            IPlatformInfo platformInfo,
            IWidgetsService widgetsService,
            ILastTimeUsageStorage lastTimeUsageStorage)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(ratingService, nameof(ratingService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(remoteConfigService, nameof(remoteConfigService));
            Ensure.Argument.IsNotNull(accessibilityService, nameof(accessibilityService));
            Ensure.Argument.IsNotNull(updateRemoteConfigCacheService, nameof(updateRemoteConfigCacheService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(platformInfo, nameof(platformInfo));
            Ensure.Argument.IsNotNull(widgetsService, nameof(widgetsService));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));

            this.dataSource = dataSource;
            this.syncManager = syncManager;
            this.platformInfo = platformInfo;
            this.userPreferences = userPreferences;
            this.rxActionFactory = rxActionFactory;
            this.analyticsService = analyticsService;
            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;
            this.accessibilityService = accessibilityService;
            this.accessRestrictionStorage = accessRestrictionStorage;
            this.widgetsService = widgetsService;
            this.remoteConfigService = remoteConfigService;
            this.lastTimeUsageStorage = lastTimeUsageStorage;

            TimeService = timeService;
            OnboardingStorage = onboardingStorage;

            SuggestionsViewModel = new SuggestionsViewModel(interactorFactory, OnboardingStorage, schedulerProvider, rxActionFactory, analyticsService, timeService, permissionsChecker, navigationService, backgroundService, userPreferences, syncManager, widgetsService);
            RatingViewModel = new RatingViewModel(timeService, ratingService, analyticsService, OnboardingStorage, navigationService, schedulerProvider, rxActionFactory);
            TimeEntriesViewModel = new TimeEntriesViewModel(dataSource, interactorFactory, analyticsService, schedulerProvider, rxActionFactory, timeService);

            TimeEntries = TimeEntriesViewModel.TimeEntries
                .Throttle(TimeSpan.FromSeconds(throttlePeriodInSeconds))
                .AsDriver(ImmutableList<MainLogSection>.Empty, schedulerProvider);

            LogEmpty = TimeEntriesViewModel.Empty.AsDriver(schedulerProvider);
            TimeEntriesCount = TimeEntriesViewModel.Count.AsDriver(schedulerProvider);

            ratingViewExperiment = new RatingViewExperiment(timeService, dataSource, onboardingStorage, remoteConfigService, updateRemoteConfigCacheService);

            SwipeActionsEnabled = userPreferences.SwipeActionsEnabled.AsDriver(schedulerProvider);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            interactorFactory.GetCurrentUser().Execute()
                .Select(u => u.Id)
                .Subscribe(analyticsService.SetAppCenterUserId);

            await SuggestionsViewModel.Initialize();
            await RatingViewModel.Initialize();
            widgetsService.Start();

            SyncProgressState = syncManager.ProgressObservable
                .AsDriver(schedulerProvider);

            var isWelcome = OnboardingStorage.IsNewUser;

            var noTimeEntries = Observable
                .CombineLatest(TimeEntriesViewModel.Empty, SuggestionsViewModel.IsEmpty,
                    (isTimeEntryEmpty, isSuggestionEmpty) => isTimeEntryEmpty && isSuggestionEmpty)
                .DistinctUntilChanged();

            ShouldShowEmptyState = ObservableAddons.CombineLatestAll(
                    isWelcome,
                    noTimeEntries
                )
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            ShouldShowWelcomeBack = ObservableAddons.CombineLatestAll(
                    isWelcome.Select(b => !b),
                    noTimeEntries
                )
                .StartWith(false)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            IsInManualMode = userPreferences
                .IsManualModeEnabledObservable
                .AsDriver(schedulerProvider);

            ShouldShowRunningTimeEntryNotification = userPreferences.AreRunningTimerNotificationsEnabledObservable;
            ShouldShowStoppedTimeEntryNotification = userPreferences.AreStoppedTimerNotificationsEnabledObservable;

            CurrentRunningTimeEntry = dataSource.TimeEntries
                .CurrentlyRunningTimeEntry
                .AsDriver(schedulerProvider);

            IsTimeEntryRunning = dataSource.TimeEntries
                .CurrentlyRunningTimeEntry
                .Select(te => te != null)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            var durationObservable = dataSource
                .Preferences
                .Current
                .Select(preferences => preferences.DurationFormat);

            ElapsedTime = TimeService
                .CurrentDateTimeObservable
                .CombineLatest(CurrentRunningTimeEntry, (now, te) => (now - te?.Start) ?? TimeSpan.Zero)
                .CombineLatest(durationObservable, (duration, format) => duration.ToFormattedString(format))
                .AsDriver(schedulerProvider);

            NumberOfSyncFailures = interactorFactory
                .GetItemsThatFailedToSync()
                .Execute()
                .Select(i => i.Count())
                .AsDriver(schedulerProvider);

            ShouldReloadTimeEntryLog = Observable.Merge(
                TimeService.MidnightObservable.SelectUnit(),
                TimeService.SignificantTimeChangeObservable.SelectUnit())
                .AsDriver(schedulerProvider);

            Refresh = rxActionFactory.FromAsync(refresh);
            OpenSettings = rxActionFactory.FromAsync(openSettings);
            OpenSyncFailures = rxActionFactory.FromAsync(openSyncFailures);
            SelectTimeEntry = rxActionFactory.FromAsync<EditTimeEntryInfo>(timeEntrySelected);
            StartTimeEntry = rxActionFactory.FromAsync<bool>(startTimeEntry, IsTimeEntryRunning.Invert());
            StopTimeEntry = rxActionFactory.FromObservable<TimeEntryStopOrigin>(stopTimeEntry, IsTimeEntryRunning);
            ContinueTimeEntry = rxActionFactory.FromAsync<ContinueTimeEntryInfo, IThreadSafeTimeEntry>(continueTimeEntry);

            ShouldShowRatingView = Observable.Merge(
                    ratingViewExperiment.RatingViewShouldBeVisible,
                    RatingViewModel.HideRatingView.SelectValue(false),
                    hideRatingView.AsObservable().SelectValue(false)
                )
                .Select(canPresentRating)
                .DistinctUntilChanged()
                .Do(trackRatingViewPresentation)
                .AsDriver(schedulerProvider);

            OnboardingStorage.StopButtonWasTappedBefore
                             .Subscribe(hasBeen => hasStopButtonEverBeenUsed = hasBeen)
                             .DisposedBy(disposeBag);

            if (platformInfo.Platform == Platform.Giskard)
                analyticsService.ApplicationInstallLocation.Track(platformInfo.InstallLocation);

            SyncProgressState
                .Subscribe(postAccessibilityAnnouncementAboutSync)
                .DisposedBy(disposeBag);
        }

        public void Track(ITrackableEvent e)
        {
            analyticsService.Track(e);
        }

        private bool canPresentRating(bool shouldBeVisible)
        {
            if (!shouldBeVisible)
                return false;

            var wasShownMoreThanOnce = OnboardingStorage.NumberOfTimesRatingViewWasShown() > 1;
            if (wasShownMoreThanOnce)
                return false;

            var lastOutcome = OnboardingStorage.RatingViewOutcome();
            if (lastOutcome != null)
            {
                var thereIsInteractionFormLastTime = lastOutcome != RatingViewOutcome.NoInteraction;
                if (thereIsInteractionFormLastTime)
                    return false;
            }

            var lastOutcomeTime = OnboardingStorage.RatingViewOutcomeTime();
            if (lastOutcomeTime != null)
            {
                var oneDayHasNotPassedSinceLastTime = lastOutcomeTime + TimeSpan.FromHours(24) > TimeService.CurrentDateTime;
                if (oneDayHasNotPassedSinceLastTime && !wasShownMoreThanOnce)
                    return false;
            }

            return true;
        }

        private void trackRatingViewPresentation(bool shouldBeVisible)
        {
            if (!shouldBeVisible)
                return;

            analyticsService.RatingViewWasShown.Track();
            OnboardingStorage.SetDidShowRatingView();
            OnboardingStorage.SetRatingViewOutcome(RatingViewOutcome.NoInteraction, TimeService.CurrentDateTime);

            TimeService.RunAfterDelay(TimeSpan.FromMinutes(ratingViewTimeout), () =>
            {
                shouldHideRatingViewIfStillVisible = true;
                hideRatingView.OnNext(Unit.Default);
            });
        }

        private async Task continueMostRecentEntry()
        {
            await interactorFactory.ContinueMostRecentTimeEntry().Execute();
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            viewDisappearedAsync();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            SuggestionsViewModel.ViewAppeared();
        }

        private async Task viewDisappearedAsync()
        {
            await TimeEntriesViewModel.FinalizeDelayDeleteTimeEntryIfNeeded();
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            ViewAppearingAsync();
        }

        internal async Task ViewAppearingAsync()
        {
            hideRatingViewIfStillVisibleAfterDelay();
            await handleNoWorkspaceState();
            handleNoDefaultWorkspaceState();

            if (await shouldShowJanuary2020Campaign())
            {
                await navigate<January2020CampaignViewModel>();
            }
        }

        private void hideRatingViewIfStillVisibleAfterDelay()
        {
            if (shouldHideRatingViewIfStillVisible)
            {
                shouldHideRatingViewIfStillVisible = false;
                hideRatingView.OnNext(Unit.Default);
            }
        }

        private async Task handleNoWorkspaceState()
        {
            if (accessRestrictionStorage.HasNoWorkspace() && !noWorkspaceViewPresented)
            {
                noWorkspaceViewPresented = true;
                await Navigate<NoWorkspaceViewModel, Unit>();
                noWorkspaceViewPresented = false;
            }
        }

        private async Task handleNoDefaultWorkspaceState()
        {
            if (!accessRestrictionStorage.HasNoWorkspace() && accessRestrictionStorage.HasNoDefaultWorkspace() && !noDefaultWorkspaceViewPresented)
            {
                noDefaultWorkspaceViewPresented = true;
                await Navigate<SelectDefaultWorkspaceViewModel, Unit>();
                noDefaultWorkspaceViewPresented = false;
            }
        }

        private async Task<bool> shouldShowJanuary2020Campaign()
        {
            var isTheAppShownInJapanese = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("JA", StringComparison.InvariantCultureIgnoreCase);
            if (isTheAppShownInJapanese)
                return false;

            var isDisabled = remoteConfigService.GetJanuary2020CampaignConfiguration().Option == January2020CampaignConfiguration.AvailableOption.None;
            if (isDisabled)
                return false;

            var wasAlreadyShown = OnboardingStorage.WasJanuary2020CampaignShown();
            if (wasAlreadyShown)
                return false;

            if (!lastTimeUsageStorage.LastLogin.HasValue)
                return false;

            var isNotANewUser =
                TimeService.CurrentDateTime - lastTimeUsageStorage.LastLogin.Value > TimeSpan.FromHours(48)
                    && await TimeEntriesCount.FirstAsync() >= 2;

            return isNotANewUser;
        }

        private Task openSettings()
        {
            return navigate<SettingsViewModel>();
        }

        private Task openSyncFailures()
            => navigate<SyncFailuresViewModel>();

        private Task startTimeEntry(bool useDefaultMode)
        {
            var initializeInManualMode = useDefaultMode == userPreferences.IsManualModeEnabled;

            OnboardingStorage.StartButtonWasTapped();

            if (hasStopButtonEverBeenUsed)
                OnboardingStorage.SetNavigatedAwayFromMainViewAfterStopButton();

            var requestCameFromLongPress = !useDefaultMode;
            var parameter = initializeInManualMode
                ? StartTimeEntryParameters.ForManualMode(TimeService.CurrentDateTime, requestCameFromLongPress)
                : StartTimeEntryParameters.ForTimerMode(TimeService.CurrentDateTime, requestCameFromLongPress);

            return navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(parameter);
        }

        private async Task<IThreadSafeTimeEntry> continueTimeEntry(ContinueTimeEntryInfo continueInfo)
        {
            var continuedTimeEntry = await interactorFactory
                .ContinueTimeEntry(continueInfo.Id, continueInfo.ContinueMode)
                .Execute()
                .ConfigureAwait(false);
               
            analyticsService.TimeEntryContinued.Track(
                originFromContinuationMode(continueInfo.ContinueMode),
                continueInfo.IndexInLog,
                continueInfo.DayInLog,
                continueInfo.DaysInThePast);

            OnboardingStorage.SetTimeEntryContinued();

            return continuedTimeEntry;

            ContinueTimeEntryOrigin originFromContinuationMode(ContinueTimeEntryMode mode)
            {
                switch (mode)
                {
                    case SingleTimeEntrySwipe:
                        return Swipe;
                    case SingleTimeEntryContinueButton:
                        return ContinueButton;
                    case TimeEntriesGroupSwipe:
                        return GroupSwipe;
                    case TimeEntriesGroupContinueButton:
                        return GroupContinueButton;
                }

                throw new InvalidEnumArgumentException($"Unexpected continue time entry mode {mode}");
            }
        }

        private async Task timeEntrySelected(EditTimeEntryInfo editTimeEntryInfo)
        {
            OnboardingStorage.TimeEntryWasTapped();

            analyticsService.EditViewOpened.Track(editTimeEntryInfo.Origin);
            await navigate<EditTimeEntryViewModel, long[]>(editTimeEntryInfo.Ids);
        }

        private async Task refresh()
        {
            await syncManager.ForceFullSync();
        }

        private IObservable<Unit> stopTimeEntry(TimeEntryStopOrigin origin)
        {
            OnboardingStorage.StopButtonWasTapped();

            return interactorFactory
                .StopTimeEntry(TimeService.CurrentDateTime, origin)
                .Execute()
                .ToObservable()
                .Do(syncManager.InitiatePushSync)
                .SelectUnit();
        }

        private Task navigate<TModel, TParameters>(TParameters value)
            where TModel : ViewModelWithInput<TParameters>
        {
            if (hasStopButtonEverBeenUsed)
                OnboardingStorage.SetNavigatedAwayFromMainViewAfterStopButton();

            return Navigate<TModel, TParameters>(value);
        }

        private Task navigate<TModel>()
            where TModel : ViewModel
        {
            if (hasStopButtonEverBeenUsed)
                OnboardingStorage.SetNavigatedAwayFromMainViewAfterStopButton();

            return Navigate<TModel>();
        }

        private void postAccessibilityAnnouncementAboutSync(SyncProgress syncProgress)
        {
            string message = "";
            switch (syncProgress)
            {
                case SyncProgress.Failed:
                    message = Resources.SyncFailed;
                    break;
                case SyncProgress.OfflineModeDetected:
                    message = Resources.SyncFailedOffline;
                    break;
                case SyncProgress.Synced:
                    message = Resources.SuccessfullySyncedData;
                    break;

                //These 2 are not announced
                case SyncProgress.Syncing:
                    return;
                case SyncProgress.Unknown:
                    return;
            }

            accessibilityService.PostAnnouncement(message);
        }
    }
}
