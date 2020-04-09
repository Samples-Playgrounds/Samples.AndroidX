using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Shared;

namespace Toggl.Networking.ApiClients
{
    public interface IPushServicesApi
    {
        Task Subscribe(PushNotificationsToken token);
        Task Unsubscribe(PushNotificationsToken token);
        Task<List<PushNotificationsToken>> GetAll();
    }
}
