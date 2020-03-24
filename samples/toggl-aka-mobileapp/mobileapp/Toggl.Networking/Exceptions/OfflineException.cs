using System;

namespace Toggl.Networking.Exceptions
{
    public sealed class OfflineException : Exception
    {
        private const string defaultMessage = "Offline mode was detected.";

        public OfflineException()
            : base(defaultMessage)
        {
        }

        public OfflineException(Exception innerException)
            : base(defaultMessage, innerException)
        {
        }
    }
}
