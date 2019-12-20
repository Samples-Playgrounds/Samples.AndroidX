using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class NotImplementedException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.NotImplemented;

        private const string defaultMessage = "This feature is not implemented.";

        internal NotImplementedException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal NotImplementedException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
