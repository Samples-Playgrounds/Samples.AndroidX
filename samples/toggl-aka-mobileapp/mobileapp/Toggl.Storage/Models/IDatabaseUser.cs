using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseUser : IUser, IDatabaseSyncable { }
}
