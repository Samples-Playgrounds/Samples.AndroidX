namespace Toggl.Core.UI.Parameters
{
    public sealed class SelectClientParameters
    {
        public long WorkspaceId { get; set; }

        public long SelectedClientId { get; set; }

        public static SelectClientParameters WithIds(long workspaceId, long? selectedClientId)
            => new SelectClientParameters
            {
                WorkspaceId = workspaceId,
                SelectedClientId = selectedClientId ?? 0
            };
    }
}
