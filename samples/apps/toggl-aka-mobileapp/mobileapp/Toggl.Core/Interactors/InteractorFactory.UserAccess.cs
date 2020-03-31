using System;
using System.Reactive;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors.UserAccess;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<Unit>> Logout(LogoutSource source)
            => new LogoutInteractor(
                analyticsService,
                notificationService,
                shortcutCreator,
                syncManager,
                database,
                userPreferences,
                privateSharedStorageService,
                userAccessManager,
                this,
                source);
    }
}
