using Toggl.Networking;
using Toggl.Networking.Network;

namespace Toggl.Core.Login
{
    public interface IApiFactory
    {
        ITogglApi CreateApiWith(Credentials credentials);
    }
}
