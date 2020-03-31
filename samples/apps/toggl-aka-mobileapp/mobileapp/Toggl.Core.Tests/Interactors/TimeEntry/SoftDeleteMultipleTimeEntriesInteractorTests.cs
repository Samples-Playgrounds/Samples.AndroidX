using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public sealed class SoftDeleteMultipleTimeEntriesInteractorTests
    {
        public class SoftDeleteMultipleTimeEntriesInteractorTest : BaseInteractorTests
        {
            private long[] ids;
            private IInteractor<IObservable<Unit>> testedInteractor;
            private IInteractorFactory interactorFactory;
            private ITimeEntriesSource dataSource;

            private void setupTest()
            {
                ids = new long[] { 4, 8, 15, 16, 23, 42 };

                interactorFactory = Substitute.For<IInteractorFactory>();

                var teInteractor = Substitute.For<IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>>>();
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                var tes = Enumerable.Range(0, 5)
                    .Select((_, index) => new MockTimeEntry(index, workspace))
                    .ToList();
                var observable = Observable.Return(tes);
                teInteractor.Execute().Returns(observable);
                dataSource = Substitute.For<ITimeEntriesSource>();
                DataSource.TimeEntries.Returns(dataSource);

                interactorFactory
                    .GetMultipleTimeEntriesById(Arg.Any<long[]>())
                    .Returns(teInteractor);

                testedInteractor = new SoftDeleteMultipleTimeEntriesInteractor(DataSource.TimeEntries, TimeService, SyncManager, interactorFactory, ids);
            }

            [Fact, LogIfTooSlow]
            public void PropagatesCorrectIdsToGetMultipleTimeEntriesInteractor()
            {
                setupTest();

                testedInteractor.Execute().Wait();

                interactorFactory.Received().GetMultipleTimeEntriesById(
                    Arg.Is<long[]>(propagatedIds => propagatedIds.SetEquals(ids, null)));
            }

            [Fact, LogIfTooSlow]
            public void CallsBatchUpdateWithUpdatedEntities()
            {
                var now = new DateTimeOffset(2019, 11, 4, 8, 33, 00, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                setupTest();

                testedInteractor.Execute().Wait();

                dataSource.Received().BatchUpdate(
                    Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(entries => entries.All(
                        entry =>
                            entry.IsDeleted
                            && entry.SyncStatus == Storage.SyncStatus.SyncNeeded
                            && entry.At == now))
                );
            }

            [Fact, LogIfTooSlow]
            public void InitiatesPushSync()
            {
                setupTest();

                testedInteractor.Execute().Wait();

                SyncManager.Received().PushSync();
            }
        }
    }
}
