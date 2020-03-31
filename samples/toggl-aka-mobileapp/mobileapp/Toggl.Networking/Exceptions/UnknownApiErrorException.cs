using System.Net;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class UnknownApiErrorException : ApiException
    {
        public HttpStatusCode HttpCode { get; }

        private const string defaultMessage = "The server responded with an unexpected HTTP status code.";

        internal UnknownApiErrorException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal UnknownApiErrorException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
            this.HttpCode = response.StatusCode;
        }
    }
}
