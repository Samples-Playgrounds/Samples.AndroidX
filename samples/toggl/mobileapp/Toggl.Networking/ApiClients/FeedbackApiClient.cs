using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Networking.Serialization.Converters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Networking.ApiClients
{
    internal class FeedbackApiClient : BaseApi, IFeedbackApi
    {
        private readonly FeedbackEndpoints endPoints;

        private readonly IJsonSerializer serializer;

        public FeedbackApiClient(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Feedback;
            this.serializer = serializer;
        }

        public Task Send(Email email, string message, IDictionary<string, string> data)
        {
            Ensure.Argument.IsValidEmail(email, nameof(email));
            Ensure.Argument.IsNotNullOrWhiteSpaceString(message, nameof(message));

            var feedback = new Feedback
            {
                Email = email,
                Message = message,
                Data = data?.Select(CommonFunctions.Identity) ?? Enumerable.Empty<KeyValuePair<string, string>>()
            };

            var json = serializer.Serialize(feedback, SerializationReason.Post);

            return SendRequest(endPoints.Post, AuthHeader, json);
        }

        [Preserve(AllMembers = true)]
        private class Feedback
        {
            [JsonConverter(typeof(EmailConverter))]
            public Email Email { get; set; }

            public string Message { get; set; }

            public IEnumerable<KeyValuePair<string, string>> Data { get; set; }
        }
    }
}
