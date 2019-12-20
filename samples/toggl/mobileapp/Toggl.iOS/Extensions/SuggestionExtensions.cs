using Toggl.Core.Suggestions;
using SharedSuggestion = Toggl.iOS.Shared.Models.Suggestion;

namespace Toggl.iOS.Extensions
{
    public static class SuggestionExtensions
    {
        public static SharedSuggestion ToSharedSuggestion(this Suggestion suggestion)
        {
            return new SharedSuggestion(
                suggestion.WorkspaceId,
                suggestion.Description,
                suggestion.ProjectId,
                suggestion.ProjectName,
                suggestion.ProjectColor,
                suggestion.TaskId,
                suggestion.TaskName,
                suggestion.ClientName,
                suggestion.IsBillable,
                suggestion.TagIds);
        }
    }
}
