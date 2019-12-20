using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class NotFoundException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.NotFound;

        private const string defaultMessage = "The resource was not found.";

        internal NotFoundException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal NotFoundException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
