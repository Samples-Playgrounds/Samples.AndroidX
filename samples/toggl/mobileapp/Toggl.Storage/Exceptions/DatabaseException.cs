using System;

namespace Toggl.Storage.Exceptions
{
    public class DatabaseException : Exception
    {
        private const string defaultErrorMessage = "An error occured while accessing the database. Check inner exception for more details.";

        public DatabaseException(string message)
            : base(message)
        {
        }

        public DatabaseException(Exception ex)
            : base(defaultErrorMessage, ex)
        {
        }

        public DatabaseException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}