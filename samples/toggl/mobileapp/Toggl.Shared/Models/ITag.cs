namespace Toggl.Shared.Models
{
    public interface ITag : IIdentifiable, IDeletable, ILastChangedDatable
    {
        long WorkspaceId { get; }

        string Name { get; }
    }
}
