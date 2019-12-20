using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public abstract class ServerErrorException : ApiException
    {
        internal ServerErrorException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
