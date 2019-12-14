namespace Toggl.Shared.Models
{
    public interface IClient : IIdentifiable, IDeletable, ILastChangedDatable
    {
        long WorkspaceId { get; }

        string Name { get; }
    }
}
