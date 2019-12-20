using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Autocomplete.Suggestions
{
    public sealed class TagSuggestion : AutocompleteSuggestion
    {
        public static IEnumerable<TagSuggestion> FromTags(IEnumerable<IThreadSafeTag> tags)
            => tags.Select(t => new TagSuggestion(t));

        public long TagId { get; }

        public string Name { get; }

        public TagSuggestion(IThreadSafeTag tag)
        {
            TagId = tag.Id;
            Name = tag.Name;
            WorkspaceName = tag.Workspace.Name;
            WorkspaceId = tag.WorkspaceId;
        }

        public override int GetHashCode()
            => HashCode.Combine(TagId, Name);
    }
}
