using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class ForbiddenException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.Forbidden;

        private const string defaultMessage = "User cannot perform this request.";

        internal ForbiddenException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal ForbiddenException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
