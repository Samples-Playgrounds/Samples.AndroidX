using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class BadGatewayException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.BadGateway;

        private const string defaultMessage = "Bad gateway.";

        internal BadGatewayException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal BadGatewayException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
