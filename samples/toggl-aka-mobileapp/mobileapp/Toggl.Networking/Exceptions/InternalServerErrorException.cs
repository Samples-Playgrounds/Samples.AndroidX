using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class InternalServerErrorException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.InternalServerError;

        private const string defaultMessage = "Internal server error.";

        internal InternalServerErrorException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal InternalServerErrorException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
