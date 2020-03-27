using System;
using Toggl.Core.UI.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectableColorViewModel : IDiffableByIdentifier<SelectableColorViewModel>
    {
        public Color Color { get; }

        public bool Selected { get; }

        public SelectableColorViewModel(Color color, bool selected)
        {
            Ensure.Argument.IsNotNull(color, nameof(color));

            Color = color;
            Selected = selected;
        }

        public bool Equals(SelectableColorViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Color, other.Color) && Selected == other.Selected;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SelectableColorViewModel other && Equals(other);
        }

        public override int GetHashCode() => HashCode.Combine(Color, Selected);

        public long Identifier => Color.GetHashCode();
    }
}
