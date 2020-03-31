using System;
using System.Linq;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Autocomplete
{
    public struct QueryInfo
    {
        public string Text { get; }

        public AutocompleteSuggestionType SuggestionType { get; }

        private const int minimumQueryLength = 2;

        private static QueryInfo emptyQueryInfo
            => new QueryInfo(string.Empty, AutocompleteSuggestionType.None);

        public QueryInfo(string text, AutocompleteSuggestionType suggestionType)
        {
            Text = text;
            SuggestionType = suggestionType;
        }

        public static QueryInfo ParseFieldInfo(TextFieldInfo info)
        {
            var querySpan = info.GetSpanWithCurrentTextCursor();

            if (string.IsNullOrEmpty(querySpan?.Text))
                return emptyQueryInfo;

            return searchByQuerySymbols(querySpan, info) ?? getDefaultQueryInfo(querySpan.Text);
        }

        private static QueryInfo? searchByQuerySymbols(QueryTextSpan span, TextFieldInfo info)
        {
            var validQuerySymbols = info.ValidQuerySymbols();
            var text = span.Text;

            var possibleIndexOfQuerySymbol =
                text.Substring(0, span.CursorPosition)
                    .Select((character, index) => ((int?)index, character))
                    .Where(tuple => validQuerySymbols.Contains(tuple.Item2))
                    .Select(tuple => tuple.Item1)
                    .FirstOrDefault(querySymbolIndex =>
                    {
                        var previousIndex = querySymbolIndex.Value - 1;
                        var isValidSymbolIndex = previousIndex < 0 || char.IsWhiteSpace(text[previousIndex]);

                        return isValidSymbolIndex;
                    });

            if (possibleIndexOfQuerySymbol == null)
                return null;

            var indexOfQuerySymbol = possibleIndexOfQuerySymbol.Value;
            var startingIndex = indexOfQuerySymbol + 1;
            var stringLength = text.Length - indexOfQuerySymbol - 1;
            var type = getSuggestionType(text[indexOfQuerySymbol]);
            var queryText = text.Substring(startingIndex, stringLength);

            return new QueryInfo(queryText, type);
        }

        private static QueryInfo getDefaultQueryInfo(string text)
            => text.Length < minimumQueryLength
                ? emptyQueryInfo
                : new QueryInfo(text, AutocompleteSuggestionType.TimeEntries);

        private static AutocompleteSuggestionType getSuggestionType(char symbol)
            => symbol == QuerySymbols.Projects
                ? AutocompleteSuggestionType.Projects
                : AutocompleteSuggestionType.Tags;
    }
}
