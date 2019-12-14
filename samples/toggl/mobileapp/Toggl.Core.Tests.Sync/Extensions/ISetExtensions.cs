using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Toggl.Storage;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class ISetExtensions
    {
        public static void ContainsNoPlaceholders<T>(this ISet<T> entities)
            where T : IDatabaseSyncable
        {
            entities.Where(entity => entity.SyncStatus == SyncStatus.RefetchingNeeded)
                .Should()
                .HaveCount(0);
        }
    }
}
