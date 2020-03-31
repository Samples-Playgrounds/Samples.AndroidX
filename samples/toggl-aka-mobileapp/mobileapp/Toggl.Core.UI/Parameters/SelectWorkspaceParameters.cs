namespace Toggl.Core.UI.Parameters
{
    public sealed class SelectWorkspaceParameters
    {
        public string Title { get; set; }
        public long CurrentWorkspaceId { get; set; }

        public SelectWorkspaceParameters(string title, long currentWorkspaceId)
        {
            Title = title;
            CurrentWorkspaceId = currentWorkspaceId;
        }
    }
}
