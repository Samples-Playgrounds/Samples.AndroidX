using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class BadRequestException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.BadRequest;

        private const string defaultMessage = "The data is not valid or acceptable.";

        internal BadRequestException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal BadRequestException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
