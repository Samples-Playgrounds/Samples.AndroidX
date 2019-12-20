using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.Helper;
using Toggl.Shared.Extensions;
using Toggl.Shared;
using static Toggl.Droid.Widgets.WidgetsConstants;

namespace Toggl.Droid.Widgets
{
    public sealed class WidgetSuggestionItem
    {
        private const string prefix = "Suggestion";

        public long WorkspaceId { get; private set; }
        public string Description { get; private set; }
        public long? ProjectId { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectColor { get; private set; }
        public string ClientName { get; private set; }

        public long? TaskId { get; private set; }
        public bool IsBillable { get; private set; }
        public long[] TagsIds { get; private set; }

        public bool HasProject => ProjectId.HasValue;

        private WidgetSuggestionItem() { }

        public static IEnumerable<WidgetSuggestionItem> SuggestionsFromSharedPreferences()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(SuggestionsList, FileCreationMode.Private);

            var suggestionsCount = sharedPreferences.GetInt(SuggestionsCount, 0);

            return Enumerable.Range(0, suggestionsCount)
                .Select(index => getItem(sharedPreferences, index));
        }

        private static WidgetSuggestionItem getItem(ISharedPreferences sharedPreferences, int index)
        {
            var projectId = (long?)sharedPreferences.GetLong($"{prefix}{nameof(ProjectId)}{index}", 0);
            projectId = projectId == 0 ? null : projectId;

            var taskId = (long?)sharedPreferences.GetLong($"{prefix}{nameof(TaskId)}{index}", 0);
            taskId = taskId == 0 ? null : taskId;

            var tagsIdsString = sharedPreferences.GetString($"{prefix}{nameof(TagsIds)}{index}", null);
            var tagsIds = tagsIdsString?.Split(',').Select(long.Parse).ToArray() ?? Array.Empty<long>();

            return new WidgetSuggestionItem
            {
                ProjectId = projectId,
                WorkspaceId = sharedPreferences.GetLong($"{prefix}{nameof(WorkspaceId)}{index}", 0),
                Description = sharedPreferences.GetString($"{prefix}{nameof(Description)}{index}", ""),
                ProjectName = sharedPreferences.GetString($"{prefix}{nameof(ProjectName)}{index}", ""),
                ProjectColor = sharedPreferences.GetString($"{prefix}{nameof(ProjectColor)}{index}", null) ?? Colors.Black.ToHexString(),
                ClientName = sharedPreferences.GetString($"{prefix}{nameof(ClientName)}{index}", ""),
                IsBillable = sharedPreferences.GetBoolean($"{prefix}{nameof(IsBillable)}{index}", false),
                TaskId = taskId,
                TagsIds = tagsIds
            };
        }

        internal static void SaveSuggestions(IImmutableList<Suggestion> suggestions)
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(SuggestionsList, FileCreationMode.Private);
            var prefsEditor = sharedPreferences.Edit();

            prefsEditor.PutInt(SuggestionsCount, suggestions.Count);

            for (int index = 0; index < suggestions.Count; index++)
            {
                var suggestion = suggestions[index];

                prefsEditor.PutLong($"{prefix}{nameof(ProjectId)}{index}", suggestion.ProjectId ?? 0);
                prefsEditor.PutLong($"{prefix}{nameof(WorkspaceId)}{index}", suggestion.WorkspaceId);
                prefsEditor.PutString($"{prefix}{nameof(Description)}{index}", suggestion.Description);
                prefsEditor.PutString($"{prefix}{nameof(ProjectName)}{index}", suggestion.ProjectName);
                prefsEditor.PutString($"{prefix}{nameof(ProjectColor)}{index}", suggestion.ProjectColor);
                prefsEditor.PutString($"{prefix}{nameof(ClientName)}{index}", suggestion.ClientName);
                prefsEditor.PutBoolean($"{prefix}{nameof(IsBillable)}{index}", suggestion.IsBillable);
                prefsEditor.PutLong($"{prefix}{nameof(TaskId)}{index}", suggestion.TaskId ?? 0);
                prefsEditor.PutString($"{prefix}{nameof(TagsIds)}{index}", string.Join(',', suggestion.TagIds).ToNullIfEmpty());
            }

            prefsEditor.Commit();
        }
    }
}
