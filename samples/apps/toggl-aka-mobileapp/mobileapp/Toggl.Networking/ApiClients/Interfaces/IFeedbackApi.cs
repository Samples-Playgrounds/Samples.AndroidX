using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Shared;

namespace Toggl.Networking.ApiClients
{
    public interface IFeedbackApi
    {
        Task Send(Email email, string message, IDictionary<string, string> data);
    }
}
