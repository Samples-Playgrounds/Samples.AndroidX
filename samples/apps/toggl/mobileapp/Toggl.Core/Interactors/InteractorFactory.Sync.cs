using System;
using System.Collections.Generic;
using Toggl.Core.Models;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<IEnumerable<SyncFailureItem>>> GetItemsThatFailedToSync()
            => new GetItemsThatFailedToSyncInteractor(dataSource);

        public IInteractor<IObservable<bool>> HasFinishedSyncBefore()
            => new HasFinsihedSyncBeforeInteractor(dataSource);

        public IInteractor<IObservable<SyncOutcome>> RunBackgroundSync()
            => new RunBackgroundSyncInteractor(syncManager, analyticsService);

        public IInteractor<IObservable<bool>> ContainsPlaceholders()
            => new ContainsPlaceholdersInteractor(dataSource);

        public IInteractor<IObservable<SyncOutcome>> RunPushNotificationInitiatedSyncInForeground()
            => new RunSyncInteractor(
                syncManager,
                analyticsService,
                PushNotificationSyncSourceState.Foreground);

        public IInteractor<IObservable<SyncOutcome>> RunPushNotificationInitiatedSyncInBackground()
            => new RunSyncInteractor(
                syncManager,
                analyticsService,
                PushNotificationSyncSourceState.Background);
    }
}
