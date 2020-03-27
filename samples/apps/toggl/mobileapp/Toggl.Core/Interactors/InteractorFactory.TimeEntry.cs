using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DTOs;
using Toggl.Core.Interactors.Generic;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Storage.Models;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<Task<IThreadSafeTimeEntry>> CreateTimeEntry(ITimeEntryPrototype prototype, TimeEntryStartOrigin origin)
            => new CreateTimeEntryInteractor(
                idProvider,
                timeService,
                dataSource,
                analyticsService,
                prototype,
                syncManager,
                prototype.StartTime,
                prototype.Duration,
                origin);

        public IInteractor<Task<IThreadSafeTimeEntry>> ContinueTimeEntry(long timeEntryId, ContinueTimeEntryMode continueMode)
            => new ContinueTimeEntryInteractor(this, timeService, (TimeEntryStartOrigin)continueMode, timeEntryId);

        public IInteractor<Task<IThreadSafeTimeEntry>> StartSuggestion(Suggestion suggestion)
            => new CreateTimeEntryInteractor(
                idProvider,
                timeService,
                dataSource,
                analyticsService,
                suggestion,
                syncManager,
                timeService.CurrentDateTime,
                null,
                TimeEntryStartOrigin.Suggestion);

        public IInteractor<Task<IThreadSafeTimeEntry>> ContinueMostRecentTimeEntry()
            => new ContinueMostRecentTimeEntryInteractor(
                idProvider,
                timeService,
                dataSource,
                analyticsService,
                syncManager);

        public IInteractor<Task> DeleteTimeEntry(long id)
            => new DeleteTimeEntryInteractor(timeService, dataSource.TimeEntries, this, id);

        public IInteractor<Task> DeleteMultipleTimeEntries(long[] ids)
             => new DeleteMultipleTimeEntriesInteractor(dataSource.TimeEntries, this, ids);

        public IInteractor<IObservable<Unit>> SoftDeleteMultipleTimeEntries(long[] ids)
            => new SoftDeleteMultipleTimeEntriesInteractor(dataSource.TimeEntries, timeService, syncManager, this, ids);

        public IInteractor<IObservable<IThreadSafeTimeEntry>> GetTimeEntryById(long id)
            => new GetByIdInteractor<IThreadSafeTimeEntry, IDatabaseTimeEntry>(dataSource.TimeEntries, analyticsService, id)
                .TrackException<Exception, IThreadSafeTimeEntry>("GetTimeEntryById");

        public IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> GetMultipleTimeEntriesById(long[] ids)
            => new GetMultipleByIdInteractor<IThreadSafeTimeEntry, IDatabaseTimeEntry>(dataSource.TimeEntries, ids);

        public IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> GetAllTimeEntriesVisibleToTheUser()
            => new GetAllTimeEntriesVisibleToTheUserInteractor(dataSource.TimeEntries);

        public IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> ObserveAllTimeEntriesVisibleToTheUser()
            => new ObserveAllTimeEntriesVisibleToTheUserInteractor(dataSource.TimeEntries, dataSource.Workspaces);

        public IInteractor<Task<IThreadSafeTimeEntry>> UpdateTimeEntry(EditTimeEntryDto dto)
            => new UpdateTimeEntryInteractor(timeService, dataSource, this, syncManager, dto);

        public IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> UpdateMultipleTimeEntries(EditTimeEntryDto[] dtos)
            => new UpdateMultipleTimeEntriesInteractor(timeService, dataSource, this, syncManager, dtos);

        public IInteractor<Task<IThreadSafeTimeEntry>> StopTimeEntry(DateTimeOffset currentDateTime, TimeEntryStopOrigin origin)
            => new StopTimeEntryInteractor(timeService, dataSource.TimeEntries, currentDateTime, analyticsService, origin);

        public IInteractor<IObservable<Unit>> ObserveTimeEntriesChanges()
            => new ObserveTimeEntriesChangesInteractor(dataSource);

        public IInteractor<IObservable<TimeSpan>> ObserveTimeTrackedToday()
            => new ObserveTimeTrackedTodayInteractor(timeService, dataSource.TimeEntries);
    }
}
