using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Toggl.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoringCase(this string self, string value)
            => self.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;

        public static string UnicodeSafeSubstring(this string self, int startIndex, int length)
        {
            Ensure.Argument.IsNotNull(self, nameof(self));

            if (length == 0)
                return string.Empty;

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var textElementCount = self.LengthInGraphemes();

            if (startIndex < 0 || startIndex > textElementCount)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + length > textElementCount)
                throw new ArgumentOutOfRangeException(nameof(length));

            var sb = new StringBuilder();
            var enumerator = StringInfo.GetTextElementEnumerator(self);

            for (int i = 0; i < startIndex; i++)
                enumerator.MoveNext();

            int graphemesProcessed = 0;
            while (enumerator.MoveNext())
            {
                if (graphemesProcessed >= length)
                    break;

                var grapheme = enumerator.GetTextElement();

                sb.Append(grapheme);
                graphemesProcessed++;
            }

            return sb.ToString();
        }

        public static string TruncatedAt(this string self, int location)
            => self.Length <= location ? self : $"{self.UnicodeSafeSubstring(0, location - 3)}...";

        public static int LengthInBytes(this string self)
            => Encoding.UTF8.GetByteCount(self);

        public static int LengthInGraphemes(this string self)
            => new StringInfo(self).LengthInTextElements;

        public static int CountOccurrences(this string text, char token)
            => text.Count(c => c == token);

        public static string ToNullIfEmpty(this string text)
            => text?.Length == 0 ? null : text;
    }
}
