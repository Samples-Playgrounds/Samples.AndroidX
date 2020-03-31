namespace Toggl.Core.UI.Parameters
{
    public sealed class SelectProjectParameter
    {
        public long? ProjectId { get; }
        public long? TaskId { get; }
        public long WorkspaceId { get; }
        public bool CreationEnabled { get; }

        public SelectProjectParameter(long? projectId, long? taskId, long workspaceId, bool creationEnabled = true)
        {
            ProjectId = projectId;
            TaskId = taskId;
            WorkspaceId = workspaceId;
            CreationEnabled = creationEnabled;
        }
    }
}
