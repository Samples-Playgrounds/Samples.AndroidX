using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.DTOs;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Storage;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Core.Interactors
{
    internal class UpdateTimeEntryInteractor : IInteractor<Task<IThreadSafeTimeEntry>>
    {
        private readonly EditTimeEntryDto dto;
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly ISyncManager syncManager;

        public UpdateTimeEntryInteractor(
            ITimeService timeService,
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            ISyncManager syncManager,
            EditTimeEntryDto dto)
        {
            Ensure.Argument.IsNotNull(dto, nameof(dto));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));

            this.dto = dto;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.interactorFactory = interactorFactory;
            this.syncManager = syncManager;
        }

        public Task<IThreadSafeTimeEntry> Execute()
            => ThreadingTask.Run(async () =>
            {
                var originalTimeEntry = await interactorFactory.GetTimeEntryById(dto.Id).Execute(); 
                var timeEntryToUpdate = createUpdatedTimeEntry(originalTimeEntry);
                var updatedTimeEntry = await dataSource.TimeEntries.Update(timeEntryToUpdate);
                syncManager.InitiatePushSync();
                return updatedTimeEntry;
            });

        private TimeEntry createUpdatedTimeEntry(IThreadSafeTimeEntry timeEntry)
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
