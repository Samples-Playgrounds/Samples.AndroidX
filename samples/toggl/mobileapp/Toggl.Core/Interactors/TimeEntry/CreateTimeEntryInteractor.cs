using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Interactors
{
    internal sealed class CreateTimeEntryInteractor : IInteractor<Task<IThreadSafeTimeEntry>>
    {
        private readonly TimeSpan? duration;
        private readonly IIdProvider idProvider;
        private readonly DateTimeOffset startTime;
        private readonly ITimeService timeService;
        private readonly TimeEntryStartOrigin origin;
        private readonly ITogglDataSource dataSource;
        private readonly ITimeEntryPrototype prototype;
        private readonly IAnalyticsService analyticsService;
        private readonly ISyncManager syncManager;

        public CreateTimeEntryInteractor(
            IIdProvider idProvider,
            ITimeService timeService,
            ITogglDataSource dataSource,
            IAnalyticsService analyticsService,
            ITimeEntryPrototype prototype,
            ISyncManager syncManager,
            DateTimeOffset startTime,
            TimeSpan? duration,
            TimeEntryStartOrigin origin)
        {
            Ensure.Argument.IsNotNull(origin, nameof(origin));
            Ensure.Argument.IsNotNull(prototype, nameof(prototype));
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));

            this.origin = origin;
            this.duration = duration;
            this.prototype = prototype;
            this.startTime = startTime;
            this.idProvider = idProvider;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.analyticsService = analyticsService;
            this.syncManager = syncManager;
        }

        public Task<IThreadSafeTimeEntry> Execute()
            => Task.Run(async () =>
            {
                var currentUser = await dataSource.User.Get();
                var timeEntryPrototype = userFromPrototype(currentUser);    
                var createdTimeEntry = await dataSource.TimeEntries.Create(timeEntryPrototype);
                notifyOfNewTimeEntryIfPossible(createdTimeEntry);
                syncManager.InitiatePushSync();
                var startTimeEntryEvent = StartTimeEntryEvent.With(origin).Invoke(createdTimeEntry);
                analyticsService.Track(startTimeEntryEvent);
                return createdTimeEntry;
            });

        private TimeEntry userFromPrototype(IThreadSafeUser user)
            => idProvider.GetNextIdentifier()
                .Apply(TimeEntry.Builder.Create)
                .SetUserId(user.Id)
                .SetTagIds(prototype.TagIds)
                .SetTaskId(prototype.TaskId)
                .SetStart(startTime)
                .SetDuration(duration)
                .SetBillable(prototype.IsBillable)
                .SetProjectId(prototype.ProjectId)
                .SetDescription(prototype.Description)
                .SetWorkspaceId(prototype.WorkspaceId)
                .SetAt(timeService.CurrentDateTime)
                .SetSyncStatus(SyncStatus.SyncNeeded)
                .Build();

        private void notifyOfNewTimeEntryIfPossible(IThreadSafeTimeEntry timeEntry)
        {
            if (dataSource.TimeEntries is TimeEntriesDataSource timeEntriesDataSource)
                timeEntriesDataSource.OnTimeEntryStarted(timeEntry, origin);
        }
    }
}
