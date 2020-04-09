namespace Toggl.Storage
{
    public sealed class DeleteResult<T> : IConflictResolutionResult<T>
    {
        public long Id { get; }

        public DeleteResult(long id)
        {
            Id = id;
        }
    }
}
