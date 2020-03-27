using System;

namespace Toggl.Networking.Serialization
{
    public class SerializationException : Exception
    {
        private const string defaultMessage = "Something went wrong serializing '{0}'.";

        public SerializationException(Type modelType, Exception innerException = null)
            : base(string.Format(defaultMessage, modelType.Name), innerException)
        {
        }
    }
}