using System.Collections.Generic;
using System.Collections.Immutable;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.Autocomplete.Suggestions;

namespace Toggl.Core.UI.Extensions
{
    public static class AutocompleteExtensions
    {
        public static TextFieldInfo FromQuerySymbolSuggestion(
            this TextFieldInfo textFieldInfo,
            QuerySymbolSuggestion querySymbolSuggestion)
        {
            var result = textFieldInfo.AddQuerySymbol(querySymbolSuggestion.Symbol);
            return result;
        }

        public static TextFieldInfo FromTimeEntrySuggestion(
            this TextFieldInfo textFieldInfo,
            TimeEntrySuggestion timeEntrySuggestion)
        {
            var builder = ImmutableList.CreateBuilder<ISpan>();
            builder.Add(new TextSpan(timeEntrySuggestion.Description));

            if (timeEntrySuggestion.HasProject)
            {
                var projectSpan = new ProjectSpan(
                    timeEntrySuggestion.ProjectId.Value,
                    timeEntrySuggestion.ProjectName,
                    timeEntrySuggestion.ProjectColor,
                    timeEntrySuggestion.TaskId,
                    timeEntrySuggestion.TaskName
                );

                builder.Add(projectSpan);
            }

            builder.Add(new QueryTextSpan());

            return TextFieldInfo
                .Empty(timeEntrySuggestion.WorkspaceId)
                .ReplaceSpans(builder.ToImmutable());
        }

        public static TextFieldInfo FromProjectSuggestion(
            this TextFieldInfo textFieldInfo,
            ProjectSuggestion projectSuggestion)
        {
            var result = textFieldInfo.WithProject(
                projectSuggestion.WorkspaceId,
                projectSuggestion.ProjectId,
                projectSuggestion.ProjectName,
                projectSuggestion.ProjectColor,
                null,
                null
            );

            return result;
        }

        public static TextFieldInfo FromTaskSuggestion(
            this TextFieldInfo textFieldInfo,
            TaskSuggestion taskSuggestion)
        {
            var result = textFieldInfo.WithProject(
                taskSuggestion.WorkspaceId,
                taskSuggestion.ProjectId,
                taskSuggestion.ProjectName,
                taskSuggestion.ProjectColor,
                taskSuggestion.TaskId,
                taskSuggestion.Name
            );

            return result;
        }

        public static TextFieldInfo FromTagSuggestion(
            this TextFieldInfo textFieldInfo,
            TagSuggestion tagSuggestion)
        {
            var result = textFieldInfo.AddTag(tagSuggestion.TagId, tagSuggestion.Name);
            return result;
        }

        public static IEnumerable<ISpan> CollapseTextSpans(this IEnumerable<ISpan> spans)
        {
            var collapsed = new List<ISpan>();

            TextSpan previousTextSpan = null;

            foreach (var span in spans)
            {
                if (span is TextSpan == false && previousTextSpan != null)
                {
                    collapsed.Add(previousTextSpan);
                    previousTextSpan = null;
                }

                if (span is TextSpan textSpan)
                {
                    previousTextSpan = previousTextSpan == null
                        ? textSpan
                        : mergeTextSpans(previousTextSpan, textSpan);
                }
                else
                {
                    collapsed.Add(span);
                }
            }

            if (previousTextSpan != null)
            {
                collapsed.Add(previousTextSpan);
            }

            return collapsed;
        }

        private static TextSpan mergeTextSpans(TextSpan first, TextSpan second)
        {
            if (first is QueryTextSpan firstQuerySpan)
            {
                return new QueryTextSpan(first.Text + second.Text, firstQuerySpan.CursorPosition);
            }

            if (second is QueryTextSpan secondQuerySpan)
            {
                return new QueryTextSpan(first.Text + second.Text, first.Text.Length + secondQuerySpan.CursorPosition);
            }

            return new TextSpan(first.Text + second.Text);
        }
    }
}
