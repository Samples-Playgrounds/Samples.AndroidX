using System;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Autocomplete.Span
{
    public sealed class QueryTextSpan : TextSpan
    {
        public int CursorPosition { get; }

        public QueryTextSpan() : this("", 0) { }

        public QueryTextSpan(string text, int cursorPosition)
            : base(text)
        {
            if (!cursorPosition.IsInRange(0, text.Length))
                throw new ArgumentOutOfRangeException(nameof(cursorPosition), "The cursor cant be outside the bounds of the span text");

            CursorPosition = cursorPosition;
        }

        public QueryTextSpan WithoutQueryForSymbol(char querySymbol)
        {
            var indexOfProjectQuerySymbolInSpan = Text.IndexOf(querySymbol);
            var substringLength = indexOfProjectQuerySymbolInSpan >= 0 ? indexOfProjectQuerySymbolInSpan : Text.Length;
            var newQuerySpanText = Text.Substring(0, substringLength);
            var newQuerySpan = new QueryTextSpan(newQuerySpanText, newQuerySpanText.Length);
            return newQuerySpan;
        }
    }
}
