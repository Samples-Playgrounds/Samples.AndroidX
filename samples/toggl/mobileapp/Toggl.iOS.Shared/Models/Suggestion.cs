using System;

namespace Toggl.iOS.Shared.Models
{
    public sealed class Suggestion
    {
        public long WorkspaceId { get; }

        public string Description { get; }

        public long? ProjectId { get; }

        public string ProjectName { get; }

        public string ProjectColor { get; }

        public long? TaskId { get; }

        public string TaskName { get; }

        public string ClientName { get; }

        public bool IsBillable { get; }

        public long[] TagIds { get; }

        public Suggestion(
            long workspaceId,
            string description,
            long? projectId = null,
            string projectName = "",
            string projectColor = "",
            long? taskId = null,
            string taskName = "",
            string clientName = "",
            bool isBillable = false,
            long[] tagIds = null)
        {
            WorkspaceId = workspaceId;
            Description = description;
            ProjectId = projectId;
            ProjectName = projectName;
            ProjectColor = projectColor;
            TaskId = taskId;
            TaskName = taskName;
            ClientName = clientName;
            IsBillable = isBillable;
            TagIds = tagIds;
        }
    }
}
