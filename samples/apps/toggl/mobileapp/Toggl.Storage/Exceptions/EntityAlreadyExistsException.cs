namespace Toggl.Storage.Exceptions
{
    public sealed class EntityAlreadyExistsException : DatabaseException
    {
        private const string defaultErrorMessage = "Entity already exists in database";

        public EntityAlreadyExistsException()
            : this(defaultErrorMessage)
        {
        }

        public EntityAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
