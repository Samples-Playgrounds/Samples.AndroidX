using Toggl.Networking.Network;

namespace Toggl.Networking.Exceptions
{
    public sealed class DeserializationException<T> : ApiException
    {
        private const string errorMessage = "An error occured while trying to deserialize type {0}. Check the Json property for more info.";

        public string Json { get; set; }

        internal DeserializationException(IRequest request, IResponse response, string json)
            : base(request, response, string.Format(errorMessage, typeof(T).Name))
        {
            Json = json;
        }
    }
}
