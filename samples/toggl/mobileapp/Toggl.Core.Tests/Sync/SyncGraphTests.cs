using System.Reactive.Concurrency;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Core.UI;
using Toggl.Networking;
using Toggl.Storage;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.Sync
{
    public sealed class SyncGraphTests
    {
        public sealed class TheSyncGraph
        {
            [Fact, LogIfTooSlow]
            public void HasNoLooseEnds()
            {
                var configurator = configureTogglSyncGraph();

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().BeEmpty();
            }
        }

        private static TestConfigurator configureTogglSyncGraph()
        {
            var configurator = new TestConfigurator();
            var entryPoints = new StateMachineEntryPoints();

            var dependencyContainer = new TestDependencyContainer();
            dependencyContainer.MockKeyValueStorage = Substitute.For<IKeyValueStorage>();
            dependencyContainer.MockPushNotificationsTokenService = Substitute.For<IPushNotificationsTokenService>();
            dependencyContainer.MockTimeService = Substitute.For<ITimeService>();
            dependencyContainer.MockRemoteConfigService = Substitute.For<IRemoteConfigService>();
            dependencyContainer.MockPushNotificationsTokenStorage = Substitute.For<IPushNotificationsTokenStorage>();

            configurator.AllDistinctStatesInOrder.Add(entryPoints);

            TogglSyncManager.ConfigureTransitions(
                configurator,
                Substitute.For<ITogglDatabase>(),
                Substitute.For<ITogglApi>(),
                Substitute.For<ITogglDataSource>(),
                Substitute.For<IScheduler>(),
                Substitute.For<ITimeService>(),
                Substitute.For<IAnalyticsService>(),
                Substitute.For<ILastTimeUsageStorage>(),
                entryPoints,
                Substitute.For<ISyncStateQueue>(),
                dependencyContainer
            );

            return configurator;
        }
    }
}
