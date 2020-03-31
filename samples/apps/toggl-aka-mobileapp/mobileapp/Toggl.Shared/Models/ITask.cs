namespace Toggl.Shared.Models
{
    public interface ITask : IIdentifiable, ILastChangedDatable
    {
        string Name { get; }

        long ProjectId { get; }

        long WorkspaceId { get; }

        long? UserId { get; }

        long EstimatedSeconds { get; }

        bool Active { get; }

        long TrackedSeconds { get; }
    }
}
