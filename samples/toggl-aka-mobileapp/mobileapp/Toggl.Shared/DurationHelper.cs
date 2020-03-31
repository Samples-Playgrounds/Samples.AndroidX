using System.Text.RegularExpressions;

namespace Toggl.Shared
{
    public static class DurationHelper
    {
        private static readonly Regex prefixExpression = new Regex("^[0:]*(:|$)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static int LengthOfDurationPrefix(string durationText)
        {
            var match = prefixExpression.Match(durationText);
            return match.Success ? match.Length : 0;
        }
    }
}
