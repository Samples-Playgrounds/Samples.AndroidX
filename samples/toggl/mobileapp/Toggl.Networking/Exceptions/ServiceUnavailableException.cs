using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class ServiceUnavailableException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.ServiceUnavailable;

        private const string defaultMessage = "Service unavailable.";

        internal ServiceUnavailableException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal ServiceUnavailableException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
