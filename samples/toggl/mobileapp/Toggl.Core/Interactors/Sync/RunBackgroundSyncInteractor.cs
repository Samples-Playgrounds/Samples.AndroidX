using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.Sync;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public class RunBackgroundSyncInteractor : IInteractor<IObservable<SyncOutcome>>
    {
        private readonly ISyncManager syncManager;
        private readonly IAnalyticsService analyticsService;

        public RunBackgroundSyncInteractor(
            ISyncManager syncManager,
            IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.syncManager = syncManager;
            this.analyticsService = analyticsService;
        }

        public IObservable<SyncOutcome> Execute()
        {
            analyticsService.BackgroundSyncStarted.Track();
            return syncManager.PullTimeEntries()
                .LastAsync()
                .Select(_ => SyncOutcome.NewData)
                .Catch((Exception error) => syncFailed(error))
                .Do(outcome =>
                    analyticsService.BackgroundSyncFinished.Track(outcome.ToString()));
        }

        private IObservable<SyncOutcome> syncFailed(Exception error)
        {
            analyticsService.BackgroundSyncFailed
                .Track(error.GetType().FullName, error.Message, error.StackTrace);
            return Observable.Return(SyncOutcome.Failed);
        }
    }
}
