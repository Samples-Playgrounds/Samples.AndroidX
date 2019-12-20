using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Toggl.Core.Autocomplete.Span;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Autocomplete
{
    [DebuggerDisplay("Description = {Description}")]
    public sealed class TextFieldInfo
    {
        public static TextFieldInfo Empty(long workspaceId)
            => new TextFieldInfo(workspaceId, ImmutableList<ISpan>.Empty);

        public static TextFieldInfo WithDescription(long workspaceId, string description)
        => new TextFieldInfo(workspaceId, ImmutableList.Create<ISpan>(new TextSpan(description)));

        private ProjectSpan projectSpan;

        public long WorkspaceId { get; }

        public IImmutableList<ISpan> Spans { get; }

        public long? ProjectId
            => HasProject ? projectSpan.ProjectId : (long?)null;

        private string description;
        public string Description
            => description ?? (description = calculateDescription());

        private bool? hasProject;
        public bool HasProject
        {
            get
            {
                if (hasProject.HasValue)
                    return hasProject.Value;

                projectSpan = Spans.OfType<ProjectSpan>().SingleOrDefault();
                hasProject = projectSpan != null;
                return hasProject.Value;
            }
        }

        private TextFieldInfo(long workspaceId, IImmutableList<ISpan> spans)
        {
            WorkspaceId = workspaceId;
            Spans = spans ?? ImmutableList<ISpan>.Empty;
        }

        public TextFieldInfo ReplaceSpans(IImmutableList<ISpan> spans)
            => new TextFieldInfo(WorkspaceId, spans);

        public TextFieldInfo AddQuerySymbol(string querySymbol)
        {
            var querySpan = GetSpanWithCurrentTextCursor();
            if (querySpan == null)
                return addSpanToRelevantPosition(WorkspaceId, new QueryTextSpan(querySymbol, 1));

            var indexBeforeCursor = querySpan.CursorPosition - 1;
            var needsSpace = indexBeforeCursor >= 0 && !char.IsWhiteSpace(querySpan.Text[indexBeforeCursor]);

            var padding = needsSpace ? " " : "";
            var textToAppend = $"{padding}{querySymbol}";
            return addSpanToRelevantPosition(WorkspaceId, new TextSpan(textToAppend));
        }

        public TextFieldInfo AddTag(long tagId, string tagName)
        {
            var spanToAdd = new TagSpan(tagId, tagName);
            var spansToReuse = Spans.Where(spanIsNotTagWithSameId(tagId));
            return addSpanToRelevantPosition(WorkspaceId, spanToAdd, spansToReuse.ToImmutableList());

            Func<ISpan, bool> spanIsNotTagWithSameId(long idOfTagTobeAdded) => span =>
            {
                if (span is TagSpan tagSpan)
                    return tagSpan.TagId != idOfTagTobeAdded;

                return true;
            };
        }

        public TextFieldInfo WithProject(long workspaceId, long projectId, string projectName, string projectColor, long? taskId, string taskName)
        {
            var spanToAdd = new ProjectSpan(projectId, projectName, projectColor, taskId, taskName);
            var spansToReuse = workspaceId != WorkspaceId
                ? Spans.Where(span => span is TextSpan)
                : Spans.Where(span => (span is ProjectSpan) == false);

            return addSpanToRelevantPosition(workspaceId, spanToAdd, spansToReuse.ToImmutableList());
        }

        public TextFieldInfo RemoveProjectQueryIfNeeded()
        {
            var querySpan = GetSpanWithCurrentTextCursor();
            return querySpan == null ? this : removeQuery(querySpan, QuerySymbols.Projects);
        }

        public TextFieldInfo RemoveTagQueryIfNeeded()
        {
            var querySpan = GetSpanWithCurrentTextCursor();
            return querySpan == null ? this : removeQuery(querySpan, QuerySymbols.Tags);
        }

        public QueryTextSpan GetSpanWithCurrentTextCursor()
            => Spans.OfType<QueryTextSpan>().SingleOrDefault();

        private TextFieldInfo removeQuery(QueryTextSpan currentQuerySpan, char querySymbol)
        {
            var newQuerySpan = currentQuerySpan.WithoutQueryForSymbol(querySymbol);

            var spansToReuse = Spans.Replace(currentQuerySpan, newQuerySpan);
            return ReplaceSpans(spansToReuse);
        }

        private TextFieldInfo addSpanToRelevantPosition(long workspaceId, ISpan spanToAdd)
            => addSpanToRelevantPosition(workspaceId, spanToAdd, Spans);

        private TextFieldInfo addSpanToRelevantPosition(long workspaceId, ISpan spanToAdd, IImmutableList<ISpan> spansToReuse)
        {
            var querySpan = GetSpanWithCurrentTextCursor();
            if (querySpan == null)
                return appendSpan(workspaceId, spanToAdd, spansToReuse);

            return addRelativeToQuerySpan(querySpan, workspaceId, spanToAdd, spansToReuse);
        }

        private TextFieldInfo appendSpan(long workspaceId, ISpan spanToAppend, IImmutableList<ISpan> spansToReuse)
        {
            var spanBuilder = ImmutableList.CreateBuilder<ISpan>();
            spanBuilder.AddRange(spansToReuse);

            if (spanToAppend is TextSpan textSpan)
            {
                spanBuilder.Add(new QueryTextSpan(textSpan.Text, textSpan.Text.Length));
            }
            else
            {
                spanBuilder.Add(spanToAppend);
                spanBuilder.Add(new QueryTextSpan());
            }

            return new TextFieldInfo(workspaceId, spanBuilder.ToImmutable());
        }

        private TextFieldInfo addRelativeToQuerySpan(QueryTextSpan querySpan, long workspaceId, ISpan spanToAdd, IImmutableList<ISpan> spansToReuse)
        {
            var spanBuilder = ImmutableList.CreateBuilder<ISpan>();
            var indexOfQueryingSpan = spansToReuse.IndexOf(querySpan);
            var spansBeforeReplacement = spansToReuse.Take(indexOfQueryingSpan);
            var spansAfterReplacement = spansToReuse.Skip(indexOfQueryingSpan + 1);

            spanBuilder.AddRange(spansBeforeReplacement);

            if (spanToAdd is TextSpan textSpan)
            {
                var indexToSplit = querySpan.CursorPosition;
                var textBeforeInsertedSpan = querySpan.Text.Substring(0, indexToSplit);
                var textAfterInsertedSpan = querySpan.Text.Substring(indexToSplit, querySpan.Text.Length - indexToSplit);

                var newQuerySpanText = $"{textBeforeInsertedSpan}{textSpan.Text}{textAfterInsertedSpan}";
                var newTextSpan = new QueryTextSpan(newQuerySpanText, newQuerySpanText.Length);
                spanBuilder.Add(newTextSpan);
            }
            else
            {
                var indexOfProjectQuerySymbolInSpan = querySpan.Text.IndexOfAny(ValidQuerySymbols());
                var substringLength = indexOfProjectQuerySymbolInSpan >= 0 ? indexOfProjectQuerySymbolInSpan : querySpan.Text.Length;
                var newQuerySpanText = querySpan.Text.Substring(0, substringLength);

                var newTextSpan = new TextSpan(newQuerySpanText);
                spanBuilder.Add(newTextSpan);
                spanBuilder.Add(spanToAdd);
                spanBuilder.Add(new QueryTextSpan());
            }

            spanBuilder.AddRange(spansAfterReplacement);

            return new TextFieldInfo(workspaceId, spanBuilder.ToImmutable());
        }

        internal char[] ValidQuerySymbols()
            => HasProject ? QuerySymbols.ProjectSelected : QuerySymbols.All;

        private string calculateDescription()
        {
            ISpan previous = null;
            return Spans
                .Aggregate(
                    new StringBuilder(),
                    (builder, span) =>
                    {
                        builder.Append(chooseSeparator(previous, span));
                        builder.Append(toString(span));
                        previous = span;
                        return builder;
                    })
                .ToString()
                .Trim();

            string toString(ISpan span)
                => span is TextSpan text ? text.Text.Trim() : string.Empty;

            string chooseSeparator(ISpan first, ISpan second)
                => first is TextSpan && second is TextSpan ? "" : " ";
        }
    }
}
