using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.DTOs;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;

namespace Toggl.Core.Interactors
{
    internal class UpdateMultipleTimeEntriesInteractor : IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>>
    {
        private readonly EditTimeEntryDto[] timeEntriesDtos;
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly ISyncManager syncManager;

        public UpdateMultipleTimeEntriesInteractor(
            ITimeService timeService,
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            ISyncManager syncManager,
            EditTimeEntryDto[] timeEntriesDtos)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(timeEntriesDtos, nameof(timeEntriesDtos));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));

            this.timeEntriesDtos = timeEntriesDtos;
            this.timeService = timeService;
            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;
            this.syncManager = syncManager;
        }

        public IObservable<IEnumerable<IThreadSafeTimeEntry>> Execute()
        {
            var dtosMap = timeEntriesDtos.ToDictionary(dto => dto.Id);

            var ids = dtosMap.Keys.ToArray();

            return interactorFactory.GetMultipleTimeEntriesById(ids)
                .Execute()
                .Select(timeEntries => timeEntries.Select(timeEntry => createUpdatedTimeEntry(timeEntry, dtosMap[timeEntry.Id])))
                .SelectMany(dataSource.TimeEntries.BatchUpdate)
                .UnwrapUpdatedThreadSafeEntities()
                .Do(syncManager.InitiatePushSync);
        }

        private TimeEntry createUpdatedTimeEntry(IThreadSafeTimeEntry timeEntry, EditTimeEntryDto dto)
            => TimeEntry.Builder.Create(dto.Id)
                .SetDescription(dto.Description)
                .SetDuration(dto.StopTime.HasValue ? (long?)(dto.StopTime.Value - dto.StartTime).TotalSeconds : null)
                .SetTagIds(dto.TagIds)
                .SetStart(dto.StartTime)
                .SetTaskId(dto.TaskId)
                .SetBillable(dto.Billable)
                .SetProjectId(dto.ProjectId)
                .SetWorkspaceId(dto.WorkspaceId)
                .SetUserId(timeEntry.UserId)
                .SetIsDeleted(timeEntry.IsDeleted)
                .SetServerDeletedAt(timeEntry.ServerDeletedAt)
                .SetAt(timeService.CurrentDateTime)
                .SetSyncStatus(SyncStatus.SyncNeeded)
                .Build();
    }
}
