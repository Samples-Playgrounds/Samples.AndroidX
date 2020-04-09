using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public abstract class ClientErrorException : ApiException
    {
        internal ClientErrorException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
