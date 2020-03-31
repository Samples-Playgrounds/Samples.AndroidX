using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IPreferencesApi
        : IUpdatingApiClient<IPreferences>,
          IPullingSingleApiClient<IPreferences>
    {
    }
}
