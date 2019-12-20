using System.Text.RegularExpressions;

namespace Toggl.Core.Helper
{
    public static class Colors
    {
        private const string pattern = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
        private static readonly Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        public const string NoProject = "#B5BCC0";
        public const string ClientNameColor = "#FF5E5B5B";

        public static bool IsValidHexColor(string hex)
        {
            if (hex == null)
            {
                return false;
            }
            return regex.Match(hex).Length > 0;
        }

        public static readonly string[] DefaultProjectColors =
        {
            "#06AAF5", "#C56BFF", "#EA468D", "#FB8B14", "#C7741C",
            "#F1C33F", "#E20505", "#4BC800", "#04BB9B", "#E19A86",
            "#3750B5", "#A01AA5", "#205500", "#000000"
        };
    }
}
