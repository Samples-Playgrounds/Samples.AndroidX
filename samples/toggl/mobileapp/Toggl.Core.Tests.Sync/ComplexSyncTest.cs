using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Sync.Exceptions;
using Toggl.Core.Tests.Sync.Helpers;
using Toggl.Core.Tests.Sync.State;
using Toggl.Networking.Tests.Integration;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.Sync
{
    public abstract class ComplexSyncTest : IDisposable
    {
        private readonly SyncStorage storage;

        protected ComplexSyncTest()
        {
            storage = new SyncStorage();
            storage.Clear().Wait();
        }

        public void Dispose()
        {
            storage.Clear().Wait();
        }

        [Fact, LogTestInfo]
        public async Task Execute()
        {
            // Initialize
            var server = await Server.Factory.Create();
            var appServices = new AppServices(server.Api, storage.Database);

            // Arrange
            ArrangeServices(appServices);

            var definedServerState = ArrangeServerState(server.InitialServerState);
            await server.Push(definedServerState);

            var actualServerStateBefore = await server.PullCurrentState();
            var definedDatabaseState = ArrangeDatabaseState(actualServerStateBefore);
            await storage.Store(definedDatabaseState);

            // Act
            await Act(appServices.SyncManager, appServices);

            // Assert
            var finalDatabaseState = await storage.LoadCurrentState();
            var finalServerState = await server.PullCurrentState();
            AssertFinalState(appServices, finalServerState, finalDatabaseState);
        }

        protected virtual void ArrangeServices(AppServices services) { }
        protected abstract ServerState ArrangeServerState(ServerState initialServerState);
        protected abstract DatabaseState ArrangeDatabaseState(ServerState serverState);

        protected virtual async Task Act(ISyncManager syncManager, AppServices services)
        {
            var progressMonitoring = MonitorProgress(syncManager);
            await syncManager.ForceFullSync();
            await progressMonitoring;
        }

        protected abstract void AssertFinalState(AppServices services, ServerState finalServerState, DatabaseState finalDatabaseState);

        protected IObservable<SyncProgress> MonitorProgress(ISyncManager syncManager)
            => syncManager.ProgressObservable
                .ThrowIf(
                    progress => progress == SyncProgress.OfflineModeDetected,
                    new SyncProcessFailedException(
                        "The syncing process failed because the device running the test is offline."))
                .ThrowIf(
                    progress => progress == SyncProgress.Failed,
                    new SyncProcessFailedException(
                        "The syncing process failed for some unknown reason. Consider debugging the test " +
                        "and putting a breakpoint into `SyncManager.processError` method"))
                .Where(progress => progress == SyncProgress.Synced)
                .FirstAsync();
    }
}
