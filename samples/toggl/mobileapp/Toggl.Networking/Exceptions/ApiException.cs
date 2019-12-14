using System;
using System.Linq;
using Toggl.Networking.Extensions;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;

namespace Toggl.Networking.Exceptions
{
    [IsAnonymized]
    public class ApiException : Exception
    {
        private const string badJsonLocalisedError = "Encountered unexpected error.";

        private readonly string message;
        internal IRequest Request { get; }
        internal IResponse Response { get; }

        public string LocalizedApiErrorMessage { get; }
        public override string Message => ToString();

        internal ApiException(IRequest request, IResponse response, string defaultMessage)
        {
            message = defaultMessage;
            Request = request;
            Response = response;
            LocalizedApiErrorMessage = getLocalizedMessageFromResponse(response);
        }

        public override string ToString()
#if DEBUG
            => detailedMessage;
#else
            => anonymizedMessage;
#endif

        private string detailedMessage
            => $"{GetType().Name} ({message}) for request {Request.HttpMethod} {Request.Endpoint} "
                + $"with response {serialisedResponse}";

        private string anonymizedMessage
            => $"{GetType().Name} ({message}) for request {Request.HttpMethod} {Request.Endpoint.Anonymize()}: {LocalizedApiErrorMessage}";

        private string serialisedResponse => new JsonSerializer().Serialize(
            new
            {
                Status = $"{(int)Response.StatusCode} {Response.StatusCode}",
                Headers = Response.Headers.ToDictionary(h => h.Key, h => h.Value),
                Body = Response.RawData
            });


        private static string getLocalizedMessageFromResponse(IResponse response)
        {
            if (!response.IsJson)
                return response.RawData;

            try
            {
                var error = new JsonSerializer().Deserialize<ResponseError>(response.RawData);
                return error?.ErrorMessage?.DefaultMessage ?? badJsonLocalisedError;
            }
            catch (DeserializationException)
            {
                return badJsonLocalisedError;
            }
        }
    }
}
