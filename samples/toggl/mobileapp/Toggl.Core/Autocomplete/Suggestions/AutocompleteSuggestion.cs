using System;

namespace Toggl.Core.Autocomplete.Suggestions
{
    public abstract class AutocompleteSuggestion : IEquatable<AutocompleteSuggestion>
    {
        public string WorkspaceName { get; protected set; } = "";

        public long WorkspaceId { get; protected set; }

        public abstract override int GetHashCode();

        public bool Equals(AutocompleteSuggestion other)
            => GetHashCode() == other.GetHashCode();
    }
}
