using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Networking.Models;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class ITaskExtensions
    {
        public static ITask With(
            this ITask task,
            New<long> workspaceId = default(New<long>),
            New<long> projectId = default(New<long>),
            New<long?> userId = default(New<long?>))
            => new Task
            {
                Id = task.Id,
                At = task.At,
                Name = task.Name,
                ProjectId = projectId.ValueOr(task.ProjectId),
                WorkspaceId = workspaceId.ValueOr(task.WorkspaceId),
                UserId = userId.ValueOr(task.UserId),
                EstimatedSeconds = task.EstimatedSeconds,
                Active = task.Active,
                TrackedSeconds = task.TrackedSeconds
            };

        public static IThreadSafeTask ToSyncable(
            this ITask task)
            => new MockTask
            {
                Id = task.Id,
                At = task.At,
                Name = task.Name,
                WorkspaceId = task.WorkspaceId,
                ProjectId = task.ProjectId,
                UserId = task.UserId,
                EstimatedSeconds = task.EstimatedSeconds,
                Active = task.Active,
                TrackedSeconds = task.TrackedSeconds,
                IsDeleted = false,
                LastSyncErrorMessage = null
            };

        public static IEnumerable<IThreadSafeTask> ToSyncable(this IEnumerable<ITask> tasks)
            => tasks.Select(task => task.ToSyncable());
    }
}
