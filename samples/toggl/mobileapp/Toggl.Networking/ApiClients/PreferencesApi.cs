using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    internal sealed class PreferencesApi : BaseApi, IPreferencesApi
    {
        private readonly PreferencesEndpoints endPoints;
        private readonly IJsonSerializer serializer;

        public PreferencesApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Preferences;
            this.serializer = serializer;
        }

        public async Task<IPreferences> Get()
            => await SendRequest<Preferences>(endPoints.Get, AuthHeader)
                .ConfigureAwait(false);

        public async Task<IPreferences> Update(IPreferences preferences)
        {
            var body = serializer.Serialize(preferences as Preferences ?? new Preferences(preferences), SerializationReason.Post);
            await SendRequest(endPoints.Post, new[] { AuthHeader }, body).ConfigureAwait(false);
            return preferences;
        }
    }
}
