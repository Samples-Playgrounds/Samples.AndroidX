using NSubstitute;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.Services
{
    public sealed class AutomaticSyncingServiceTests
    {
        public abstract class BaseAutomaticSyncingServiceTest
        {
            protected IAutomaticSyncingService AutomaticSyncingService { get; }
            protected ISubject<TimeSpan> AppResumedFromBackground { get; } = new Subject<TimeSpan>();
            protected ISyncManager SyncManager { get; } = Substitute.For<ISyncManager>();
            protected IBackgroundService BackgroundService { get; } = Substitute.For<IBackgroundService>();
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ILastTimeUsageStorage LastTimeUsageStorage { get; } = Substitute.For<ILastTimeUsageStorage>();

            protected BaseAutomaticSyncingServiceTest()
            {
                BackgroundService.AppResumedFromBackground.Returns(AppResumedFromBackground.AsObservable());
                AutomaticSyncingService = new AutomaticSyncingService(BackgroundService, TimeService, LastTimeUsageStorage);
            }
        }

        public sealed class TheStartWithMethod : BaseAutomaticSyncingServiceTest
        {
            [Fact, LogIfTooSlow]
            public async Task SubscribesToResumingFromBackgroundSignal()
            {
                AutomaticSyncingService.Start(SyncManager);
                AppResumedFromBackground.OnNext(Core.Services.AutomaticSyncingService.MinimumTimeInBackgroundForFullSync);

                await SyncManager.Received().ForceFullSync();
            }

            [Fact, LogIfTooSlow]
            public async Task SubscribesToTheMidnightObservable()
            {
                var errors = new Subject<Exception>();
                SyncManager.Errors.Returns(errors);

                errors.OnNext(new Exception());
                AppResumedFromBackground.OnNext(TimeSpan.FromHours(1));

                await SyncManager.DidNotReceive().ForceFullSync();
            }

            [Fact, LogIfTooSlow]
            public async Task StopsWhenSyncManagerFails()
            {
                var midnightSubject = new Subject<DateTimeOffset>();
                TimeService.MidnightObservable.Returns(midnightSubject);

                AutomaticSyncingService.Start(SyncManager);
                midnightSubject.OnNext(new DateTimeOffset(2018, 12, 17, 00, 00, 00, TimeSpan.Zero));

                await SyncManager.Received().CleanUp();
            }
        }

        public sealed class TheStopMethod : BaseAutomaticSyncingServiceTest
        {
            [Fact, LogIfTooSlow]
            public async Task UnsubscribesFromTheSignalWhenStopped()
            {
                var subject = new Subject<TimeSpan>();
                BackgroundService.AppResumedFromBackground.Returns(subject.AsObservable());
                var errors = Observable.Never<Exception>();
                SyncManager.Errors.Returns(errors);

                AutomaticSyncingService.Start(SyncManager);
                SyncManager.ClearReceivedCalls();
                AutomaticSyncingService.Stop();

                subject.OnNext(Core.Services.AutomaticSyncingService.MinimumTimeInBackgroundForFullSync);

                await SyncManager.DidNotReceive().ForceFullSync();
            }
        }
    }
}
