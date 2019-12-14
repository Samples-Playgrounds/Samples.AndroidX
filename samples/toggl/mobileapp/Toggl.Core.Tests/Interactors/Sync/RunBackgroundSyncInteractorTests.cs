using FluentAssertions;
using NSubstitute;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Tests.Generators;
using Xunit;
using SyncOutcome = Toggl.Core.Models.SyncOutcome;
using SyncState = Toggl.Core.Sync.SyncState;

namespace Toggl.Core.Tests.Interactors.Workspace
{
    public sealed class RunBackgroundSyncInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useSyncManager, bool useAnalyticsService)
            {
                Action tryingToConstructWithNull = () =>
                    new RunBackgroundSyncInteractor(
                        useSyncManager ? SyncManager : null,
                        useAnalyticsService ? AnalyticsService : null);

                tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private readonly RunBackgroundSyncInteractor interactor;

            public TheExecuteMethod()
            {
                interactor = new RunBackgroundSyncInteractor(SyncManager, AnalyticsService);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsFailedIfSyncFails()
            {
                SyncManager.PullTimeEntries().Returns(Observable.Throw<SyncState>(new Exception()));
                (await interactor.Execute().SingleAsync()).Should().Be(SyncOutcome.Failed);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNewDataIfSyncSucceeds()
            {
                SyncManager.PullTimeEntries().Returns(Observable.Return(SyncState.Sleep));
                (await interactor.Execute().SingleAsync()).Should().Be(SyncOutcome.NewData);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksIfSyncSucceeds()
            {
                SyncManager.PullTimeEntries().Returns(Observable.Return(SyncState.Sleep));
                await interactor.Execute().SingleAsync();
                AnalyticsService.BackgroundSyncStarted.Received().Track();
                AnalyticsService.BackgroundSyncFinished.Received().Track(nameof(SyncOutcome.NewData));
                AnalyticsService.BackgroundSyncFailed.DidNotReceive().Track(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public async Task TracksIfSyncFails()
            {
                var exception = new Exception();
                SyncManager.PullTimeEntries().Returns(Observable.Throw<SyncState>(exception));
                await interactor.Execute().SingleAsync();
                AnalyticsService.BackgroundSyncStarted.Received().Track();
                AnalyticsService.BackgroundSyncFinished.Received().Track(nameof(SyncOutcome.Failed));
                AnalyticsService.BackgroundSyncFailed.Received()
                    .Track(exception.GetType().FullName, exception.Message, exception.StackTrace);
            }
        }
    }
}
