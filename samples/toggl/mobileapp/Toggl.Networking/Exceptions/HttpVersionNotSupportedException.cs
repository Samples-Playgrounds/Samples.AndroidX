using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class HttpVersionNotSupportedException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.HttpVersionNotSupported;

        private const string defaultMessage = "HTTP version is not supported.";

        internal HttpVersionNotSupportedException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal HttpVersionNotSupportedException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}