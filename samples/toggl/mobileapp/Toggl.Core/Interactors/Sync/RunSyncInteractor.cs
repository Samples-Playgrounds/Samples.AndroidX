using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Sync;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public class RunSyncInteractor : IInteractor<IObservable<SyncOutcome>>
    {
        private readonly ISyncManager syncManager;
        private readonly IAnalyticsService analyticsService;
        private readonly PushNotificationSyncSourceState sourceState;

        public RunSyncInteractor(
            ISyncManager syncManager,
            IAnalyticsService analyticsService,
            PushNotificationSyncSourceState sourceState)
        {
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(sourceState, nameof(sourceState));

            this.syncManager = syncManager;
            this.analyticsService = analyticsService;
            this.sourceState = sourceState;
        }

        public IObservable<SyncOutcome> Execute()
        {
            analyticsService.PushNotificationSyncStarted.Track(sourceState.ToString());

            var syncAction = sourceState == PushNotificationSyncSourceState.Foreground
                ? syncManager.PullTimeEntries()
                    .LastAsync()
                    .ThenExecute(syncManager.ForceFullSync)
                : syncManager.PullTimeEntries();

            return syncAction.LastAsync()
                .Select(_ => SyncOutcome.NewData)
                .Catch((Exception error) => syncFailed(error))
                .Do(outcome =>
                {
                    analyticsService.PushNotificationSyncFinished.Track(sourceState.ToString());
                });
        }

        private IObservable<SyncOutcome> syncFailed(Exception error)
        {
            analyticsService.PushNotificationSyncFailed.Track(sourceState.ToString(), error.GetType().FullName, error.Message, error.StackTrace);
            return Observable.Return(SyncOutcome.Failed);
        }
    }
}
