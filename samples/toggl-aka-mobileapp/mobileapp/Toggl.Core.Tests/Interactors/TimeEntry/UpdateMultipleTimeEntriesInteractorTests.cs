using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.DTOs;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Interactors.TimeEntry
{
    public class UpdateMultipleTimeEntriesInteractorTests : BaseInteractorTests
    {
        private static DateTimeOffset now = new DateTimeOffset(2018, 05, 14, 18, 00, 00, TimeSpan.Zero);

        private readonly IThreadSafeTimeEntry[] timeEntries;
        private readonly ITimeEntriesSource timeEntriesSource;

        public UpdateMultipleTimeEntriesInteractorTests()
        {
            int count = 8;

            timeEntries = new IThreadSafeTimeEntry[count];

            for (int i = 0; i < 8; i++)
            {
                timeEntries[i] =
                    Models.TimeEntry.Builder
                    .Create(i + 10)
                    .SetUserId(10)
                    .SetDescription($"Description {i + 1}")
                    .SetWorkspaceId(11)
                    .SetSyncStatus(SyncStatus.InSync)
                    .SetAt(now.AddDays(-1))
                    .SetStart(now.AddHours(-2))
                    .Build();
            }

            var observable = Observable.Return(timeEntries);
            timeEntriesSource = Substitute.For<ITimeEntriesSource>();
            DataSource.TimeEntries.Returns(timeEntriesSource);
            timeEntriesSource.GetByIds(Arg.Any<long[]>()).Returns(observable);
        }

        private EditTimeEntryDto[] prepareTimeEntries()
            => timeEntries.Select(te => createDto(te)).ToArray();

        private EditTimeEntryDto createDto(IThreadSafeTimeEntry timeEntry)
        {
            return new EditTimeEntryDto
            {
                Id = timeEntry.Id,
                Description = timeEntry.Description + " Updated",
                StartTime = DateTimeOffset.UtcNow,
                ProjectId = 13,
                Billable = true,
                WorkspaceId = 71,
                TagIds = new long[] { 1, 10, 34, 42 }
            };
        }



        private class TimeEntriesCollectionComparer
        {
            private Func<IThreadSafeTimeEntry, EditTimeEntryDto, bool> compareFunction;
            private Dictionary<long, EditTimeEntryDto> timeEntriesMap;

            public TimeEntriesCollectionComparer(IEnumerable<EditTimeEntryDto> timeEntries, Func<IThreadSafeTimeEntry, EditTimeEntryDto, bool> compareFunction)
            {
                timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
                this.compareFunction = compareFunction;
            }

            public bool CompareTo(IEnumerable<IThreadSafeTimeEntry> collection)
                => collection.Count() == timeEntriesMap.Count &&
                collection.All(te => compareFunction(te, timeEntriesMap[te.Id]));
        }

        private class TimeEntriesAtUpdatedComparer
        {
            private Dictionary<long, IThreadSafeTimeEntry> timeEntriesMap;

            public TimeEntriesAtUpdatedComparer(IEnumerable<IThreadSafeTimeEntry> timeEntries)
            {
                timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            }

            public bool CompareTo(IEnumerable<IThreadSafeTimeEntry> collection)
                => collection.Count() == timeEntriesMap.Count
                && collection.All(te => te.At > timeEntriesMap[te.Id].At);
        }

        private class TimeEntriesOtherPropertiesComparer
        {
            private Dictionary<long, IThreadSafeTimeEntry> timeEntriesMap;

            public TimeEntriesOtherPropertiesComparer(IEnumerable<IThreadSafeTimeEntry> timeEntries)
            {
                timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            }

            public bool CompareTo(IEnumerable<IThreadSafeTimeEntry> collection)
                => collection.Count() == timeEntriesMap.Count
                && collection.All(te => ensurePropertiesDidNotChange(te, timeEntriesMap[te.Id]));

            private bool ensurePropertiesDidNotChange(IThreadSafeTimeEntry timeEntry, IThreadSafeTimeEntry otherTimeEntry)
              => timeEntry.Id == otherTimeEntry.Id
              && timeEntry.UserId == otherTimeEntry.UserId
              && timeEntry.IsDeleted == otherTimeEntry.IsDeleted
              && timeEntry.ServerDeletedAt == otherTimeEntry.ServerDeletedAt;
        }

        private UpdateMultipleTimeEntriesInteractor createInteractor(EditTimeEntryDto[] dtos)
            => new UpdateMultipleTimeEntriesInteractor(TimeService, DataSource, InteractorFactory, SyncManager, dtos);

        [Fact, LogIfTooSlow]
        public async Task UpdatesTheDescriptionProperty()
        {
            var dtos = prepareTimeEntries();
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesCollectionComparer(
                dtos, (a, b) => a.Description == b.Description);

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task UpdatesTheSyncStatusProperty()
        {
            var dtos = prepareTimeEntries();
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesCollectionComparer(
                dtos, (a, _) => a.SyncStatus == SyncStatus.SyncNeeded);

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task UpdatesTheAtProperty()
        {
            var dtos = prepareTimeEntries();
            TimeService.CurrentDateTime.Returns(DateTimeOffset.UtcNow);
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesAtUpdatedComparer(timeEntries);

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task UpdatesTheProjectId()
        {
            var dtos = prepareTimeEntries();
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesCollectionComparer(
                dtos, (a, b) => a.ProjectId == b.ProjectId);

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task UpdatesTheBillbaleFlag()
        {
            var dtos = prepareTimeEntries();
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesCollectionComparer(
                dtos, (a, b) => a.Billable == b.Billable);

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task UpdatesTheTagIds()
        {
            var dtos = prepareTimeEntries();
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesCollectionComparer(
                dtos, (a, b) => a.TagIds.SequenceEqual(b.TagIds));

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task UpdatesTheWorkspaceId()
        {
            var dtos = prepareTimeEntries();
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesCollectionComparer(
                dtos, (a, b) => a.WorkspaceId == b.WorkspaceId);

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task LeavesAllOtherPropertiesUnchanged()
        {
            var dtos = prepareTimeEntries();
            var timeEntriesMap = timeEntries.ToDictionary(te => te.Id);
            var comparer = new TimeEntriesOtherPropertiesComparer(timeEntries);

            await createInteractor(dtos).Execute();

            await timeEntriesSource.Received().BatchUpdate(
                Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(tes => comparer.CompareTo(tes)));
        }

        [Fact, LogIfTooSlow]
        public async Task TriggersPushSync()
        {
            var dtos = prepareTimeEntries();

            await createInteractor(dtos).Execute();

            SyncManager.Received().InitiatePushSync();
        }
    }
}
