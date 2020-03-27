using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Tests.Generators;
using Xunit;
using SyncOutcome = Toggl.Core.Models.SyncOutcome;
using SyncState = Toggl.Core.Sync.SyncState;

namespace Toggl.Core.Tests.Interactors.Workspace
{
    public sealed class RunSyncInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useSyncManager, bool useAnalyticsService)
            {
                Action tryingToConstructWithNull = () =>
                    new RunSyncInteractor(
                        useSyncManager ? SyncManager : null,
                        useAnalyticsService ? AnalyticsService : null,
                        PushNotificationSyncSourceState.Foreground
                    );

                tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
            }
        }

        public class TheExecuteMethod : BaseInteractorTests
        {
            public static IEnumerable<object[]> SyncSources =>
                new List<object[]>
                {
                    new object[] { PushNotificationSyncSourceState.Foreground },
                    new object[] { PushNotificationSyncSourceState.Background }
                };

            public RunSyncInteractor CreateSyncInteractor(PushNotificationSyncSourceState sourceState)
            {
                return new RunSyncInteractor(
                    SyncManager,
                    AnalyticsService,
                    sourceState
                );
            }
        }

        public class WhenSyncFails : TheExecuteMethod
        {
            public WhenSyncFails()
            {
                SyncManager.ForceFullSync().Returns(Observable.Throw<SyncState>(new Exception()));
                SyncManager.PullTimeEntries().Returns(Observable.Throw<SyncState>(new Exception()));
                SyncManager.PushSync().Returns(Observable.Throw<SyncState>(new Exception()));
                SyncManager.CleanUp().Returns(Observable.Throw<SyncState>(new Exception()));
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SyncSources))]
            public async Task ReturnsFailedIfSyncFails(PushNotificationSyncSourceState sourceState)
            {
                var interactor = CreateSyncInteractor(sourceState);
                (await interactor.Execute().SingleAsync()).Should().Be(SyncOutcome.Failed);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SyncSources))]
            public async Task TracksIfSyncFails(PushNotificationSyncSourceState sourceState)
            {
                var exception = new Exception();
                SyncManager.ForceFullSync().Returns(Observable.Throw<SyncState>(exception));
                var interactor = CreateSyncInteractor(sourceState);

                await interactor.Execute().SingleAsync();

                AnalyticsService.PushNotificationSyncStarted.Received().Track(sourceState.ToString());
                AnalyticsService.PushNotificationSyncFinished.Received().Track(sourceState.ToString());
                AnalyticsService.PushNotificationSyncFailed.Received().Track(sourceState.ToString(), exception.GetType().FullName, exception.Message, exception.StackTrace);
            }
        }

        public class WhenSyncSucceeds : TheExecuteMethod
        {
            public WhenSyncSucceeds()
            {
                SyncManager.ForceFullSync().Returns(Observable.Return(SyncState.Sleep));
                SyncManager.PullTimeEntries().Returns(Observable.Return(SyncState.Sleep));
                SyncManager.PushSync().Returns(Observable.Return(SyncState.Sleep));
                SyncManager.CleanUp().Returns(Observable.Return(SyncState.Sleep));
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SyncSources))]
            public async Task ReturnsNewDataIfSyncSucceeds(PushNotificationSyncSourceState sourceState)
            {
                var interactor = CreateSyncInteractor(sourceState);
                (await interactor.Execute().SingleAsync()).Should().Be(SyncOutcome.NewData);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SyncSources))]
            public async Task TracksIfSyncSucceeds(PushNotificationSyncSourceState sourceState)
            {
                SyncManager.ForceFullSync().Returns(Observable.Return(SyncState.Sleep));
                var interactor = CreateSyncInteractor(sourceState);

                await interactor.Execute().SingleAsync();

                AnalyticsService.PushNotificationSyncStarted.Received().Track(sourceState.ToString());
                AnalyticsService.PushNotificationSyncFinished.Received().Track(sourceState.ToString());
                AnalyticsService.PushNotificationSyncFailed.DidNotReceive().Track(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            }
        }
    }
}
