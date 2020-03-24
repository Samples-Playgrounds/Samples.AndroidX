using System;
using System.Collections.Generic;
using System.Linq;
using static System.StringSplitOptions;

namespace Toggl.Core.Extensions
{
    public static class StringExtensions
    {
        public static IList<string> SplitToQueryWords(this string text)
            => text.Split(new[] { ' ' }, RemoveEmptyEntries)
                .Distinct()
                .ToList();
    }
}
