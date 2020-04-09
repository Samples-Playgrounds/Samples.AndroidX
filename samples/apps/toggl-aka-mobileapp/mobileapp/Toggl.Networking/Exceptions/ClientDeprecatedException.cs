using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class ClientDeprecatedException : ClientErrorException
    {
        // HTTP status code 418 "I Am A Teapot" is not included in the System.Net.HttpStatusCode enum
        public const HttpStatusCode CorrespondingHttpCode = (HttpStatusCode)418;

        private const string defaultMessage = "This version of client application is deprecated and must be updated to an up-to-date version.";

        internal ClientDeprecatedException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal ClientDeprecatedException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
