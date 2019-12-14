using System.Collections.Generic;
using System.Net;
using Toggl.Shared;

namespace Toggl.Networking.Network
{
    internal sealed class Response : IResponse
    {
        public string RawData { get; }
        public bool IsSuccess { get; }
        public string ContentType { get; }
        public HttpStatusCode StatusCode { get; }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; }

        public bool IsJson => ContentType == "application/json";

        public Response(string rawData, bool isSuccess, string contentType, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, HttpStatusCode statusCode)
        {
            Ensure.Argument.IsNotNull(rawData, nameof(rawData));
            Ensure.Argument.IsNotNull(contentType, nameof(contentType));
            Ensure.Argument.IsNotNull(headers, nameof(headers));

            RawData = rawData;
            IsSuccess = isSuccess;
            ContentType = contentType;
            StatusCode = statusCode;
            Headers = headers;
        }
    }
}