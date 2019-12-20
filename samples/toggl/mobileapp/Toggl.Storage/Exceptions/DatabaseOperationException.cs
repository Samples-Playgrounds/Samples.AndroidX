using System;

namespace Toggl.Storage.Exceptions
{
    public sealed class DatabaseOperationException<T> : DatabaseException
    {
        private const string defaultErrorMessage = "Database operation failed";

        public DatabaseOperationException(Exception exception)
            : base($"{defaultErrorMessage} [{nameof(T)}]: {exception.Message}", exception)
        {
        }
    }
}
