using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectableWorkspaceViewModel : IDiffableByIdentifier<SelectableWorkspaceViewModel>
    {
        public long WorkspaceId { get; }

        public string WorkspaceName { get; }

        public bool Selected { get; }

        public SelectableWorkspaceViewModel(IThreadSafeWorkspace workspace, bool selected)
        {
            Ensure.Argument.IsNotNull(workspace, nameof(workspace));

            Selected = selected;
            WorkspaceId = workspace.Id;
            WorkspaceName = workspace.Name;
        }

        public bool Equals(SelectableWorkspaceViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return WorkspaceId == other.WorkspaceId && string.Equals(WorkspaceName, other.WorkspaceName) && Selected == other.Selected;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SelectableWorkspaceViewModel other && Equals(other);
        }

        public override int GetHashCode()
            => HashCode.Combine(
                WorkspaceId,
                WorkspaceName ?? string.Empty,
                Selected
            );

        public long Identifier => WorkspaceId;
    }
}
