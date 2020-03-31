using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class UnauthorizedException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.Unauthorized;

        private const string defaultMessage = "User is not authorized to make this request and must enter login again.";

        private readonly IEnumerable<HttpHeader> requestHeaders;

        public string ApiToken
            => requestHeaders
                .Where(header => header.Type == HttpHeader.HeaderType.Auth)
                .Select(tryDecodeApiToken)
                .FirstOrDefault(token => token != null);

        internal UnauthorizedException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal UnauthorizedException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
            requestHeaders = request.Headers;
        }

        private static string tryDecodeApiToken(HttpHeader authHeader)
        {
            byte[] decodedBytes;
            try
            {
                decodedBytes = Convert.FromBase64String(authHeader.Value);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }

            var authString = Encoding.UTF8.GetString(decodedBytes, 0, decodedBytes.Length);

            var parts = authString.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || parts[1] != "api_token")
                return null;

            return parts[0];
        }
    }
}
