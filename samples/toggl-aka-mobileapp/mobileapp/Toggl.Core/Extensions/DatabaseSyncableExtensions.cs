using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Extensions
{
    public static class DatabaseSyncableExtensions
    {
        public static bool IsLocalOnly(this IDatabaseSyncable syncable)
            => syncable is IIdentifiable identifiable && identifiable.Id < 0;
    }
}
