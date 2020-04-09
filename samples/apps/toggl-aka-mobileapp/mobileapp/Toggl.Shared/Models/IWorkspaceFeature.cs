namespace Toggl.Shared.Models
{
    public interface IWorkspaceFeature
    {
        WorkspaceFeatureId FeatureId { get; }

        bool Enabled { get; }
    }
}
