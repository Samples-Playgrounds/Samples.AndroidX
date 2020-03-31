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
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Core.Interactors
{
    public class ContinueMostRecentTimeEntryInteractor : IInteractor<Task<IThreadSafeTimeEntry>>
    {
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IAnalyticsService analyticsService;
        private readonly ISyncManager syncManager;

        public ContinueMostRecentTimeEntryInteractor(
            IIdProvider idProvider,
            ITimeService timeService,
            ITogglDataSource dataSource,
            IAnalyticsService analyticsService,
            ISyncManager syncManager)
        {
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));

            this.idProvider = idProvider;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.analyticsService = analyticsService;
            this.syncManager = syncManager;
        }

        public Task<IThreadSafeTimeEntry> Execute()
            => ThreadingTask.Run(async () =>
            {
                var timeEntries = await dataSource.TimeEntries.GetAll(te => !te.IsDeleted);
                var copy = timeEntries.MaxBy(te => te.Start).Apply(newTimeEntry);

                var newlyCreatedTimeEntry = await dataSource.TimeEntries.Create(copy);

                syncManager.InitiatePushSync();
                analyticsService.Track(StartTimeEntryEvent.With(TimeEntryStartOrigin.ContinueMostRecent, newlyCreatedTimeEntry));

                return newlyCreatedTimeEntry;
            });

        private IThreadSafeTimeEntry newTimeEntry(IThreadSafeTimeEntry timeEntry)
            => TimeEntry.Builder
                        .Create(idProvider.GetNextIdentifier())
                        .SetTagIds(timeEntry.TagIds)
                        .SetUserId(timeEntry.UserId)
                        .SetTaskId(timeEntry.TaskId)
                        .SetBillable(timeEntry.Billable)
                        .SetProjectId(timeEntry.ProjectId)
                        .SetAt(timeService.CurrentDateTime)
                        .SetSyncStatus(SyncStatus.SyncNeeded)
                        .SetDescription(timeEntry.Description)
                        .SetStart(timeService.CurrentDateTime)
                        .SetWorkspaceId(timeEntry.WorkspaceId)
                        .Build();
    }
}
