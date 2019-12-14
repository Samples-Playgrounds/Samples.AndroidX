using System;
using Toggl.Core.UI.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public abstract class SelectableTagBaseViewModel : IDiffableByIdentifier<SelectableTagBaseViewModel>
    {
        public string Name { get; }
        public bool Selected { get; }

        public long WorkspaceId { get; }

        public long Identifier { get; protected set; }

        public SelectableTagBaseViewModel(string name, bool selected, long workspaceId)
        {
            Ensure.Argument.IsNotNullOrWhiteSpaceString(name, nameof(name));
            Name = name;
            Selected = selected;
            WorkspaceId = workspaceId;
        }

        public override string ToString()
            => Name;

        public override int GetHashCode()
            => HashCode.Combine(Name ?? string.Empty, WorkspaceId, Selected);

        public bool Equals(SelectableTagBaseViewModel other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Identifier == other.Identifier
                && Name == other.Name
                && WorkspaceId == other.WorkspaceId
                && Selected == other.Selected;
        }
    }

    [Preserve(AllMembers = true)]
    public sealed class SelectableTagViewModel : SelectableTagBaseViewModel
    {
        public long Id { get; }

        public SelectableTagViewModel(
            long id,
            string name,
            bool selected,
            long workspaceId
        )
            : base(name, selected, workspaceId)
        {
            Id = id;
            Identifier = Id;
        }
    }

    [Preserve(AllMembers = true)]
    public sealed class SelectableTagCreationViewModel : SelectableTagBaseViewModel
    {
        public SelectableTagCreationViewModel(string name, long workspaceId)
            : base(name, false, workspaceId)
        {
            /*
             * This identifier property has to be unique across all the instances
             * of the class SelectableTagBaseViewModel. As we are not using an ID of
             * 0, neither by our own ID provider, or by autoincrement in the database,
             * we are using the value of 0 to identify "+ Create Tag XYZ" element.
             */
            Identifier = 0;
        }
    }
}
