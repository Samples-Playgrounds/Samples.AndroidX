using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabasePreferences : IPreferences, IDatabaseSyncable, IIdentifiable
    {
    }
}
