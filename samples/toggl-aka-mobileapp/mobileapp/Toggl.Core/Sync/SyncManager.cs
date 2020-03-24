using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;
using Toggl.Core.Services;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using static Toggl.Core.Sync.SyncState;

namespace Toggl.Core.Sync
{
    public sealed class SyncManager : ISyncManager
    {
        private readonly object stateLock = new object();
        private readonly ISyncStateQueue queue;
        private readonly IStateMachineOrchestrator orchestrator;
        private readonly IAnalyticsService analyticsService;
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;
        private readonly ITimeService timeService;
        private readonly IAutomaticSyncingService automaticSyncingService;

        private bool isFrozen;

        private readonly ISubject<SyncProgress> progress;
        private readonly ISubject<Exception> errors;

        public bool IsRunningSync { get; private set; }

        public SyncState State => orchestrator.State;

        public IObservable<SyncProgress> ProgressObservable { get; }

        public IObservable<Exception> Errors { get; }

        public SyncManager(
            ISyncStateQueue queue,
            IStateMachineOrchestrator orchestrator,
            IAnalyticsService analyticsService,
            ILastTimeUsageStorage lastTimeUsageStorage,
            ITimeService timeService,
            IAutomaticSyncingService automaticSyncingService)
        {
            Ensure.Argument.IsNotNull(queue, nameof(queue));
            Ensure.Argument.IsNotNull(orchestrator, nameof(orchestrator));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(automaticSyncingService, nameof(automaticSyncingService));

            this.queue = queue;
            this.timeService = timeService;
            this.orchestrator = orchestrator;
            this.analyticsService = analyticsService;
            this.lastTimeUsageStorage = lastTimeUsageStorage;
            this.automaticSyncingService = automaticSyncingService;

            progress = new BehaviorSubject<SyncProgress>(SyncProgress.Unknown);
            ProgressObservable = progress.AsObservable();

            errors = new Subject<Exception>();
            Errors = errors.AsObservable();

            orchestrator.SyncCompleteObservable.Subscribe(syncOperationCompleted);
            isFrozen = false;
            automaticSyncingService.Start(this);
        }

        public IObservable<SyncState> PushSync()
        {
            lock (stateLock)
            {
                queue.QueuePushSync();
                return startSyncIfNeededAndObserve();
            }
        }

        public IObservable<SyncState> ForceFullSync()
        {
            lastTimeUsageStorage.SetFullSyncAttempt(timeService.CurrentDateTime);

            lock (stateLock)
            {
                queue.QueuePullSync();
                return startSyncIfNeededAndObserve().Do(saveTimeWhenReachedSleep);
            }
        }

        public IObservable<SyncState> CleanUp()
        {
            lock (stateLock)
            {
                queue.QueueCleanUp();
                return startSyncIfNeededAndObserve();
            }
        }

        public IObservable<SyncState> PullTimeEntries()
        {
            lock (stateLock)
            {
                queue.QueuePullTimeEntries();
                return startSyncIfNeededAndObserve();
            }
        }

        public IObservable<SyncState> Freeze()
        {
            lock (stateLock)
            {
                if (isFrozen == false)
                {
                    isFrozen = true;
                    orchestrator.Freeze();
                    automaticSyncingService.Stop();
                }

                return IsRunningSync
                    ? syncStatesUntilAndIncludingSleep().LastAsync()
                    : Observable.Return(Sleep);
            }
        }

        private void saveTimeWhenReachedSleep(SyncState state)
        {
            if (state == Sleep)
            {
                lastTimeUsageStorage.SetSuccessfulFullSync(timeService.CurrentDateTime);
            }
        }

        private void syncOperationCompleted(SyncResult result)
        {
            lock (stateLock)
            {
                analyticsService.SyncCompleted.Track();

                IsRunningSync = false;

                if (result is Success)
                {
                    startSyncIfNeeded();
                    if (IsRunningSync == false)
                    {
                        progress.OnNext(SyncProgress.Synced);
                    }

                    return;
                }

                if (result is Error error)
                {
                    processError(error.Exception);
                    return;
                }

                throw new ArgumentException(nameof(result));
            }
        }

        private void processError(Exception error)
        {
            analyticsService.TrackAnonymized(error);
            analyticsService.SyncFailed.Track(error.GetType().FullName, error.Message, error.StackTrace);

            queue.Clear();
            orchestrator.Start(Sleep);

            if (error is NoWorkspaceException
                || error is NoDefaultWorkspaceException)
            {
                errors.OnNext(error);
                progress.OnNext(SyncProgress.Synced);
                return;
            }

            if (error is OfflineException)
            {
                progress.OnNext(SyncProgress.OfflineModeDetected);
                analyticsService.OfflineModeDetected.Track();
            }
            else
            {
                progress.OnNext(SyncProgress.Failed);
            }

            if (error is ClientDeprecatedException
                || error is ApiDeprecatedException
                || error is UnauthorizedException)
            {
                Freeze();
                errors.OnNext(error);
                progress.OnNext(SyncProgress.Failed);
            }
        }

        private IObservable<SyncState> startSyncIfNeededAndObserve()
        {
            startSyncIfNeeded();

            return syncStatesUntilAndIncludingSleep();
        }

        private void startSyncIfNeeded()
        {
            if (IsRunningSync)
                return;

            var state = isFrozen ? Sleep : queue.Dequeue();
            analyticsService.SyncOperationStarted.Track(state.ToString());

            IsRunningSync = state != Sleep;

            if (IsRunningSync && progress.FirstAsync().Wait() != SyncProgress.Syncing)
            {
                progress.OnNext(SyncProgress.Syncing);
            }

            orchestrator.Start(state);
        }

        private IObservable<SyncState> syncStatesUntilAndIncludingSleep()
            => orchestrator.StateObservable
                .TakeWhile(s => s != Sleep)
                .Concat(Observable.Return(Sleep))
                .ConnectedReplay();
    }
}
