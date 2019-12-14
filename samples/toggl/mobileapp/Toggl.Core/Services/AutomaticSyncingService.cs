using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.Services
{
    public sealed class AutomaticSyncingService : IAutomaticSyncingService
    {
        public static TimeSpan MinimumTimeInBackgroundForFullSync { get; } = TimeSpan.FromMinutes(5);

        private readonly IBackgroundService backgroundService;
        private readonly ITimeService timeService;
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;

        private CompositeDisposable syncingDisposeBag;

        public AutomaticSyncingService(
            IBackgroundService backgroundService,
            ITimeService timeService,
            ILastTimeUsageStorage lastTimeUsageStorage)
        {
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));

            this.backgroundService = backgroundService;
            this.timeService = timeService;
            this.lastTimeUsageStorage = lastTimeUsageStorage;
        }

        public void Start(ISyncManager syncManager)
        {
            Stop();

            backgroundService.AppResumedFromBackground
                .Where(timeInBackground => timeInBackground >= MinimumTimeInBackgroundForFullSync
                    || hasCreatedPlaceholdersWhileInBackground(timeInBackground))
                .SelectMany(_ => syncManager.ForceFullSync())
                .Subscribe()
                .DisposedBy(syncingDisposeBag);

            timeService.MidnightObservable
                .Subscribe(_ => syncManager.CleanUp())
                .DisposedBy(syncingDisposeBag);
        }

        public void Stop()
        {
            syncingDisposeBag?.Dispose();
            syncingDisposeBag = new CompositeDisposable();
        }

        private bool hasCreatedPlaceholdersWhileInBackground(TimeSpan timeInBackground)
        {
            var lastTimePlaceholderWasCreated =
                lastTimeUsageStorage.LastTimePlaceholdersWereCreated ?? default(DateTimeOffset);
            var timeSinceLastPlaceholderWasCreated = timeService.CurrentDateTime - lastTimePlaceholderWasCreated;
            return timeSinceLastPlaceholderWasCreated <= timeInBackground;
        }
    }
}
