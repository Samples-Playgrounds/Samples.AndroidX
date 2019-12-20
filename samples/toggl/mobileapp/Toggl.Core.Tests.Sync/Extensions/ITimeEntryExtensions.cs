using System.Collections.Generic;
using Toggl.Networking.Models;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class ITimeEntryExtensions
    {
        public static ITimeEntry With(
            this ITimeEntry timeEntry,
            New<long> workspaceId = default(New<long>),
            New<IEnumerable<long>> tagIds = default(New<IEnumerable<long>>),
            New<long?> taskId = default(New<long?>),
            New<long?> projectId = default(New<long?>))
            => new TimeEntry
            {
                Id = timeEntry.Id,
                ServerDeletedAt = timeEntry.ServerDeletedAt,
                At = timeEntry.At,
                WorkspaceId = workspaceId.ValueOr(timeEntry.WorkspaceId),
                ProjectId = projectId.ValueOr(timeEntry.ProjectId),
                TaskId = taskId.ValueOr(timeEntry.TaskId),
                Billable = timeEntry.Billable,
                Start = timeEntry.Start,
                Duration = timeEntry.Duration,
                Description = timeEntry.Description,
                TagIds = tagIds.ValueOr(timeEntry.TagIds),
                UserId = timeEntry.UserId
            };
    }
}
