namespace Toggl.Core.UI.Parameters
{
    public sealed class SelectTagsParameter
    {
        public long[] TagIds { get; }
        public long WorkspaceId { get; }
        public bool CreationEnabled { get; }

        public SelectTagsParameter(long[] tagIds, long workspaceId, bool creationEnabled = true)
        {
            TagIds = tagIds;
            WorkspaceId = workspaceId;
            CreationEnabled = creationEnabled;
        }
    }
}
