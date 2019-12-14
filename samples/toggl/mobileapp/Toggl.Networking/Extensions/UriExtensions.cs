using System;
using System.Text.RegularExpressions;

namespace Toggl.Networking.Extensions
{
    public static class UriExtensions
    {
        private static readonly Regex patternForId = new Regex(@"/\d+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private const string placeholderForId = "/{id}";

        public static Uri Anonymize(this Uri uri)
            => new Uri(patternForId.Replace(uri.ToString(), placeholderForId));
    }
}
