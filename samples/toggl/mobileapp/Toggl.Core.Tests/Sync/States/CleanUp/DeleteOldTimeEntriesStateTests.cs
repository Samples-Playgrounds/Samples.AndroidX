using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.States.CleanUp;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.CleanUp
{
    public sealed class DeleteOldTimeEntriesStateTests
    {
        private readonly DateTimeOffset now = DateTimeOffset.UtcNow;

        private readonly ITimeService timeService = Substitute.For<ITimeService>();
        private readonly ITimeEntriesSource dataSource = Substitute.For<ITimeEntriesSource>();

        private readonly DeleteOldTimeEntriesState state;

        public DeleteOldTimeEntriesStateTests()
        {
            state = new DeleteOldTimeEntriesState(timeService, dataSource);

            timeService.CurrentDateTime.Returns(now);
        }

        [Fact, LogIfTooSlow]
        public async Task DeletesOldSyncedTimeEntries()
        {
            var timeEntries = getTimeEntries();
            configureDataSourceReturn(timeEntries);

            await state.Start().SingleAsync();

            await dataSource.Received()
                .DeleteAll(Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => tes.Count() == 10));
        }

        [Fact, LogIfTooSlow]
        public async Task OnlyDeletesSyncedTimeEntries()
        {
            var timeEntries = getTimeEntries();
            timeEntries.Take(5).ForEach(te => te.SyncStatus = SyncStatus.SyncNeeded);
            configureDataSourceReturn(timeEntries);

            await state.Start().SingleAsync();

            await dataSource.Received()
                .DeleteAll(Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => tes.Count() == 5));
        }

        [Fact, LogIfTooSlow]
        public async Task OnlyDeletesTimeEntriesOlderThanEightWeeks()
        {
            var timeEntries = getTimeEntries();
            timeEntries.Take(5).ForEach(te => te.Start = now.AddDays(-7 * 8).AddDays(1));
            configureDataSourceReturn(timeEntries);

            await state.Start().SingleAsync();

            await dataSource.Received()
                .DeleteAll(Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => tes.Count() == 5));
        }

        private void configureDataSourceReturn(IEnumerable<IDatabaseTimeEntry> timeEntries)
        {
            dataSource
                .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                .Returns(callInfo =>
                {
                    var filteredTimeEntries = timeEntries.Where(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>());
                    return Observable.Return(filteredTimeEntries.Cast<IThreadSafeTimeEntry>());
                });
        }

        private IList<MockTimeEntry> getTimeEntries()
            => Enumerable.Range(0, 10).Select(i => new MockTimeEntry { Start = now.AddDays(-(7 * 8)).AddDays(-1) }).ToList();
    }
}
