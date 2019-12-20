using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class TooManyRequestsException : ClientErrorException
    {
        // HTTP status code 429 "Too Many Redirects" is not included in the System.Net.HttpStatusCode enum.
        public const HttpStatusCode CorrespondingHttpCode = (HttpStatusCode)429;

        private const string defaultMessage = "The rate limiting does not work properly, fix it.";

        internal TooManyRequestsException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal TooManyRequestsException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
