using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Networking.Models;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class ITagExtensions
    {
        public static ITag With(
            this ITag tag,
            New<long> workspaceId = default(New<long>))
            => new Tag
            {
                Id = tag.Id,
                ServerDeletedAt = tag.ServerDeletedAt,
                At = tag.At,
                Name = tag.Name,
                WorkspaceId = workspaceId.ValueOr(tag.WorkspaceId)
            };

        public static IThreadSafeTag ToSyncable(
            this ITag tag)
            => new MockTag
            {
                Id = tag.Id,
                At = tag.At,
                ServerDeletedAt = tag.ServerDeletedAt,
                Name = tag.Name,
                WorkspaceId = tag.WorkspaceId,
                IsDeleted = false,
                LastSyncErrorMessage = null
            };

        public static IEnumerable<IThreadSafeTag> ToSyncable(this IEnumerable<ITag> tags)
            => tags.Select(tag => tag.ToSyncable());
    }
}
