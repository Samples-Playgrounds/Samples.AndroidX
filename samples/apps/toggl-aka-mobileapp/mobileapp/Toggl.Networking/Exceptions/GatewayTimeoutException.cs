using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class GatewayTimeoutException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.GatewayTimeout;

        private const string defaultMessage = "Bad gateway.";

        internal GatewayTimeoutException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal GatewayTimeoutException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}