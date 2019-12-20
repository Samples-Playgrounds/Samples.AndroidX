using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Toggl.Shared;

namespace Toggl.Networking.Network
{
    internal sealed class Request : IRequest
    {
        public Either<string, byte[]> Body { get; }

        public Uri Endpoint { get; }

        public HttpMethod HttpMethod { get; }

        public IEnumerable<HttpHeader> Headers { get; }

        public Request(string body, Uri endpoint, IEnumerable<HttpHeader> headers, HttpMethod httpMethod)
        {
            Ensure.Argument.IsNotNull(body, nameof(body));
            // ReSharper disable once PossibleMultipleEnumeration
            Ensure.Argument.IsNotNull(headers, nameof(headers));
            Ensure.Argument.IsNotNull(endpoint, nameof(endpoint));

            Body = Either<string, byte[]>.WithLeft(body);
            // ReSharper disable once PossibleMultipleEnumeration
            Headers = headers.ToList();
            Endpoint = endpoint;
            HttpMethod = httpMethod;
        }
    }
}
