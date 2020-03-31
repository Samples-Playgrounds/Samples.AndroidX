namespace Toggl.Storage
{
    public sealed class UpdateResult<T> : IConflictResolutionResult<T>
    {
        public long OriginalId { get; }

        public T Entity { get; }

        public UpdateResult(long originalId, T entity)
        {
            OriginalId = originalId;
            Entity = entity;
        }
    }
}
