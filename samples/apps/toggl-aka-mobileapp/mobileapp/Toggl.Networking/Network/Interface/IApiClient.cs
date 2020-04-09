using System;
using System.Threading.Tasks;

namespace Toggl.Networking.Network
{
    internal interface IApiClient : IDisposable
    {
        Task<IResponse> Send(IRequest request);
    }
}
