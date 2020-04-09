namespace Toggl.Core.DTOs
{
    public sealed class CreateProjectDTO
    {
        public string Name { get; set; }

        public string Color { get; set; }

        public bool IsPrivate { get; set; }

        public long? ClientId { get; set; }

        public long WorkspaceId { get; set; }

        public bool? Billable { get; set; }
    }
}
